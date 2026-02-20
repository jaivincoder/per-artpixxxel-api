using api.artpixxel.data.Features.Accounts;
using api.artpixxel.data.Features.Checkouts;
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Emails;
using api.artpixxel.data.Features.Notifications;
using api.artpixxel.data.Models;
using api.artpixxel.data.Services;
using api.artpixxel.Data;
using api.artpixxel.repo.Utils.Generator.Config;
using api.artpixxel.repo.Utils.Generator;
using api.artpixxel.service.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using api.artpixxel.repo.Extensions;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using api.artpixxel.data.Features.OrderStatuses;
using Microsoft.Extensions.Hosting;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace api.artpixxel.repo.Features.Checkouts
{
    public class CheckOutNewService : ICheckOutNewService
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
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;


        public CheckOutNewService(ArtPixxelContext context, IWebHostEnvironment hostingEnvironment, RoleManager<UserRole> roleManager, IPaymentService paymentService,
             INotificationService notificationService, ISMTPService sMTPService, IOrderStatusService orderStatusService, IEmailService emailService, UserManager<User> userManager,
             ICurrentUserService currentUserService, IWebHostEnvironment environment, IConfiguration configuration)
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
            _environment = environment;
            _configuration = configuration;
        }

        public async Task<CheckoutResponse> CheckoutNew(CheckoutNew checkout)
        {
            try
            {
                foreach (var item in checkout.Cart.Items ?? Enumerable.Empty<CartItemNew>())
                {
                    var category = await _context.FrameCategories.Where(c => c.Id == item.CategoryId && c.IsDeleted == false)
                        .Select(c => new { c.CategoryType }).FirstOrDefaultAsync();

                    if (category == null)
                    {
                        throw new InvalidOperationException("CatogeryId Not Found!!");
                    }

                    if (category != null && category.CategoryType.Equals(framecategories.CategoryTypeArtMat, StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (var frame in item.Frames)
                        {

                            if (frame.Images == null || !frame.Images.Any() || frame.Images.Count <= 0)
                            {
                                return new CheckoutResponse
                                {
                                    TrackId = string.Empty,
                                    Response = new BaseResponse
                                    {
                                        Message = "Images are required for ArtMat category items.",
                                        Result = RequestResult.Error,
                                        Succeeded = false,
                                        Title = "Validation Error"
                                    }
                                };
                            }

                        }
                    }
                }

                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@checkout", checkout);
                Customer customer = null;
                if (@checkout.ShippingInformation.UserAccount == true)
                {
                    customer = await CreateCustomerAccount(@checkout);
                }
                else
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
                    if (await _context.OrderStatuses.AnyAsync(e => e.IsDefault == true))
                    {
                        status = await _context.OrderStatuses.Where(e => e.IsDefault == true).FirstOrDefaultAsync();
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
                            Name = await OrderName(checkout),
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
                            foreach (var cartCategory in checkout.Cart.Items)
                            {
                                foreach (var frame in cartCategory.Frames)
                                {
                                    var Image = await SaveFile(frame.PreviewImage);
                                    var orderItem = new OrderCartItems
                                    {
                                        OrderId = order.Id.ToString(),
                                        CategoryId = cartCategory.CategoryId,
                                        TotalAmountPerCategory = cartCategory.TotalAmountPerCategory,
                                        FrameId = frame.FrameId?.ToString(),
                                        TemplateConfigId = frame.templateConfigId,
                                        FrameClass = frame.FrameClass,
                                        LineColor = frame.LineColor,
                                        PreviewImage = Image,
                                        UniqueItemId = frame.UniqueItemId,
                                        Amount = frame.Amount,
                                        Quantity = frame.Quantity,
                                        FrameSize = frame.FrameSize,
                                    };

                                    _context.OrderCartItems.Add(orderItem);
                                    await _context.SaveChangesAsync();

                                    if (frame.Images != null && frame.Images.Any() && frame.Images.Count >= 0)
                                    {
                                        foreach (var img in frame.Images)
                                        {
                                            var croppedPath = await SaveFile(img.CroppedImages);
                                            var originalPath = await SaveFile(img.OriginalImages);
                                            var imageItem = new OrderCartItemImages
                                            {
                                                OrderCartItemId = orderItem.id,
                                                CroppedItemImage = croppedPath,
                                                OriginalItemImage = originalPath
                                            };

                                            _context.OrderCartItemImages.Add(imageItem);
                                        }

                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }
                        }
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
                        Title = "Unknown Error"
                    }
                };
            }


        }

        private async Task<string> OrderName(CheckoutNew checkout)
        {
            var firstItem = checkout.Cart.Items.FirstOrDefault();
            if (firstItem == null)
                return string.Empty;

            var category = await _context.FrameCategories.FirstOrDefaultAsync(e => e.Id == firstItem.CategoryId);

            if (category == null)
                return string.Empty;

            string categoryType = category.CategoryType;

            int imageCount = firstItem.Frames?.SelectMany(f => f.Images ?? new List<ItemCardImage>()).Count() ?? 0;

            if (categoryType.Equals("LinearArt", StringComparison.OrdinalIgnoreCase))
            {
                return $"{imageCount} UploadedImage";
            }
            if (categoryType.Equals("ArtMat", StringComparison.OrdinalIgnoreCase))
            {
                if (imageCount <= 1)
                    return "1 UploadedImage";

                return $"{imageCount} UploadedImages";
            }

            return categoryType;
        }



        private async Task<Customer> CreateCustomerAccount(CheckoutNew checkout)
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
                        await _context.SaveChangesAsync();

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
            catch (Exception ex)
            {
                throw (ex);
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
            var folderName = "CheckOut";

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
