using api.artpixxel.data.Features.Accounts;
using api.artpixxel.data.Features.Checkouts;
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Emails;
using api.artpixxel.data.Features.Notifications;
using api.artpixxel.data.Features.OrderStatuses;
using api.artpixxel.data.Models;
using api.artpixxel.data.Services;
using api.artpixxel.Data;
using api.artpixxel.repo.Extensions;
using api.artpixxel.repo.Utils.Generator;
using api.artpixxel.repo.Utils.Generator.Config;
using api.artpixxel.service.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace api.artpixxel.repo.Features.Checkouts
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ArtPixxelContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailService _emailService;
        private readonly ISMTPService _sMTPService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderStatusService _orderStatusService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuration; private readonly IWebHostEnvironment _environment;


        public CheckoutService(ArtPixxelContext context,
            IWebHostEnvironment hostingEnvironment,
             RoleManager<UserRole> roleManager,
             IPaymentService paymentService,
             INotificationService notificationService,
            ISMTPService sMTPService,
            IOrderStatusService orderStatusService,
            IEmailService emailService,
            UserManager<User> userManager,
            IConfiguration configuration,
            IWebHostEnvironment environment,

            ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _hostingEnvironment = hostingEnvironment;
            _emailService = emailService;
            _notificationService = notificationService;
            _sMTPService = sMTPService;
            _userManager = userManager;
            _orderStatusService = orderStatusService;
            _roleManager = roleManager;
            _paymentService = paymentService;
            _configuration = configuration;
            _environment = environment;
        }


        private string OrderName(Checkout checkout)
        {
            string name = string.Empty;
            if (checkout.Cart.Items.Any(e => e.Id.IsGuid() && e.Category == OrderItemCategory.MixnMatch.ToString()))
            {

                List<CartItem> mxMatches = checkout.Cart.Items.Where(e => e.Id.IsGuid() && e.Category == OrderItemCategory.MixnMatch.ToString()).ToList();
                if (mxMatches.Any())
                {
                    name = mxMatches.Count == 1 ? 1 + " " + OrderItemCategory.MixnMatch.ToString() : mxMatches.Count + " " + OrderItemCategory.MixnMatch.ToString() + "es";
                }

            }

            if (checkout.Cart.Items.Any(e => e.Category == OrderItemCategory.UploadedImage.ToString()))
            {
                List<CartItem> upImages = checkout.Cart.Items.Where(e => e.Category == OrderItemCategory.UploadedImage.ToString()).ToList();
                if (upImages.Any())
                {
                    string localname = upImages.Count == 1 ? 1 + " " + OrderItemCategory.UploadedImage.ToString() : upImages.Count + " " + OrderItemCategory.UploadedImage.ToString() + "es";
                    name = string.IsNullOrEmpty(name) ? localname : name + ", " + localname;

                }


            }

            if (checkout.Cart.Items.Any(e => e.Id.IsGuid() && e.Category == OrderItemCategory.WallArt.ToString()))
            {
                List<CartItem> wallImages = checkout.Cart.Items.Where(e => e.Id.IsGuid() && e.Category == OrderItemCategory.WallArt.ToString()).ToList();
                if (wallImages.Any())
                {
                    string localname = wallImages.Count == 1 ? 1 + " " + OrderItemCategory.WallArt.ToString() : wallImages.Count + " " + OrderItemCategory.WallArt.ToString() + "s";
                    name = string.IsNullOrEmpty(name) ? localname : name + ", " + localname;

                }
            }


            return name;
        }
        public async Task<CheckoutResponse> Checkout(Checkout checkout)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@checkout", checkout);

                Customer customer = null;

                if (@checkout.ShippingInformation.UserAccount == true)
                {


                    customer = await CreateCustomerAccount(@checkout);


                }
                else  // already a custoer
                {
                    customer = await _context.Customers.Where(e => e.UserId == _currentUserService.GetUserId()).Include(e => e.User).FirstOrDefaultAsync();


                }


                OrderStatus status = null;


                if (string.IsNullOrEmpty(checkout.PaymentIntentId))
                {

                    return new CheckoutResponse
                    {
                        TrackId = string.Empty,
                        Response = new BaseResponse
                        {
                            Message = "Payment intent error. Please try again later",
                            Result = RequestResult.Error,
                            Succeeded = false,
                            Title = "Payment Failure"
                        }
                    };

                }


                else
                {

                    string invoiceNumber = await UniqueInvoiceNumber();

                    //var _amount = (checkout.Cart.TotalAmount + checkout.DeliveryFee + checkout.Vat) * 100m;

                    //PaymentChargeResponse chargeResult = await _paymentService.ChargeToken(
                    //    new()
                    //    {
                    //        Amount = (long) _amount,
                    //        EmailAddress = checkout.ShippingInformation.EmailAddress,
                    //        InvoiceId = invoiceNumber,
                    //        TokenId = checkout.PaymentIntentId
                    //    });








                    if (await _context.OrderStatuses.AnyAsync(e => e.IsDefault == true))
                    {
                        status = await _context.OrderStatuses.Where(e => e.IsDefault == true).FirstOrDefaultAsync();
                    }


                    DateTime upperLimit = DateTime.ParseExact(checkout.UpperLimit, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    DateTime lowerLimit = DateTime.ParseExact(checkout.LowerLimit, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                    State state = string.IsNullOrEmpty(@checkout.ShippingInformation.State) ? null : _context.States.Find(@checkout.ShippingInformation.State);
                    Country country = string.IsNullOrEmpty(@checkout.ShippingInformation.Country) ? null : _context.Countries.Find(@checkout.ShippingInformation.Country);


                    Order order = new()
                    {
                        ShippingAddress = @checkout.ShippingInformation.ShippingAddress,
                        SubTotal = @checkout.Cart.TotalAmount,
                        DeliveryFee = @checkout.DeliveryFee,
                        VAT = @checkout.Vat,
                        GrandTotal = @checkout.Cart.TotalAmount + @checkout.DeliveryFee + @checkout.Vat,
                        InvoiceNumber = invoiceNumber,
                        CountryName = country == null ? "N/A" : country.Name,
                        StateName = state == null ? "N/A" : state.StateName,
                        CityName = string.IsNullOrWhiteSpace(@checkout.ShippingInformation.City) ? "N/A" : @checkout.ShippingInformation.City,
                        StateId = state == null ? null : state.Id,
                        // CityId = string.IsNullOrEmpty(@checkout.ShippingInformation.City) ? null : _context.Cities.Find(@checkout.ShippingInformation.City).Id,
                        CountryId = country == null ? null : country.Id,
                        CustomerId = customer == null ? null : customer.Id,
                        CustomerName = $"{@checkout.ShippingInformation.FirstName} {@checkout.ShippingInformation.LastName}",
                        UserId = customer == null ? null : _context.Users.Find(customer.UserId).Id,
                        ZipCode = @checkout.ShippingInformation.Zipcode,
                        Name = OrderName(@checkout),
                        EmailAddress = @checkout.ShippingInformation.EmailAddress,
                        MobilePhoneNumber = @checkout.ShippingInformation.MobilePhoneNumber,
                        AdditionalPhoneNumber = string.IsNullOrEmpty(@checkout.ShippingInformation.AdditionalPhoneNumber) ? null :
                        @checkout.ShippingInformation.AdditionalPhoneNumber,
                        StatusId = status?.Id,
                        OrderState = OrderState.Open,
                        PaymentIntentId = @checkout.PaymentIntentId,
                        Seen = false,
                        PaymentStatus = PaymentStatus.Unpaid,
                        SpeculativeDeliveryDate = lowerLimit,
                        DeliveryDate = upperLimit,
                    };

                    _context.Orders.Add(order);
                    int savres = await _context.SaveChangesAsync();

                    if (savres > 0)
                    {





                        if (checkout.Cart.Items.Any(e => e.Id.IsGuid() && e.Category == OrderItemCategory.MixnMatch.ToString()))
                        {

                            List<string> mixNMatchIds = checkout.Cart.Items.Where(e => e.Id.IsGuid() && e.Category == OrderItemCategory.MixnMatch.ToString()).Select(e => e.Id).ToList();

                            List<MixnMatch> mixnMatches = await _context.MixnMatches.Where(e => mixNMatchIds.Contains(e.Id)).ToListAsync();
                            if (mixnMatches.Any())
                            {
                                List<CartItem> mixNMatchOrders = checkout.Cart.Items.Where(e => e.Id.IsGuid() && e.Category == OrderItemCategory.MixnMatch.ToString()).ToList();
                                if (mixNMatchOrders.Any())
                                {

                                    List<string> sizeIds = mixNMatchOrders.Select(e => e.Size).ToList();
                                    List<Size> sizes = sizeIds.Any() ? await _context.Sizes.Where(e => sizeIds.Contains(e.Id)).ToListAsync() : new List<Size>();
                                    Size size = null;


                                    List<OrderItem> orderItems = new();
                                    foreach (CartItem mixNMatchOrder in mixNMatchOrders)
                                    {
                                        MixnMatch mixnMatch = mixnMatches.Where(e => e.Id == mixNMatchOrder.Id).FirstOrDefault();
                                        if (mixnMatch != null)
                                        {

                                            Size _size = size == null ? sizes.Where(e => e.Id == mixNMatchOrder.Size).FirstOrDefault()
                                            : size.Id == mixNMatchOrder.Size ? size : sizes.Where(e => e.Id == mixNMatchOrder.Size).FirstOrDefault();

                                            size = _size;

                                            OrderItem orderItem = new()
                                            {
                                                Amount = mixNMatchOrder.Amount,
                                                TotalAmount = mixNMatchOrder.TotalAmount,
                                                Category = (OrderItemCategory)Enum.Parse(typeof(OrderItemCategory), mixNMatchOrder.Category),
                                                Type = (OrderType)Enum.Parse(typeof(OrderType), mixNMatchOrder.Type),
                                                ImageAbsURL = mixnMatch.ImageAbsURL,
                                                ImageRelURL = mixnMatch.ImageRelURL,
                                                ImageURL = mixnMatch.ImageURL,
                                                SizeId = _size?.Id,
                                                SizeName = _size?.Name,
                                                OrderId = order.Id,
                                                Quantity = mixNMatchOrder.Quantity,
                                                ItemName = mixNMatchOrder.ItemName,
                                                Description = mixNMatchOrder.Description,
                                                FrameClass = mixNMatchOrder.FrameClass,
                                                Heading = mixNMatchOrder.Heading,
                                                MixnMatchId = mixnMatch.Id



                                            };

                                            orderItems.Add(orderItem);
                                        }
                                    }


                                    if (orderItems.Any())
                                    {

                                        _context.OrderItems.AddRange(orderItems);
                                        await _context.SaveChangesAsync();




                                    }
                                }
                            }
                        }




                        if (checkout.Cart.Items.Any(e => e.Category == OrderItemCategory.UploadedImage.ToString()))
                        {

                            List<CartItem> uploadedImageOrders = checkout.Cart.Items.Where(e => e.Category == OrderItemCategory.UploadedImage.ToString()).ToList();

                            if (uploadedImageOrders.Any())
                            {
                                List<OrderItem> orderItems = new();
                                List<OrderItemImage> orderItemImages = new();

                                List<string> sizedIds = new List<string>();
                                List<string> mixedTemplateSizeIds = new List<string>();
                                List<string> regularTemplateSizeIds = new List<string>();
                                List<string> kidsTemplateSizeIds = new List<string>();
                                List<string> christmasTemplateSizeIds = new List<string>();
                                List<KidsGalleryImage> uploaddedCharacters = new List<KidsGalleryImage>();
                                List<FestiveDesign> uploaddedFestiveDesigns = new List<FestiveDesign>();

                                List<string> uploaddedCharactersIds = new List<string>();
                                List<string> uploaddedFestiveDesignsIds = new List<string>();

                                if (uploadedImageOrders.Any(e => e.Type == OrderType.UploadRegular.ToString()))
                                {

                                    List<string> ids = uploadedImageOrders.Where(e => e.Type == OrderType.UploadRegular.ToString()).Select(e => e.Size).Distinct().ToList();
                                    if (ids.Any())
                                    {
                                        sizedIds.AddRange(ids);
                                    }

                                }

                                if (uploadedImageOrders.Any(e => e.Type == OrderType.FloralRegular.ToString()))
                                {

                                    List<string> ids = uploadedImageOrders.Where(e => e.Type == OrderType.FloralRegular.ToString()).Select(e => e.Size).Distinct().ToList();
                                    if (ids.Any())
                                    {

                                        if (sizedIds.Any())
                                        {
                                            ids = ids.Where(e => !sizedIds.Any(a => a == e)).ToList();
                                        }

                                        if (ids.Any())
                                        {
                                            sizedIds.AddRange(ids);
                                        }


                                    }

                                }


                                if (uploadedImageOrders.Any(e => e.Type == OrderType.KidsRegular.ToString()))
                                {

                                    List<string> ids = uploadedImageOrders.Where(e => e.Type == OrderType.KidsRegular.ToString()).Select(e => e.Size).Distinct().ToList();
                                    if (ids.Any())
                                    {

                                        if (sizedIds.Any())
                                        {
                                            ids = ids.Where(e => !sizedIds.Any(a => a == e)).ToList();
                                        }

                                        if (ids.Any())
                                        {
                                            sizedIds.AddRange(ids);
                                        }

                                    }

                                }


                                if (uploadedImageOrders.Any(e => e.Type == OrderType.ChristmasRegular.ToString()))
                                {

                                    List<string> ids = uploadedImageOrders.Where(e => e.Type == OrderType.ChristmasRegular.ToString()).Select(e => e.Size).Distinct().ToList();
                                    if (ids.Any())
                                    {

                                        if (sizedIds.Any())
                                        {
                                            ids = ids.Where(e => !sizedIds.Any(a => a == e)).ToList();
                                        }
                                        if (ids.Any())
                                        {
                                            sizedIds.AddRange(ids);
                                        }

                                    }

                                }





                                if (uploadedImageOrders.Any(e => e.Type == OrderType.MixedTemplate.ToString()))
                                {
                                    mixedTemplateSizeIds = uploadedImageOrders.Where(e => e.Type == OrderType.MixedTemplate.ToString()).Select(e => e.Size).Distinct().ToList();
                                }

                                if (uploadedImageOrders.Any(e => e.Type == OrderType.UploadTemplate.ToString()))
                                {
                                    regularTemplateSizeIds = uploadedImageOrders.Where(e => e.Type == OrderType.UploadTemplate.ToString()).Select(e => e.Size).Distinct().ToList();
                                }

                                if (uploadedImageOrders.Any(e => e.Type == OrderType.KidsTemplate.ToString()))
                                {
                                    kidsTemplateSizeIds = uploadedImageOrders.Where(e => e.Type == OrderType.KidsTemplate.ToString()).Select(e => e.Size).Distinct().ToList();
                                }

                                if (uploadedImageOrders.Any(e => e.Type == OrderType.ChristmasTemplate.ToString()))
                                {
                                    christmasTemplateSizeIds = uploadedImageOrders.Where(e => e.Type == OrderType.ChristmasTemplate.ToString()).Select(e => e.Size).Distinct().ToList();
                                }




                                Size size = null;
                                MixedTemplateSize mixedTemplateSize = null;
                                KidsTemplateSize kidsTemplateSize = null;
                                RegularTemplateSize regularTemplateSize = null;
                                ChristmasTemplateSize christmasTemplateSize = null;



                                if (uploadedImageOrders.Any(e => e.CroppedImage.IsBase64String() == true && (e.Image.IsBase64String() == false)))
                                {
                                    List<string> characterIds = uploadedImageOrders
                                        .Where(e => e.CroppedImage.IsBase64String() == true && (e.Image.IsBase64String() == false)
                                        && (e.Type == OrderType.KidsTemplate.ToString()))
                                        .Select(e => e.Id)
                                        .Distinct()
                                        .ToList();


                                    if (characterIds.Any())
                                    {
                                        uploaddedCharactersIds.AddRange(characterIds);


                                    }




                                    List<string> festiveDesignIds = uploadedImageOrders
                                       .Where(e => e.CroppedImage.IsBase64String() == true && (e.Image.IsBase64String() == false)
                                       && (e.Type != OrderType.KidsTemplate.ToString()))
                                       .Select(e => e.Id)
                                       .Distinct()
                                       .ToList();


                                    if (festiveDesignIds.Any())
                                    {
                                        uploaddedFestiveDesignsIds.AddRange(festiveDesignIds);


                                    }




                                }



                                if (uploadedImageOrders.Any(e => e.Type == OrderType.KidsTemplate.ToString()))
                                {
                                    List<ImageModel> allCharacters = uploadedImageOrders
                                        .Where(e => e.Type == OrderType.KidsTemplate.ToString())
                                        .SelectMany(e => e.Images)
                                        .ToList();

                                    if (allCharacters.Any(e => e.Type == ImageFileType.Character))
                                    {
                                        List<string> characterIds = allCharacters.Where(e => e.Type == ImageFileType.Character).Select(e => e.Id).Distinct().ToList();
                                        if (characterIds.Any())
                                        {
                                            uploaddedCharactersIds.AddRange(characterIds);
                                        }
                                    }
                                }


                                if (uploaddedCharactersIds.Any())
                                {
                                    uploaddedCharacters = await _context.KidsGalleryImages.Where(e => uploaddedCharactersIds.Contains(e.Id)).ToListAsync();
                                }


                                if (uploaddedFestiveDesignsIds.Any())
                                {
                                    uploaddedFestiveDesigns = await _context.FestiveDesigns.Where(e => uploaddedFestiveDesignsIds.Contains(e.Id)).ToListAsync();
                                }







                                List<Size> sizes = sizedIds.Any() ? await _context.Sizes.Where(e => sizedIds.Contains(e.Id)).ToListAsync() : new List<Size>();
                                List<MixedTemplateSize> mixedTemplateSizes = mixedTemplateSizeIds.Any() ? await _context.MixedTemplateSizes.Where(e => mixedTemplateSizeIds.Contains(e.Id)).ToListAsync() : new List<MixedTemplateSize>();
                                List<RegularTemplateSize> regularTemplateSizes = regularTemplateSizeIds.Any() ? await _context.RegularTemplateSizes.Where(e => regularTemplateSizeIds.Contains(e.Id)).ToListAsync() : new List<RegularTemplateSize>();
                                List<KidsTemplateSize> kidsTemplateSizes = kidsTemplateSizeIds.Any() ? await _context.KidsTemplateSizes.Where(e => kidsTemplateSizeIds.Contains(e.Id)).ToListAsync() : new List<KidsTemplateSize>();
                                List<ChristmasTemplateSize> christmasTemplateSizes = christmasTemplateSizeIds.Any() ? await _context.ChristmasTemplateSizes.Where(e => christmasTemplateSizeIds.Contains(e.Id)).ToListAsync() : new List<ChristmasTemplateSize>();



                                //TODO: get christmas template sizes


                                string rootPath = _hostingEnvironment.WebRootPath + "\\images\\UploadedImage";
                                if (!await rootPath.DirectoryExistAsync())
                                {
                                    await rootPath.CreateDirectoryAsync();
                                }




                                foreach (CartItem uploadedImageOrder in uploadedImageOrders)
                                {
                                    if (Enum.TryParse(uploadedImageOrder.Type, out OrderType orderType))
                                    {




                                        FileMeta fileMeta = null;
                                        FileMeta croppedFileMeta = null;
                                        FileMeta previewFileMeta = null;
                                        KidsGalleryImage kidsGallery = null;
                                        FestiveDesign festiveDesign = null;

                                        string imageUrl = string.Empty;
                                        string previewUrl = string.Empty;
                                        string croppedUrl = string.Empty;

                                        if (uploadedImageOrder.Image.IsBase64String())
                                        {

                                            if (!(string.IsNullOrEmpty(uploadedImageOrder.Image)))
                                            {


                                                //string outputPath = rootPath + "\\" + order.Id + "_" + uploadedImageOrder.Id;
                                                //fileMeta = await uploadedImageOrder.Image.SaveBase64AsImage(outputPath);


                                                if (!string.IsNullOrEmpty(uploadedImageOrder.Image))
                                                {
                                                    imageUrl = await SaveFile(uploadedImageOrder.Image);
                                                }
                                            }

                                            if (!(string.IsNullOrEmpty(uploadedImageOrder.PreviewImage)))
                                            {
                                                //string previewOutputPath = rootPath + "\\" + order.Id + "_preview_" + uploadedImageOrder.Id;
                                                //previewFileMeta = await uploadedImageOrder.PreviewImage.SaveBase64AsImage(previewOutputPath);

                                                if (!string.IsNullOrEmpty(uploadedImageOrder.PreviewImage))
                                                {
                                                    previewUrl = await SaveFile(uploadedImageOrder.PreviewImage);
                                                }
                                            }

                                            if (!(string.IsNullOrEmpty(uploadedImageOrder.CroppedImage)))
                                            {
                                                //string croppedOutputPath = rootPath + "\\" + order.Id + "_cropped_" + uploadedImageOrder.Id;
                                                //croppedFileMeta = await uploadedImageOrder.CroppedImage.SaveBase64AsImage(croppedOutputPath);

                                                if (!string.IsNullOrEmpty(uploadedImageOrder.CroppedImage))
                                                {
                                                    croppedUrl = await SaveFile(uploadedImageOrder.CroppedImage);
                                                }
                                            }
                                        }

                                        else
                                        {

                                            if (orderType == OrderType.KidsRegular)
                                            {
                                                kidsGallery = uploaddedCharacters.Where(e => e.Id == uploadedImageOrder.Id).FirstOrDefault();
                                            }
                                            else
                                            {
                                                festiveDesign = uploaddedFestiveDesigns.Where(e => e.Id == uploadedImageOrder.Id).FirstOrDefault();
                                            }



                                            if (!(string.IsNullOrEmpty(uploadedImageOrder.PreviewImage)))
                                            {
                                                string previewOutputPath = rootPath + "\\" + order.Id + "_preview_" + uploadedImageOrder.Id;
                                                previewFileMeta = await uploadedImageOrder.PreviewImage.SaveBase64AsImage(previewOutputPath);
                                            }
                                        }



                                        Size _size = null;
                                        MixedTemplateSize _mixedTemplateSize = null;
                                        KidsTemplateSize _kidsTemplateSize = null;
                                        RegularTemplateSize _regularTemplateSize = null;
                                        ChristmasTemplateSize _christmasTemplateSize = null;
                                        string sizeName = string.Empty;

                                        if ((orderType == OrderType.UploadRegular) || (orderType == OrderType.FloralRegular)
                                            || (orderType == OrderType.ChristmasRegular)
                                            || (orderType == OrderType.KidsRegular))
                                        {
                                            _size = size == null ? sizes.Where(e => e.Id == uploadedImageOrder.Size).FirstOrDefault()
                                      : size.Id == uploadedImageOrder.Size ? size : sizes.Where(e => e.Id == uploadedImageOrder.Size).FirstOrDefault();


                                            if (_size != null)
                                            {
                                                size = _size;
                                                sizeName = size.Name;
                                            }
                                        }


                                        else if (orderType == OrderType.MixedTemplate)
                                        {
                                            _mixedTemplateSize = mixedTemplateSize == null ? mixedTemplateSizes.Where(e => e.Id == uploadedImageOrder.Size).FirstOrDefault()
                                     : mixedTemplateSize.Id == uploadedImageOrder.Size ? mixedTemplateSize : mixedTemplateSizes.Where(e => e.Id == uploadedImageOrder.Size).FirstOrDefault();

                                            if (_mixedTemplateSize != null)
                                            {
                                                mixedTemplateSize = _mixedTemplateSize;
                                                sizeName = mixedTemplateSize.Name;
                                            }

                                        }


                                        else if (orderType == OrderType.UploadTemplate)
                                        {
                                            _regularTemplateSize = regularTemplateSize == null ? regularTemplateSizes.Where(e => e.Id == uploadedImageOrder.Size).FirstOrDefault()
                                     : regularTemplateSize.Id == uploadedImageOrder.Size ? regularTemplateSize : regularTemplateSizes.Where(e => e.Id == uploadedImageOrder.Size).FirstOrDefault();

                                            if (_regularTemplateSize != null)
                                            {
                                                regularTemplateSize = _regularTemplateSize;
                                                sizeName = regularTemplateSize.Name;
                                            }

                                        }


                                        else if (orderType == OrderType.ChristmasTemplate)
                                        {
                                            _christmasTemplateSize = christmasTemplateSize == null ? christmasTemplateSizes.Where(e => e.Id == uploadedImageOrder.Size).FirstOrDefault()
                                     : christmasTemplateSize.Id == uploadedImageOrder.Size ? christmasTemplateSize : christmasTemplateSizes.Where(e => e.Id == uploadedImageOrder.Size).FirstOrDefault();

                                            if (_christmasTemplateSize != null)
                                            {
                                                christmasTemplateSize = _christmasTemplateSize;
                                                sizeName = christmasTemplateSize.Name;
                                            }

                                        }

                                        else
                                        {
                                            _kidsTemplateSize = kidsTemplateSize == null ? kidsTemplateSizes.Where(e => e.Id == uploadedImageOrder.Size).FirstOrDefault()
                                    : kidsTemplateSize.Id == uploadedImageOrder.Size ? kidsTemplateSize : kidsTemplateSizes.Where(e => e.Id == uploadedImageOrder.Size).FirstOrDefault();
                                            if (_kidsTemplateSize != null)
                                            {
                                                kidsTemplateSize = _kidsTemplateSize;
                                                sizeName = kidsTemplateSize.Name;
                                            }

                                        }




                                        OrderItem orderItem = new()
                                        {
                                            Amount = uploadedImageOrder.Amount,
                                            TotalAmount = uploadedImageOrder.TotalAmount,
                                            Category = (OrderItemCategory)Enum.Parse(typeof(OrderItemCategory), uploadedImageOrder.Category),
                                            Type = (OrderType)Enum.Parse(typeof(OrderType), uploadedImageOrder.Type),
                                            ImageURL = imageUrl != null ? imageUrl : kidsGallery != null   ? kidsGallery.ImageRelURL : festiveDesign != null ? festiveDesign.ImageRelURL : string.Empty,
                                            ImageAbsURL = imageUrl == null ? kidsGallery == null ? festiveDesign == null ? string.Empty : festiveDesign.ImageAbsURL : kidsGallery.ImageAbsURL : _currentUserService.WebRoot() + imageUrl,
                                            ImageRelURL = imageUrl == null ? kidsGallery == null ? festiveDesign == null ? string.Empty : festiveDesign.ImageRelURL : kidsGallery.ImageRelURL : imageUrl,
                                            CroppedImageAbsURL = croppedFileMeta == null ? string.Empty : _currentUserService.WebRoot() + croppedUrl,
                                            CroppedImageURL = croppedUrl != null ? croppedUrl : string.Empty,
                                            PreviewImageAbsURL = previewUrl == null ? "" : _currentUserService.WebRoot() + previewUrl,
                                            PreviewImageURL = previewUrl != null  ? previewUrl : string.Empty,
                                            LineColor = uploadedImageOrder.LineColor,
                                            LineStyle = uploadedImageOrder.LineStyle,
                                            SizeName = sizeName,
                                            SizeId = _size == null ? null : _size?.Id,
                                            MixedTemplateSizeId = _mixedTemplateSize == null ? null : _mixedTemplateSize?.Id,
                                            KidsTemplateSizeId = _kidsTemplateSize == null ? null : _kidsTemplateSize?.Id,
                                            RegularTemplateSizeId = _regularTemplateSize == null ? null : _regularTemplateSize?.Id,
                                            ChristmasTemplateSizeId = _christmasTemplateSize == null ? null : _christmasTemplateSize?.Id,
                                            OrderId = order.Id,
                                            Design = uploadedImageOrder.Design,
                                            TempId = UniqueTempOrderItemId(orderItems),
                                            Quantity = uploadedImageOrder.Quantity,
                                            ItemName = uploadedImageOrder.ItemName,
                                            Description = uploadedImageOrder.Description,
                                            FrameClass = uploadedImageOrder.FrameClass,
                                            Heading = uploadedImageOrder.Heading

                                        };

                                        orderItems.Add(orderItem);

                                        if (
                                            (orderType == OrderType.KidsTemplate)
                                            || (orderType == OrderType.MixedTemplate)
                                            || (orderType == OrderType.UploadTemplate)
                                            || (orderType == OrderType.ChristmasTemplate))
                                        {
                                            if (uploadedImageOrder.Images.Any())
                                            {
                                                List<KidsGalleryImage> kidsGalleryImages = new List<KidsGalleryImage>();

                                                if (uploadedImageOrder.Images.Any(e => e.Type == ImageFileType.Character))
                                                {
                                                    List<string> characterIds = uploadedImageOrder.Images.Where(e => e.Type == ImageFileType.Character)
                                                        .Select(e => e.Id)
                                                        .Distinct().ToList();

                                                    if (characterIds.Any())
                                                    {
                                                        kidsGalleryImages = uploaddedCharacters
                                                            .Where(e => characterIds.Contains(e.Id))
                                                            .ToList();
                                                    }
                                                }


                                                foreach (ImageModel image in uploadedImageOrder.Images)
                                                {


                                                    if (image.Type == ImageFileType.LocalFile)
                                                    {
                                                        //string outputPath = rootPath + "\\" + order.Id + "_item_" + uploadedImageOrder.Id;
                                                        //FileMeta itemImageMeta = await image.Image.SaveBase64AsImage(outputPath);

                                                        //string outputPath2 = rootPath + "\\" + order.Id + "_Cropped_" + uploadedImageOrder.Id;
                                                        //FileMeta itemImageMeta2 = await image.CroppedImage.SaveBase64AsImage(outputPath2);
                                                        string imageUrl2 = !string.IsNullOrWhiteSpace(image.Image)  ? await SaveFile(image.Image) : string.Empty;

                                                        // Save cropped image
                                                        string croppedUrl2 = !string.IsNullOrWhiteSpace(image.CroppedImage)? await SaveFile(image.CroppedImage): string.Empty;

                                                        if (!string.IsNullOrEmpty(imageUrl2))
                                                        {
                                                            OrderItemImage orderItemImage = new()
                                                            {
                                                                ImageAbsURL = _currentUserService.WebRoot() + imageUrl2,
                                                                ImageRelURL = imageUrl2,
                                                                ImageURL =imageUrl2,
                                                                CroppedImageAbsURL= _currentUserService.WebRoot() + croppedUrl2,
                                                                CroppedImageRelURL= croppedUrl2,
                                                                CroppedImageURL= croppedUrl2,
                                                                OrderItemId = orderItem.TempId
                                                            };


                                                            orderItemImages.Add(orderItemImage);


                                                        }


                                                    }



                                                    else if (image.Type == ImageFileType.Character)
                                                    {
                                                        if (kidsGalleryImages.Any())
                                                        {
                                                            KidsGalleryImage kidsGalleryImage = kidsGalleryImages.Where(e => e.Id == image.Id).FirstOrDefault();
                                                            if (kidsGalleryImage != null)
                                                            {

                                                                OrderItemImage orderItemImage = new()
                                                                {
                                                                    ImageAbsURL = kidsGalleryImage.ImageAbsURL,
                                                                    ImageRelURL = kidsGalleryImage.ImageRelURL,
                                                                    ImageURL = kidsGalleryImage.ImageURL,
                                                                    CroppedImageURL = kidsGalleryImage.CroppedImageURL,
                                                                    CroppedImageRelURL = kidsGalleryImage.CroppedImageRelURL,
                                                                    CroppedImageAbsURL = kidsGalleryImage.CroppedImageAbsURL,
                                                                    OrderItemId = orderItem.TempId //temporary idea, used for temporary reference
                                                                };


                                                                orderItemImages.Add(orderItemImage);


                                                            }

                                                        }

                                                    }



                                                }
                                            }
                                        }

                                    }



                                }


                                if (orderItems.Any())
                                {
                                    _context.OrderItems.AddRange(orderItems);
                                    int ordersvResult = await _context.SaveChangesAsync();
                                    if (orderItemImages.Any())
                                    {
                                        if (ordersvResult > 0)
                                        {
                                            List<OrderItemImage> adjustedOrderItemImages = new();

                                            foreach (var orderItem in orderItems)
                                            {
                                                if (orderItemImages.Any(e => e.OrderItemId == orderItem.TempId))
                                                {
                                                    List<OrderItemImage> itemImages = orderItemImages.Where(e => e.OrderItemId == orderItem.TempId).ToList();
                                                    if (itemImages.Any())
                                                    {
                                                        foreach (OrderItemImage itemImage in itemImages)
                                                        {
                                                            itemImage.OrderItemId = orderItem.Id;
                                                            adjustedOrderItemImages.Add(itemImage);

                                                        }
                                                    }
                                                }
                                            }

                                            if (adjustedOrderItemImages.Any())
                                            {
                                                _context.OrderItemImages.AddRange(adjustedOrderItemImages);
                                                await _context.SaveChangesAsync();
                                            }

                                        }
                                    }




                                }
                            }

                        }

                        if (@checkout.Cart.Items.Any(e => e.Id.IsGuid() && e.Category == OrderItemCategory.WallArt.ToString()))
                        {
                            List<string> wallartIds = checkout.Cart.Items.Where(e => e.Id.IsGuid() && e.Category == OrderItemCategory.WallArt.ToString()).Select(e => e.Id).ToList();
                            List<string> wallartSizeIds = checkout.Cart.Items.Where(e => e.Id.IsGuid() && e.Category == OrderItemCategory.WallArt.ToString()).Select(e => e.Size).ToList();

                            List<WallArt> wallArts = await _context.WallArts.Where(e => wallartIds.Contains(e.Id)).Include(e => e.Images).ToListAsync();
                            if (wallArts.Any() && wallartSizeIds.Any())
                            {
                                List<CartItem> wallArtOrders = checkout.Cart.Items.Where(e => e.Id.IsGuid() && e.Category == OrderItemCategory.WallArt.ToString()).ToList();
                                if (wallArtOrders.Any())
                                {
                                    WallArtSize size = null;

                                    List<OrderItem> orderItems = new List<OrderItem>();
                                    List<OrderItemImage> orderItemImages = new List<OrderItemImage>();
                                    List<WallArtSize> sizes = await _context.WallArtSizes.Where(e => wallartSizeIds.Contains(e.Id)).ToListAsync();

                                    if (sizes.Any())
                                    {



                                        foreach (CartItem wallArtOrder in wallArtOrders)
                                        {
                                            WallArt wallArt = wallArts.Where(e => e.Id == wallArtOrder.Id).FirstOrDefault();
                                            if (wallArt != null)
                                            {


                                                WallArtSize wallArtSize = size == null ? sizes.Where(e => e.Id == wallArtOrder.Size).FirstOrDefault()
                                                    : size.Id == wallArtOrder.Size ? size : sizes.Where(e => e.Id == wallArtOrder.Size).FirstOrDefault();
                                                size = wallArtSize;

                                                if (wallArtSize != null)
                                                {


                                                    OrderItem orderItem = new()
                                                    {
                                                        Amount = wallArtOrder.Amount,
                                                        TotalAmount = wallArtOrder.TotalAmount,
                                                        WallArtId = wallArt.Id,
                                                        Category = (OrderItemCategory)Enum.Parse(typeof(OrderItemCategory), wallArtOrder.Category),
                                                        Type = (OrderType)Enum.Parse(typeof(OrderType), wallArtOrder.Type),
                                                        ImageAbsURL = wallArt.ImageAbsURL,
                                                        ImageRelURL = wallArt.ImageRelURL,
                                                        ImageURL = wallArt.ImageURL,
                                                        WallArtSizeId = wallArtSize.Id, // _context.WallArtSizes.Find(wallArtOrder.Size).Id,
                                                        SizeName = wallArtSize.Name,
                                                        OrderId = order.Id,
                                                        Quantity = wallArtOrder.Quantity,
                                                        ItemName = wallArtOrder.ItemName,
                                                        Description = wallArtOrder.Description,
                                                        FrameClass = wallArtOrder.FrameClass,
                                                        Heading = wallArtOrder.Heading,

                                                    };

                                                    orderItems.Add(orderItem);
                                                }




                                                //_context.OrderItems.Add(orderItem);
                                                //int oiSaveRes = await _context.SaveChangesAsync();

                                                //if (oiSaveRes > 0)
                                                //{
                                                //    List<OrderItemImage> orderItemImages = wallArt.Images.Select(e => new OrderItemImage
                                                //    {
                                                //        ImageAbsURL = e.ImageAbsURL,
                                                //        ImageRelURL = e.ImageRelURL,
                                                //        ImageURL = e.ImageURL,
                                                //        WallArtImageId = e.Id,
                                                //        OrderItemId = orderItem.Id
                                                //    }).ToList();


                                                //    _context.OrderItemImages.AddRange(orderItemImages);
                                                //    await _context.SaveChangesAsync();


                                                //}
                                            }

                                        }


                                        if (orderItems.Any())
                                        {
                                            _context.OrderItems.AddRange(orderItems);
                                            int wzAddResult = await _context.SaveChangesAsync();

                                            if (wzAddResult > 0)
                                            {
                                                foreach (OrderItem orderItem in orderItems)
                                                {
                                                    WallArt wallArt = wallArts.Where(e => e.Id == orderItem.WallArtId).FirstOrDefault();
                                                    if (wallArt != null)
                                                    {
                                                        if (wallArt.Images.Any())
                                                        {
                                                            List<OrderItemImage> _orderItemImages = wallArt.Images.Select(e => new OrderItemImage
                                                            {
                                                                ImageAbsURL = e.ImageAbsURL,
                                                                ImageRelURL = e.ImageRelURL,
                                                                ImageURL = e.ImageURL,
                                                                WallArtImageId = e.Id,
                                                                OrderItemId = orderItem.Id
                                                            }).ToList();

                                                            if (_orderItemImages.Any())
                                                            {
                                                                orderItemImages.AddRange(_orderItemImages);

                                                            }
                                                        }
                                                    }
                                                }


                                                if (orderItemImages.Any())
                                                {
                                                    _context.OrderItemImages.AddRange(orderItemImages);
                                                    await _context.SaveChangesAsync();
                                                }


                                            }
                                        }


                                    }
                                }

                            }
                        }



                        //continue after saving order


                        //send order notifcation to admin

                        List<string> admins = await _context.Users.Where(e => e.IsAdmin == true).Select(e => e.Id).ToListAsync();
                        if (admins.Any())
                        {
                            await _notificationService.SendNotification(
                                     new NotificationModel
                                     {
                                         Recipients = admins,
                                         NotificationRequest = new NotificationContent
                                         {
                                             AccessType = AccessType.Specific,
                                             SubjectId = customer == null ? null : customer.UserId,
                                             NotificationPriority = NotificationPriority.Urgent,
                                             Message = $"{(customer == null ? order.CustomerName : customer.User.FullName)} placed an order.",
                                             Title = "New Order"

                                         }
                                     });
                        }


                        if (status != null)
                        {





                            if (await _context.DefNotifications.AnyAsync())
                            {
                                DefNotification defNotification = await _context.DefNotifications.FirstOrDefaultAsync();
                                if (defNotification != null)
                                {
                                    await _orderStatusService.SendNotification(new OrderStatusNotificationRequest
                                    {
                                        Message = defNotification.Message,
                                        Option = defNotification.NotificationOption,
                                        Order = order
                                    });



                                    OrderStatusHistory history = new()
                                    {
                                        ColorCode = status.ColorCode,
                                        Comment = defNotification.Message,
                                        NotificationOption = NotificationOption.EmailAndMessage,
                                        Icon = status.Icon,
                                        Label = status.Label,
                                        OrderId = order.Id,
                                        StatusId = status.Id,

                                    };

                                    _context.OrderStatusHistories.Add(history);
                                    await _context.SaveChangesAsync();
                                }
                            }


                            else
                            {
                                OrderStatusHistory history = new()
                                {
                                    ColorCode = status.ColorCode,
                                    Comment = "Default",
                                    NotificationOption = NotificationOption.EmailAndMessage,
                                    Icon = status.Icon,
                                    Label = status.Label,
                                    OrderId = order.Id,
                                    StatusId = status.Id,

                                };

                                _context.OrderStatusHistories.Add(history);
                                await _context.SaveChangesAsync();
                            }
                        }




                        CheckoutNotification checkoutNotification = new()
                        {
                            Order = order,
                            FullName = checkout.ShippingInformation.FirstName + " " + checkout.ShippingInformation.LastName,
                            EmailAddress = checkout.ShippingInformation.EmailAddress,
                            AdditionalPhoneNumber = string.IsNullOrEmpty(checkout.ShippingInformation.AdditionalPhoneNumber) ? string.Empty :
                            checkout.ShippingInformation.AdditionalPhoneNumber,
                            ZipCode = checkout.ShippingInformation.Zipcode,
                            ShippingAddress = checkout.ShippingInformation.ShippingAddress,
                            City = checkout.ShippingInformation.City,
                            // City = string.IsNullOrEmpty(checkout.ShippingInformation.City) ? string.Empty : await _context.Cities.Where(e => e.Id == checkout.ShippingInformation.City).Select(e => e.CityName).FirstOrDefaultAsync(),
                            State = string.IsNullOrEmpty(checkout.ShippingInformation.State) ? string.Empty : await _context.States.Where(e => e.Id == checkout.ShippingInformation.State).Select(e => e.StateName).FirstOrDefaultAsync(),
                            Country = string.IsNullOrEmpty(checkout.ShippingInformation.Country) ? string.Empty : await _context.Countries.Where(e => e.Id == checkout.ShippingInformation.Country).Select(e => e.Name).FirstOrDefaultAsync(),
                            MobilePhoneNumber = string.IsNullOrEmpty(checkout.ShippingInformation.MobilePhoneNumber) ? string.Empty :
                            checkout.ShippingInformation.MobilePhoneNumber,


                        };

                        //send order notification mail to admin
                        // send order confirmation to customer
                        await _sMTPService.SendAdminOrderNotification(checkoutNotification);
                        await _sMTPService.SendCustomerOrderNotification(checkoutNotification);





                        if (customer != null)
                        {
                            customer.LastOrder = checkout.Cart.TotalAmount;
                            customer.TotalOrder += checkout.Cart.TotalAmount;
                            List<decimal> previous = await _context.Orders.Where(e => e.CustomerId == customer.Id).Select(e => e.SubTotal).ToListAsync();
                            customer.AverageOrder = previous.Any() ? previous.Average() : checkout.Cart.TotalAmount;

                            _context.Customers.Update(customer);
                            await _context.SaveChangesAsync();
                        }





                        return new CheckoutResponse
                        {
                            Response = new BaseResponse
                            {
                                Message = "Order successfully placed.",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"

                            }
                            ,
                            TrackId = order.Id
                        };



                    }


                }


                return new CheckoutResponse
                {
                    TrackId = string.Empty,
                    Response = new BaseResponse
                    {
                        Message = "An error occurred. Order could not be placed.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Unknown Error"
                    }
                };


            }
            catch (Exception ex)
            {

                return new CheckoutResponse
                {
                    TrackId = string.Empty,
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    }
                };





            }
        }




        private async Task<string> UniqueInvoiceNumber()
        {
            string invoiceNumber = (Generators.Generate(new GenerationOptions(useSpecialCharacters: false, useNumbers: true, length: 15))).ToUpper();
            if (await _context.Orders.AnyAsync(e => e.InvoiceNumber == invoiceNumber))
            {
                invoiceNumber = await UniqueInvoiceNumber();
            }

            return invoiceNumber;
        }


        private string UniqueTempOrderItemId(List<OrderItem> orderItems)
        {
            string tempId = (Generators.Generate(new GenerationOptions(useSpecialCharacters: false, useNumbers: true, length: 20))).ToUpper();

            if (orderItems.Any())
            {
                if (orderItems.Any(e => e.TempId == tempId))
                {
                    tempId = UniqueTempOrderItemId(orderItems);
                }
            }


            return tempId;
        }


        private async Task<Customer> CreateCustomerAccount(Checkout checkout)
        {
            try
            {
                Customer newCustomer = null;




                User user = new()
                {
                    Email = checkout.ShippingInformation.EmailAddress,
                    UserName = checkout.ShippingInformation.Username,
                    FirstName = checkout.ShippingInformation.FirstName,
                    LastName = checkout.ShippingInformation.LastName,
                    PhoneNumber = checkout.ShippingInformation.MobilePhoneNumber,
                    StateId = string.IsNullOrEmpty(checkout.ShippingInformation.State) ? null : _context.States.Find(checkout.ShippingInformation.State).Id,
                    HomeAddress = checkout.ShippingInformation.ShippingAddress

                };

                IdentityResult result = await _userManager.CreateAsync(user, checkout.ShippingInformation.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, DefaultRoles.Customer);
                    List<Claim> userClaims = new()
                    {
                        new Claim(ClaimTypes.Role, DefaultRoles.Customer),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Surname, user.LastName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.MobilePhone, user.PhoneNumber)
                    };

                    await _userManager.AddClaimsAsync(user, userClaims);



                    string categoryId = null;
                    if (await _context.CustomerCategories.AnyAsync(e => e.IsDefault == true))
                    {
                        CustomerCategory customerCategory = await _context.CustomerCategories.Where(e => e.IsDefault == true).FirstOrDefaultAsync();
                        if (customerCategory != null)
                        {
                            categoryId = customerCategory.Id;
                        }
                    };

                    Customer customer = new()
                    {
                        UserId = _context.Users.Find(user.Id).Id,
                        CityId = string.IsNullOrEmpty(checkout.ShippingInformation.City) ? null : _context.Cities.Find(checkout.ShippingInformation.City).Id,
                        CategoryId = categoryId,
                        LastOrder = 0m,
                        TotalOrder = 0m,
                        AverageOrder = 0m,
                        SubscribedNewsletter = true

                    };


                    _context.Customers.Add(customer);
                    int acct = await _context.SaveChangesAsync();




                    if (acct > 0)
                    {

                        newCustomer = await _context.Customers.Where(e => e.Id == customer.Id).Include(e => e.User).FirstAsync();

                        await _emailService.AddToEmailList(new EmailListRequest
                        {
                            EmailAddress = checkout.ShippingInformation.EmailAddress,
                            FirstName = checkout.ShippingInformation.FirstName,
                            LastName = checkout.ShippingInformation.LastName
                        });

                        AddressBook addressBook = new()
                        {
                            AdditionalInformation = string.Empty,
                            Address = checkout.ShippingInformation.ShippingAddress,
                            CityId = string.IsNullOrEmpty(checkout.ShippingInformation.City) ? null : _context.Cities.Find(checkout.ShippingInformation.City).Id,
                            CustomerId = _context.Customers.Find(customer.Id).Id,
                            IsDefault = true,
                            Name = "Default AddressBook"

                        };
                        _context.AddressBooks.Add(addressBook);
                        int saveResult = await _context.SaveChangesAsync();



                        await _sMTPService.SendWelcomeMail(new AccountModel
                        {
                            EmailAddress = checkout.ShippingInformation.EmailAddress,
                            FirstName = checkout.ShippingInformation.FirstName,
                            LastName = checkout.ShippingInformation.LastName,
                            Username = checkout.ShippingInformation.Username,
                            Password = checkout.ShippingInformation.Password
                        });


                        List<string> admins = await _context.Users.Where(e => e.IsAdmin == true).Select(e => e.Id).ToListAsync();
                        if (admins.Any())
                        {
                            await _notificationService.SendNotification(
                                     new NotificationModel
                                     {
                                         Recipients = admins,
                                         NotificationRequest = new NotificationContent
                                         {
                                             AccessType = AccessType.Specific,
                                             SubjectId = user.Id,
                                             NotificationPriority = NotificationPriority.Default,
                                             Message = user.FullName + " created a customer account.",
                                             Title = "New Customer Signup"

                                         }
                                     });
                        }




                    }
                }




                return newCustomer;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<string> SaveFile(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                return null;

            string extension = "";
            string pureBase64 = base64String;

            if (base64String.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                var header = base64String.Split(',')[0];
                var mime = header.Split(':')[1].Split(';')[0];

                if (!mime.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Unsupported file type. Only images allowed.");

                extension = GetExtensionFromMime(mime);
                pureBase64 = base64String.Split(',')[1];
            }

            pureBase64 = pureBase64.Trim();

            byte[] fileBytes;
            try
            {
                fileBytes = Convert.FromBase64String(pureBase64);
            }
            catch
            {
                throw new Exception("Invalid Base64 string.");
            }

            if (string.IsNullOrEmpty(extension))
            {
                extension = DetectImageExtension(fileBytes);

                if (string.IsNullOrEmpty(extension))
                    throw new Exception("Unsupported or invalid image.");
            }

            var uploadBase = _configuration["FileStorage:UploadFolder"] ?? "";
            var folderName = "UploadedImage";

            var uploadsFolder = Path.Combine(_environment.ContentRootPath, uploadBase, folderName);
            Directory.CreateDirectory(uploadsFolder);

            string fileName = $"{DateTime.Now:ddMMyyyy_HHmmssfff}{extension}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            await File.WriteAllBytesAsync(filePath, fileBytes);

            return $"/images/{folderName}/{fileName}";
        }

        private string GetExtensionFromMime(string mime)
        {
            return mime.ToLower() switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                "image/gif" => ".gif",
                "image/bmp" => ".bmp",
                "image/svg+xml" => ".svg",
                _ => throw new Exception("Unsupported image type.")
            };
        }

        private string DetectImageExtension(byte[] bytes)
        {
            // JPEG FF D8 FF
            if (bytes.Length > 3 &&
                bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
                return ".jpg";

            // PNG 89 50 4E 47
            if (bytes.Length > 4 &&
                bytes[0] == 0x89 && bytes[1] == 0x50 &&
                bytes[2] == 0x4E && bytes[3] == 0x47)
                return ".png";

            // GIF 47 49 46
            if (bytes.Length > 3 &&
                bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46)
                return ".gif";

            // WEBP "RIFF" + "WEBP"
            if (bytes.Length > 12 &&
                Encoding.ASCII.GetString(bytes, 0, 4) == "RIFF" &&
                Encoding.ASCII.GetString(bytes, 8, 4) == "WEBP")
                return ".webp";

            // SVG (text-based) — check beginning for XML or <svg tag
            try
            {
                int len = Math.Min(bytes.Length, 1024); // inspect up to first 1KB
                string start = Encoding.UTF8.GetString(bytes, 0, len).TrimStart();

                // common patterns: <?xml ... ?> or <svg ...>
                if (start.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase) ||
                    start.IndexOf("<svg", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return ".svg";
                }
            }
            catch
            {
                // ignore decoding errors and fall through
            }

            return null;
        }
    }
}
