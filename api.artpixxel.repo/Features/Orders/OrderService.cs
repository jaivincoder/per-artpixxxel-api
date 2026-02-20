using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Orders;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using api.artpixxel.repo.Extensions;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using api.artpixxel.data.Models;
using api.artpixxel.data.Features.LeadTimes;
using api.artpixxel.data.Features.OrderStatuses;
using static api.artpixxel.repo.Utils.Orders.OrderFilterUtils;
using LinqKit;
using api.artpixxel.data.Services;
using api.artpixxel.data.Features.Checkouts;
using api.artpixxel.data.Features.States;

namespace api.artpixxel.repo.Features.Orders
{
    public class OrderService : IOrderService
    {
        private readonly ArtPixxelContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICurrentUserService _currentUserService;

        public OrderService(ArtPixxelContext context,
            ICurrentUserService currentUserService,
            IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _currentUserService = currentUserService;


        }





        private List<OrderModel> FormatOrder(List<Order> orders)
        {
            try
            {


                if (orders == null || !orders.Any())
                    return new List<OrderModel>();

                var allImages = _context.OrderCartItemImages.ToList();

                var allSize = _context.Sizes.ToList();

                var allFrames = _context.Frames.ToList();

                var items = _context.OrderCartItems.Where(x => orders.Select(o => o.Id).Contains(x.OrderId)).ToList();

                return orders.Any() ? orders.Select(o => new OrderModel
                {
                    Id = o.Id.ToString(),
                    SubTotal = o.SubTotal,
                    ShippingAddress = o.ShippingAddress,
                    City = string.IsNullOrEmpty(o.CityId) ? string.IsNullOrEmpty(o.CityName) ? new BaseOption { Id = string.Empty, Name = string.Empty } : new BaseOption { Id = string.Empty, Name = o.CityName } : new BaseOption { Id = o.City?.Id ?? string.Empty, Name = o.City?.CityName ?? string.Empty },
                    State = string.IsNullOrEmpty(o.StateId) ? new BaseOption { Id = string.Empty, Name = string.Empty } : new BaseOption { Id = o.State?.Id ?? string.Empty, Name = o.State?.StateName ?? string.Empty },
                    Country = string.IsNullOrEmpty(o.CountryId) ? new CountryOption { Id = string.Empty, Name = string.Empty, Flag = string.Empty } : new CountryOption { Id = o.Country.Id, Name = o.Country.Name, Flag = o.Country.Flag.Name },
                    FullName = string.IsNullOrEmpty(o.CustomerName) ? o.Customer == null ? string.Empty : o.Customer.User.FullName : o.CustomerName,
                    CustomerId = o.CustomerId ?? string.Empty,
                    Name = o.Name,
                    InvoiceNumber = o.InvoiceNumber,
                    FullDate = o.CreatedOn.ToString("dddd, dnn MMMM yyyy", useExtendedSpecifiers: true),
                    MobilePhoneNumber = o.MobilePhoneNumber,
                    AdditionalMobileNumber = o.AdditionalPhoneNumber,
                    DeliveryDate = o.DeliveryDate.ToString("dddd, dnn MMMM yyyy", useExtendedSpecifiers: true),
                    SpecDeliveryDate = o.SpeculativeDeliveryDate.ToString("dddd, dnn MMMM yyyy", useExtendedSpecifiers: true),
                    ZipCode = string.IsNullOrEmpty(o.ZipCode) ? "" : o.ZipCode,
                    EmailAddress = o.EmailAddress,
                    OrderState = new BaseOption { Id = string.Empty, Name = o.OrderState.ToString() },
                    PaymentStatus = new BaseOption { Id = string.Empty, Name = o.PaymentStatus.ToString() },
                    Status = string.IsNullOrEmpty(o.StatusId) ? new OrderModelStatusBase { Id = string.Empty, Label = string.Empty, ColorCode = string.Empty, Icon = string.Empty } : new OrderModelStatusBase { Id = o.StatusId, Label = o.Status.Label, ColorCode = o.Status.ColorCode, Icon = o.Status.Icon },
                    Date = o.CreatedOn.ToString(DefaultDateFormat.ddMMyyyy),
                    VAT = decimal.Round(o.VAT, 2, MidpointRounding.AwayFromZero),
                    DeliveryFee = decimal.Round(o.DeliveryFee, 2, MidpointRounding.AwayFromZero),
                    GrandTotal = decimal.Round(o.GrandTotal, 2, MidpointRounding.AwayFromZero),
                    Items = o.Items.Select(i => new OrderModelItem
                    {
                        Amount = i.TotalAmount,

                        Category = new BaseOption { Id = string.Empty, Name = i.Category.ToString() },
                        FrameClass = i.FrameClass,
                        ImageURL =  GetRelativeImageUrl(i.ImageURL),
                        PreviewImageURL = GetRelativeImageUrl(i.PreviewImageURL),
                        CroppedImageURL = GetRelativeImageUrl(i.CroppedImageURL),
                        Description = i.Description,
                        Heading = i.Heading,
                        Id = i.Id,
                        ItemName = i.ItemName,
                        Quantity = i.Quantity,
                        DownloadingOriginal = false,
                        DownloadingCropped = false,
                        Size = i.Type == OrderType.Regular ? string.IsNullOrEmpty(i.WallArtSizeId) ?
                         string.IsNullOrEmpty(i.SizeId) ? new BaseOption { Id = string.Empty, Name = i.SizeName } : new BaseOption { Id = i.SizeId, Name = i.Size.Name }
                         : new BaseOption { Id = i.WallArtSize.Id, Name = i.WallArtSize.Name }
                        : i.Type == OrderType.MixedTemplate ? string.IsNullOrEmpty(i.MixedTemplateSizeId) ? new BaseOption { Id = string.Empty, Name = i.SizeName } : new BaseOption { Id = i.MixedTemplateSize.Id, Name = i.MixedTemplateSize.Name }
                        : i.Type == OrderType.KidsTemplate ? string.IsNullOrEmpty(i.KidsTemplateSizeId) ? new BaseOption { Id = string.Empty, Name = i.SizeName } : new BaseOption { Id = i.KidsTemplateSize.Id, Name = i.KidsTemplateSize.Name }
                        : i.Type == OrderType.UploadTemplate ? string.IsNullOrEmpty(i.RegularTemplateSizeId) ? new BaseOption { Id = string.Empty, Name = i.SizeName } : new BaseOption { Id = i.RegularTemplateSize.Id, Name = i.RegularTemplateSize.Name }
                        : i.Type == OrderType.ChristmasTemplate ? string.IsNullOrEmpty(i.ChristmasTemplateSizeId) ? new BaseOption { Id = string.Empty, Name = i.SizeName } : new BaseOption { Id = i.ChristmasTemplateSize.Id, Name = i.ChristmasTemplateSize.Name }
                        : new BaseOption { Id = string.Empty, Name = i.SizeName },
                        FrameSizeName = i.SizeName,
                        Images = i.Images.Any() ?
                        i.Images.Select(im => new OrderModelItemImage
                        {
                            Id = im.Id,
                            Amount = decimal.Round((i.TotalAmount / i.Images.Count()), 2, MidpointRounding.AwayFromZero),
                            Description = string.IsNullOrEmpty(im.WallArtImageId) ? i.Description : im.WallArtImage.Description,
                            FrameClass = i.FrameClass,
                            Downloading = false,
                            Name = string.IsNullOrEmpty(im.WallArtImageId) ? i.ItemName : im.WallArtImage.Name,
                            Quantity = i.Quantity,
                            Category = i.Type == OrderType.Regular ? "WallArt Image" : i.Type == OrderType.MixedTemplate ? "Mixed Template Image" : i.Type == OrderType.ChristmasTemplate ? "Christmas Image" : "Kids Template Image",
                            Size = i.Type == OrderType.Regular ? string.IsNullOrWhiteSpace(i.WallArtSizeId) ? string.Empty : i.WallArtSize.Name
                            : i.Type == OrderType.MixedTemplate ? string.IsNullOrWhiteSpace(i.MixedTemplateSizeId) ? string.Empty : i.MixedTemplateSize.Name
                            : i.Type == OrderType.KidsTemplate ? string.IsNullOrWhiteSpace(i.KidsTemplateSizeId) ? string.Empty : i.KidsTemplateSize.Name
                            : i.Type == OrderType.UploadTemplate ? string.IsNullOrWhiteSpace(i.RegularTemplateSizeId) ? string.Empty : i.RegularTemplateSize.Name
                            : i.Type == OrderType.ChristmasTemplate ? string.IsNullOrWhiteSpace(i.ChristmasTemplateSizeId) ? string.Empty : i.ChristmasTemplateSize.Name
                            : string.Empty,

                            ImageURL = GetRelativeImageUrl(im.ImageURL),
                            CroppedImageURL = GetRelativeImageUrl(im.CroppedImageURL)

                        }).ToList() : new List<OrderModelItemImage>()


                    }).ToList(),
                    ItemsNew = o.ItemsNew.GroupBy(i => i.CategoryId)
                        .Select(g =>
                        {
                            var frames = g.Select(i =>
                            {
                                var images = allImages.Where(img => img.OrderCartItemId == i.id)
                                    .Select(img => new ItemCardImage
                                    {
                                        CroppedImages = img.CroppedItemImage,
                                        OriginalImages = img.OriginalItemImage
                                    })
                                    .ToList();
                                var framePrice = allFrames.FirstOrDefault(f => f.Id == i.FrameId)?.FramePrice ?? 0;

                                var sizeName = allSize.FirstOrDefault(s => s.Id == i.FrameSize)?.Name ?? "Unknown";
                                return new FrameSet
                                {
                                    FrameId = i.FrameId,
                                    templateConfigId = i.TemplateConfigId,
                                    FrameClass = i.FrameClass,
                                    LineColor = i.LineColor,
                                    PreviewImage = i.PreviewImage,
                                    UniqueItemId = i.UniqueItemId,
                                    Images = images,
                                    Amount = i.TotalAmountPerCategory,
                                    Quantity = i.Quantity,
                                    FrameSize = i.FrameSize,
                                    FrameSizeName = sizeName,
                                    FramePrice = framePrice
                                };
                            }).ToList();

                            return new CartItemNew
                            {
                                CategoryId = g.Key,
                                TotalAmountPerCategory = g.Sum(x => x.TotalAmountPerCategory),
                                Frames = frames
                            };
                        })
                        .ToList(),
                    Histories = o.Histories.Any() ? o.Histories.OrderBy(e => e.CreatedOn).Select(e => new OrderModelHistory
                    {
                        ColorCode = e.Status.ColorCode,
                        Date = e.CreatedOn.ToString(DefaultDateFormat.ddMMyyyyhhmmsstt),
                        Icon = e.Status.Icon,
                        FullDate = o.CreatedOn.ToString("dddd, dnn MMMM yyyy", useExtendedSpecifiers: true),
                        NotificationOption = e.NotificationOption.ToString(),
                        OrderId = e.OrderId,
                        Comment = e.Comment,
                        StatusId = e.StatusId,
                        Id = e.Id,
                        Label = e.Status.Label

                    }).ToList() : new List<OrderModelHistory>()

                }).ToList() :

               new List<OrderModel>();
            }
            catch (Exception)
            {

                throw;
            }

        }
        private string GetRelativeImageUrl(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return AssetDefault.DefaultImage;

            // Normalize slashes first
            path = path.Replace("\\", "/");

            // If already relative
            if (path.StartsWith("/images/", StringComparison.OrdinalIgnoreCase))
                return path;

            // If full URL
            if (Uri.TryCreate(path, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                return uri.AbsolutePath;
            }

            // Find /wwwroot/images
            var marker = "/wwwroot/images/";
            var idx = path.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

            if (idx >= 0)
                return path.Substring(idx + "/wwwroot".Length);

            return AssetDefault.DefaultImage;
        }




        private async Task<OrderResponse> OrderData(OrderFilterData filter)
        {
            try
            {
                if (await _context.Orders.AnyAsync())
                {
                    ExpressionStarter<Order> pred = ApplyFilter(@filter);
                    List<Order> orders = await _context.Orders
                        .Where(pred)
                        .OrderByDescending(e => e.CreatedOn)
                        .Skip(filter.Skip)
                        .Take(filter.PageSize)
                        .Include(c => c.Customer).ThenInclude(e => e.User)
                        .Include(e => e.Histories).ThenInclude(e => e.Status)
                        .Include(c => c.Country).ThenInclude(f => f.Flag)
                        .Include(st => st.State)
                        .Include(ct => ct.City)
                        .Include(st => st.Status)
                        .Include(im => im.Items).ThenInclude(im => im.WallArtSize)
                        .Include(im => im.Items).ThenInclude(im => im.Size)
                        .Include(im => im.Items).ThenInclude(im => im.MixedTemplateSize)
                        .Include(im => im.Items).ThenInclude(im => im.KidsTemplateSize)
                        .Include(im => im.Items).ThenInclude(im => im.RegularTemplateSize)
                        .Include(im => im.Items).ThenInclude(im => im.ChristmasTemplateSize)
                        .Include(im => im.Items).ThenInclude(im => im.Images).ThenInclude(e => e.WallArtImage).ToListAsync();





                    List<OrderSummary> totalOrders = await _context.Orders.Where(pred).Select(e => new
                    OrderSummary
                    {
                        Delivery = e.DeliveryFee,
                        VAT = e.VAT,
                        GrandTotal = e.GrandTotal,
                        SubTotal = e.SubTotal

                    })
                   .ToListAsync();


                    List<OrderSummary> sums = totalOrders.GroupBy(e => true).Select(s => new OrderSummary
                    {
                        VAT = s.Sum(e => e.VAT),
                        Delivery = s.Sum(d => d.Delivery),
                        GrandTotal = s.Sum(g => g.GrandTotal),
                        SubTotal = s.Sum(sb => sb.SubTotal)
                    }).ToList();







                    return new OrderResponse
                    {
                        VAT = sums.Any() ? sums.First().VAT : 0m,
                        Delivery = sums.Any() ? sums.First().Delivery : 0m,
                        GrandTotal = sums.Any() ? sums.First().GrandTotal : 0m,
                        Orders = FormatOrder(orders),
                        SubTotal = sums.Any() ? sums.First().SubTotal : 0m,
                        TotalOrder = totalOrders.Count

                    };







                }


                return new()
                {
                    VAT = 0m,
                    Delivery = 0m,
                    GrandTotal = 0m,
                    Orders = new List<OrderModel>(),
                    SubTotal = 0m
                };
            }
            catch (Exception)
            {

                throw;
            }
        }
        private async Task<OrderResponse> OrderData(Pagination pagination)
        {
            try
            {
                if (await _context.Orders.AnyAsync())
                {

                    List<Order> orders = await _context.Orders
                        .OrderByDescending(e => e.CreatedOn)
                        .Skip(pagination.Skip).Take(pagination.PageSize)
                        .Include(c => c.Customer).ThenInclude(e => e.User)
                        .Include(e => e.Histories).ThenInclude(e => e.Status)
                        .Include(c => c.Country).ThenInclude(f => f.Flag)
                        .Include(st => st.State)
                        .Include(ct => ct.City)
                        .Include(st => st.Status)
                        .Include(im => im.Items).ThenInclude(im => im.WallArtSize)
                        .Include(im => im.Items).ThenInclude(im => im.Size)
                        .Include(im => im.Items).ThenInclude(im => im.MixedTemplateSize)
                        .Include(im => im.Items).ThenInclude(im => im.KidsTemplateSize)
                        .Include(im => im.Items).ThenInclude(im => im.RegularTemplateSize)
                        .Include(im => im.Items).ThenInclude(im => im.ChristmasTemplateSize)
                        .Include(im => im.Items).ThenInclude(im => im.Images).ThenInclude(e => e.WallArtImage).ToListAsync();




                    if (orders.Any(e => e.Seen == false))
                    {
                        List<Order> NewOrders = orders.Where(e => e.Seen == false).ToList();
                        if (NewOrders.Any())
                        {
                            foreach (Order order in NewOrders)
                            {
                                order.Seen = true;
                            }

                            _context.Orders.UpdateRange(NewOrders);
                            await _context.SaveChangesAsync();
                        }

                    }


                    List<OrderSummary> sums = (await _context.Orders.Select(e => new
                    OrderSummary
                    {
                        Delivery = e.DeliveryFee,
                        VAT = e.VAT,
                        GrandTotal = e.GrandTotal,
                        SubTotal = e.SubTotal

                    })
                   .ToListAsync()).GroupBy(e => true).Select(s => new OrderSummary
                   {
                       VAT = s.Sum(e => e.VAT),
                       Delivery = s.Sum(d => d.Delivery),
                       GrandTotal = s.Sum(g => g.GrandTotal),
                       SubTotal = s.Sum(sb => sb.SubTotal)
                   }).ToList();






                    return new OrderResponse
                    {
                        VAT = sums.Any() ? sums.First().VAT : 0m,
                        Delivery = sums.Any() ? sums.First().Delivery : 0m,
                        GrandTotal = sums.Any() ? sums.First().GrandTotal : 0m,
                        Orders = FormatOrder(orders),
                        SubTotal = sums.Any() ? sums.First().SubTotal : 0m,
                        TotalOrder = await _context.Orders.CountAsync(),

                    };







                }


                return new()
                {
                    VAT = 0m,
                    Delivery = 0m,
                    GrandTotal = 0m,
                    Orders = new List<OrderModel>(),
                    SubTotal = 0m
                };
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<OrderInitResponse> Init(Pagination pagination)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@pagination", pagination);


                OrderResponse order = await OrderData(@pagination);
                List<BaseOption> cities = await _context.Cities.OrderBy(e => e.CityName).Select(e => new BaseOption
                {
                    Id = e.Id,
                    Name = e.CityName
                }).ToListAsync();

                List<BaseOption> states = await _context.States.OrderBy(e => e.StateName).Select(e => new BaseOption
                {
                    Id = e.Id,
                    Name = e.StateName
                }).ToListAsync();
                List<CountryOption> countries = await _context.Countries.Include(f => f.Flag).OrderBy(e => e.Name).Select(e => new CountryOption
                {
                    Id = e.Id,
                    Name = e.Name,
                    Flag = e.Flag.Name
                }).ToListAsync();

                List<OrderModelStatusBase> orderStatuses = await _context.OrderStatuses.OrderByDescending(e => e.IsDefault).ThenBy(e => e.CreatedOn).Select(e => new OrderModelStatusBase
                {
                    Id = e.Id,
                    Label = e.Label,
                    IsDefault = e.IsDefault,
                    ColorCode = e.ColorCode,
                    Icon = e.Icon
                }).ToListAsync();

                List<BaseOption> customers = await _context.Customers.Include(e => e.User).Select(u => new BaseOption
                {
                    Id = u.User.Id,
                    Name = u.User.FullName

                }).ToListAsync();

                List<BaseOption> orderStates = new List<BaseOption>
                        {
                            new BaseOption
                            {
                                Id = string.Empty,
                                Name = OrderState.Open.ToString()
                            },
                            new BaseOption
                            {
                                 Id = string.Empty,
                                Name = OrderState.Closed.ToString()
                            }
                        };

                LeadTimeModel leadTime = await _context.LeadTimes.Select(e => new LeadTimeModel
                {
                    Id = e.Id,
                    UpperBandQuantifier = e.UpperBandQuantifier,
                    LowerBandQuantifier = e.LowerBandQuantifier,
                    Description = e.Description,
                    TimeLimit = e.TimeLimit.ToString()

                }).FirstOrDefaultAsync();

                decimal newOrderCount = decimal.Round(await _context.Orders.Where(e => e.Seen == false).CountAsync(), 0, MidpointRounding.AwayFromZero);
                DefNotificationModel defNotification = await _context.DefNotifications.AnyAsync() ? await _context.DefNotifications.Select(d => new DefNotificationModel
                {
                    Id = d.Id,
                    Message = d.Message,
                    Option = d.NotificationOption.ToString()

                }).FirstOrDefaultAsync()

                         : new DefNotificationModel
                         {
                             Id = null,
                             Message = null,
                             Option = null
                         };


                return new OrderInitResponse
                {
                    Order = order,
                    Cities = cities,
                    States = states,
                    Countries = countries,

                    OrderStatuses = orderStatuses,
                    Customers = customers,
                    OrderStates = orderStates,

                    LeadTime = leadTime,
                    NewOrderCount = newOrderCount,
                    DefNotification = defNotification

                };

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<OrderResponse> Orders(OrderFilterData filter)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@filter", filter);

                bool emptyFilter = EmptyFilter(filter);
                if (emptyFilter)
                {
                    OrderResponse data = await OrderData(new Pagination { PageSize = filter.PageSize, Skip = filter.Skip });
                    data.Orders = ApplySort(data.Orders, filter);

                    return data;
                }


                OrderResponse orderData = await OrderData(filter);
                orderData.Orders = ApplySort(orderData.Orders, filter);
                return orderData;




            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<CustomerOrder> Customer(Pagination pagination)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@pagination", pagination);

                string userId = _currentUserService.GetUserId();
                if (await _context.Orders.AnyAsync(e => e.UserId == userId))
                {
                    return new CustomerOrder
                    {
                        Closed = new OrderGroup
                        {
                            Count = decimal.Round(await _context.Orders.Where(e => e.UserId == userId && (e.OrderState == OrderState.Closed)).CountAsync(), 0, MidpointRounding.AwayFromZero),
                            Orders = FormatOrder(
                                await _context.Orders
                        .Where(e => e.UserId == userId && (e.OrderState == OrderState.Closed))
                        .OrderByDescending(e => e.CreatedOn)
                        .Skip(@pagination.Skip)
                        .Take(@pagination.PageSize)
                        .Include(c => c.Customer).ThenInclude(e => e.User)
                        .Include(e => e.Histories).ThenInclude(e => e.Status)
                        .Include(c => c.Country).ThenInclude(f => f.Flag)
                        .Include(st => st.State)
                        .Include(ct => ct.City)
                        .Include(st => st.Status)
                        .Include(im => im.Items).ThenInclude(im => im.WallArtSize)
                        .Include(im => im.Items).ThenInclude(im => im.Size)
                        .Include(im => im.Items).ThenInclude(im => im.MixedTemplateSize)
                        .Include(im => im.Items).ThenInclude(im => im.KidsTemplateSize)
                         .Include(im => im.Items).ThenInclude(im => im.ChristmasTemplateSize)
                        .Include(im => im.Items).ThenInclude(im => im.RegularTemplateSize)
                        .Include(im => im.Items).ThenInclude(im => im.Images).ThenInclude(e => e.WallArtImage).ToListAsync())
                        },
                        Open = new OrderGroup
                        {
                            Count = decimal.Round(await _context.Orders.Where(e => e.UserId == userId && (e.OrderState == OrderState.Open)).CountAsync(), 0, MidpointRounding.AwayFromZero),
                            Orders = FormatOrder(
                                await _context.Orders
                        .Where(e => e.UserId == userId && (e.OrderState == OrderState.Open))
                        .OrderByDescending(e => e.CreatedOn)
                        .Skip(@pagination.Skip)
                        .Take(@pagination.PageSize)
                        .Include(c => c.Customer).ThenInclude(e => e.User)
                        .Include(e => e.Histories).ThenInclude(e => e.Status)
                        .Include(c => c.Country).ThenInclude(f => f.Flag)
                        .Include(st => st.State)
                        .Include(ct => ct.City)
                        .Include(st => st.Status)
                        .Include(im => im.Items).ThenInclude(im => im.WallArtSize)
                        .Include(im => im.Items).ThenInclude(im => im.Size)
                        .Include(im => im.Items).ThenInclude(im => im.MixedTemplateSize)
                        .Include(im => im.Items).ThenInclude(im => im.KidsTemplateSize)
                         .Include(im => im.Items).ThenInclude(im => im.ChristmasTemplateSize)
                        .Include(im => im.Items).ThenInclude(im => im.RegularTemplateSize)
                        .Include(im => im.Items).ThenInclude(im => im.Images).ThenInclude(e => e.WallArtImage).ToListAsync())
                        }
                    };
                }


                return new CustomerOrder
                {
                    Closed = new OrderGroup
                    {
                        Count = 0,
                        Orders = new List<OrderModel>()
                    },

                    Open = new OrderGroup
                    {
                        Count = 0,
                        Orders = new List<OrderModel>()
                    }
                };


            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<OrderGroup> Open(SearchFilter search)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@search", search);
                string userId = _currentUserService.GetUserId();

                return string.IsNullOrEmpty(search.Search) ? new OrderGroup
                {
                    Count = await _context.Orders.Where(e => e.UserId == userId && (e.OrderState == OrderState.Open)).CountAsync(),
                    Orders = FormatOrder(
                                await _context.Orders
                        .Where(e => e.UserId == userId && (e.OrderState == OrderState.Open))
                        .OrderByDescending(e => e.CreatedOn)
                        .Skip(@search.Skip)
                        .Take(@search.PageSize)
                        .Include(c => c.Customer).ThenInclude(e => e.User)
                        .Include(e => e.Histories).ThenInclude(e => e.Status)
                        .Include(c => c.Country).ThenInclude(f => f.Flag)
                        .Include(st => st.State)
                        .Include(ct => ct.City)
                        .Include(st => st.Status)
                        .Include(im => im.Items).ThenInclude(im => im.WallArtSize)
                        .Include(im => im.Items).ThenInclude(im => im.Size)
                        .Include(im => im.Items).ThenInclude(im => im.MixedTemplateSize)
                        .Include(im => im.Items).ThenInclude(im => im.KidsTemplateSize)
                        .Include(im => im.Items).ThenInclude(im => im.ChristmasTemplateSize)
                        .Include(im => im.Items).ThenInclude(im => im.RegularTemplateSize)
                        .Include(im => im.Items).ThenInclude(im => im.Images).ThenInclude(e => e.WallArtImage).ToListAsync())
                }
                : new OrderGroup
                {
                    Orders = FormatOrder(await _context.Orders
                      .Where(e => e.UserId == userId && (e.OrderState == OrderState.Open) && (e.InvoiceNumber.Contains(@search.Search)))
                      .OrderByDescending(e => e.CreatedOn)
                      .Skip(@search.Skip)
                      .Take(@search.PageSize)
                      .Include(c => c.Customer).ThenInclude(e => e.User)
                      .Include(e => e.Histories).ThenInclude(e => e.Status)
                      .Include(c => c.Country).ThenInclude(f => f.Flag)
                      .Include(st => st.State)
                      .Include(ct => ct.City)
                      .Include(st => st.Status)
                      .Include(im => im.Items).ThenInclude(im => im.WallArtSize)
                      .Include(im => im.Items).ThenInclude(im => im.Size)
                      .Include(im => im.Items).ThenInclude(im => im.MixedTemplateSize)
                      .Include(im => im.Items).ThenInclude(im => im.KidsTemplateSize)
                      .Include(im => im.Items).ThenInclude(im => im.ChristmasTemplateSize)
                      .Include(im => im.Items).ThenInclude(im => im.RegularTemplateSize)
                      .Include(im => im.Items).ThenInclude(im => im.Images).ThenInclude(e => e.WallArtImage).ToListAsync()),
                    Count = decimal.Round(await _context.Orders.Where(e => e.UserId == userId && (e.OrderState == OrderState.Open) && (e.InvoiceNumber.Contains(@search.Search))).CountAsync(), 0, MidpointRounding.AwayFromZero),
                }
                ;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<OrderGroup> Closed(SearchFilter search)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@search", search);
                string userId = _currentUserService.GetUserId();

                return string.IsNullOrEmpty(@search.Search) ? new OrderGroup
                {
                    Count = decimal.Round(await _context.Orders.Where(e => e.UserId == userId && (e.OrderState == OrderState.Closed)).CountAsync(), 0, MidpointRounding.AwayFromZero),
                    Orders = FormatOrder(
                               await _context.Orders
                       .Where(e => e.UserId == userId && (e.OrderState == OrderState.Closed))
                       .OrderByDescending(e => e.CreatedOn)
                       .Skip(@search.Skip)
                       .Take(@search.PageSize)
                       .Include(c => c.Customer).ThenInclude(e => e.User)
                       .Include(e => e.Histories).ThenInclude(e => e.Status)
                       .Include(c => c.Country).ThenInclude(f => f.Flag)
                       .Include(st => st.State)
                       .Include(ct => ct.City)
                       .Include(st => st.Status)
                       .Include(im => im.Items).ThenInclude(im => im.WallArtSize)
                       .Include(im => im.Items).ThenInclude(im => im.Size)
                       .Include(im => im.Items).ThenInclude(im => im.MixedTemplateSize)
                       .Include(im => im.Items).ThenInclude(im => im.KidsTemplateSize)
                       .Include(im => im.Items).ThenInclude(im => im.ChristmasTemplateSize)
                       .Include(im => im.Items).ThenInclude(im => im.RegularTemplateSize)
                       .Include(im => im.Items).ThenInclude(im => im.Images).ThenInclude(e => e.WallArtImage).ToListAsync())
                }
                :
                new OrderGroup
                {
                    Orders = FormatOrder(await _context.Orders
                      .Where(e => e.UserId == userId && (e.OrderState == OrderState.Closed) && (e.InvoiceNumber.Contains(@search.Search)))
                      .OrderByDescending(e => e.CreatedOn)
                      .Skip(@search.Skip)
                      .Take(@search.PageSize)
                      .Include(c => c.Customer).ThenInclude(e => e.User)
                      .Include(e => e.Histories).ThenInclude(e => e.Status)
                      .Include(c => c.Country).ThenInclude(f => f.Flag)
                      .Include(st => st.State)
                      .Include(ct => ct.City)
                      .Include(st => st.Status)
                      .Include(im => im.Items).ThenInclude(im => im.WallArtSize)
                      .Include(im => im.Items).ThenInclude(im => im.Size)
                      .Include(im => im.Items).ThenInclude(im => im.MixedTemplateSize)
                      .Include(im => im.Items).ThenInclude(im => im.KidsTemplateSize)
                      .Include(im => im.Items).ThenInclude(im => im.ChristmasTemplateSize)
                      .Include(im => im.Items).ThenInclude(im => im.RegularTemplateSize)
                      .Include(im => im.Items).ThenInclude(im => im.Images).ThenInclude(e => e.WallArtImage).ToListAsync()),
                    Count = decimal.Round(await _context.Orders.Where(e => e.UserId == userId && (e.OrderState == OrderState.Closed) && (e.InvoiceNumber.Contains(@search.Search))).CountAsync(), 0, MidpointRounding.AwayFromZero),

                }
                ;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<OrderGroup> SearchOpen(SearchFilter request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                string userId = _currentUserService.GetUserId();
                if (await _context.Orders.AnyAsync(e => e.UserId == userId))
                {
                    if (string.IsNullOrEmpty(request.Search))
                    {
                        return new OrderGroup
                        {
                            Count = decimal.Round(await _context.Orders.Where(e => e.UserId == userId && (e.OrderState == OrderState.Open)).CountAsync(), 0, MidpointRounding.AwayFromZero),
                            Orders = FormatOrder(
                                    await _context.Orders
                            .Where(e => e.UserId == userId && (e.OrderState == OrderState.Open))
                            .OrderByDescending(e => e.CreatedOn)
                            .Skip(@request.Skip)
                            .Take(request.PageSize)
                            .Include(c => c.Customer).ThenInclude(e => e.User)
                            .Include(e => e.Histories).ThenInclude(e => e.Status)
                            .Include(c => c.Country).ThenInclude(f => f.Flag)
                            .Include(st => st.State)
                            .Include(ct => ct.City)
                            .Include(st => st.Status)
                            .Include(im => im.Items).ThenInclude(im => im.WallArtSize)
                            .Include(im => im.Items).ThenInclude(im => im.Size)
                            .Include(im => im.Items).ThenInclude(im => im.MixedTemplateSize)
                            .Include(im => im.Items).ThenInclude(im => im.KidsTemplateSize)
                            .Include(im => im.Items).ThenInclude(im => im.ChristmasTemplateSize)
                            .Include(im => im.Items).ThenInclude(im => im.RegularTemplateSize)
                            .Include(im => im.Items).ThenInclude(im => im.Images).ThenInclude(e => e.WallArtImage).ToListAsync())
                        };
                    }


                    return new OrderGroup
                    {
                        Orders = FormatOrder(await _context.Orders
                      .Where(e => e.UserId == userId && (e.OrderState == OrderState.Open) && (e.InvoiceNumber.Contains(@request.Search)))
                      .OrderByDescending(e => e.CreatedOn)
                      .Skip(@request.Skip)
                      .Take(@request.PageSize)
                      .Include(c => c.Customer).ThenInclude(e => e.User)
                      .Include(e => e.Histories).ThenInclude(e => e.Status)
                      .Include(c => c.Country).ThenInclude(f => f.Flag)
                      .Include(st => st.State)
                      .Include(ct => ct.City)
                      .Include(st => st.Status)
                      .Include(im => im.Items).ThenInclude(im => im.WallArtSize)
                      .Include(im => im.Items).ThenInclude(im => im.Size)
                      .Include(im => im.Items).ThenInclude(im => im.MixedTemplateSize)
                      .Include(im => im.Items).ThenInclude(im => im.KidsTemplateSize)
                      .Include(im => im.Items).ThenInclude(im => im.ChristmasTemplateSize)
                      .Include(im => im.Items).ThenInclude(im => im.RegularTemplateSize)
                      .Include(im => im.Items).ThenInclude(im => im.Images).ThenInclude(e => e.WallArtImage).ToListAsync()),
                        Count = decimal.Round(await _context.Orders.Where(e => e.UserId == userId && (e.OrderState == OrderState.Open) && (e.InvoiceNumber.Contains(@request.Search))).CountAsync(), 0, MidpointRounding.AwayFromZero),



                    };


                }






                return new OrderGroup
                {
                    Count = 0,
                    Orders = new List<OrderModel>()
                };


            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<OrderGroup> SearchClosed(SearchFilter request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                string userId = _currentUserService.GetUserId();
                if (await _context.Orders.AnyAsync(e => e.UserId == userId))
                {
                    if (string.IsNullOrEmpty(request.Search))
                    {
                        return new OrderGroup
                        {
                            Count = decimal.Round(await _context.Orders.Where(e => e.UserId == userId && (e.OrderState == OrderState.Closed)).CountAsync(), 0, MidpointRounding.AwayFromZero),
                            Orders = FormatOrder(
                                    await _context.Orders
                            .Where(e => e.UserId == userId && (e.OrderState == OrderState.Closed))
                            .OrderByDescending(e => e.CreatedOn)
                            .Skip(@request.Skip)
                            .Take(request.PageSize)
                            .Include(c => c.Customer).ThenInclude(e => e.User)
                            .Include(e => e.Histories).ThenInclude(e => e.Status)
                            .Include(c => c.Country).ThenInclude(f => f.Flag)
                            .Include(st => st.State)
                            .Include(ct => ct.City)
                            .Include(st => st.Status)
                            .Include(im => im.Items).ThenInclude(im => im.WallArtSize)
                            .Include(im => im.Items).ThenInclude(im => im.Size)
                            .Include(im => im.Items).ThenInclude(im => im.MixedTemplateSize)
                            .Include(im => im.Items).ThenInclude(im => im.KidsTemplateSize)
                            .Include(im => im.Items).ThenInclude(im => im.ChristmasTemplateSize)
                            .Include(im => im.Items).ThenInclude(im => im.RegularTemplateSize)
                            .Include(im => im.Items).ThenInclude(im => im.Images).ThenInclude(e => e.WallArtImage).ToListAsync())
                        };
                    }


                    return new OrderGroup
                    {
                        Orders = FormatOrder(await _context.Orders
                      .Where(e => e.UserId == userId && (e.OrderState == OrderState.Closed) && (e.InvoiceNumber.Contains(@request.Search)))
                      .OrderByDescending(e => e.CreatedOn)
                      .Skip(@request.Skip)
                      .Take(@request.PageSize)
                      .Include(c => c.Customer).ThenInclude(e => e.User)
                      .Include(e => e.Histories).ThenInclude(e => e.Status)
                      .Include(c => c.Country).ThenInclude(f => f.Flag)
                      .Include(st => st.State)
                      .Include(ct => ct.City)
                      .Include(st => st.Status)
                      .Include(im => im.Items).ThenInclude(im => im.WallArtSize)
                      .Include(im => im.Items).ThenInclude(im => im.Size)
                      .Include(im => im.Items).ThenInclude(im => im.MixedTemplateSize)
                      .Include(im => im.Items).ThenInclude(im => im.KidsTemplateSize)
                      .Include(im => im.Items).ThenInclude(im => im.ChristmasTemplateSize)
                      .Include(im => im.Items).ThenInclude(im => im.RegularTemplateSize)
                      .Include(im => im.Items).ThenInclude(im => im.Images).ThenInclude(e => e.WallArtImage).ToListAsync()),
                        Count = decimal.Round(await _context.Orders.Where(e => e.UserId == userId && (e.OrderState == OrderState.Closed) && (e.InvoiceNumber.Contains(@request.Search))).CountAsync(), 0, MidpointRounding.AwayFromZero),



                    };


                }






                return new OrderGroup
                {
                    Count = 0,
                    Orders = new List<OrderModel>()
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<BaseResponse> Pay(string id)
        {
            try
            {
                if (await _context.Orders.AnyAsync(e => e.Id == id))
                {
                    Order order = await _context.Orders.Where(e => e.Id == id).FirstOrDefaultAsync();
                    if (order != null)
                    {
                        order.PaymentStatus = PaymentStatus.Paid;
                        _context.Orders.Update(order);

                        int updateState = await _context.SaveChangesAsync();

                        if (updateState > 0)
                        {
                            return new BaseResponse
                            {
                                Message = "Order status updated!",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            };
                        }

                        return new BaseResponse
                        {
                            Message = "An error occurred. Payment status could not be updated",
                            Result = RequestResult.Error,
                            Succeeded = false,
                            Title = "Unknown Error"
                        };

                    }
                }

                return new BaseResponse
                {
                    Message = "Reference to order could not be found.",
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = "Order not found"
                };
            }
            catch (Exception ex)
            {

                return new BaseResponse
                {
                    Message = ex.Message,
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = ex.Source
                };
            }
        }


        private async Task<OrderResponse> OrderDataNew(Pagination pagination)
        {
            try
            {
                var orders = await _context.Orders.OrderByDescending(e => e.CreatedOn).Skip(pagination.Skip).Take(pagination.PageSize).ToListAsync();

                List<OrderNew> result = new();

                foreach (var o in orders)
                {
                    var customer = await _context.Customers.FindAsync(o.CustomerId);
                    if (customer != null)
                    {
                        var users = await _context.Users.FindAsync(customer.UserId);
                    }

                    var user = await _context.Users.FindAsync(o.UserId);
                    var status = await _context.OrderStatuses.FindAsync(o.StatusId);
                    var city = await _context.Cities.FindAsync(o.CityId);
                    var state = await _context.States.FindAsync(o.StateId);
                    var country = await _context.Countries.FindAsync(o.CountryId);
                    var flag = await _context.Flags.FindAsync(country.FlagId);
                    var items = await _context.OrderItems.Where(i => i.OrderId == o.Id).ToListAsync();
                    var itemNew = await _context.OrderCartItems.Where(i => i.OrderId == o.Id).ToListAsync();
                    var histories = await _context.OrderStatusHistories.Where(h => h.OrderId == o.Id).ToListAsync();

                    foreach (var h in histories)
                    {
                        h.Status = await _context.OrderStatuses.FindAsync(h.StatusId);
                    }

                    result.Add(new OrderNew
                    {
                        Id = o.Id,
                        CreatedOn = o.CreatedOn,
                        AdditionalPhoneNumber = o.AdditionalPhoneNumber,
                        EmailAddress = o.EmailAddress,
                        MobilePhoneNumber = o.MobilePhoneNumber,
                        CustomerId = o.CustomerId,
                        Customer = customer,
                        CustomerName = o.CustomerName,
                        UserId = o.UserId,
                        CityName = o.CityName,
                        User = user,
                        Name = o.Name,
                        StatusId = o.StatusId,
                        Status = status,
                        PaymentStatus = o.PaymentStatus,
                        OrderState = o.OrderState,
                        SubTotal = o.SubTotal,
                        DeliveryFee = o.DeliveryFee,
                        VAT = o.VAT,
                        ShippingAddress = o.ShippingAddress,
                        ZipCode = o.ZipCode,
                        CityId = o.CityId,
                        City = city,

                        StateId = o.StateId,
                        State = state,
                        CountryId = o.CountryId,
                        Country = country,
                        CountryName = country?.Name,
                        SpeculativeDeliveryDate = o.SpeculativeDeliveryDate,
                        DeliveryDate = o.DeliveryDate,
                        InvoiceNumber = o.InvoiceNumber,
                        GrandTotal = o.GrandTotal,
                        Seen = o.Seen,
                        PaymentIntentId = o.PaymentIntentId,
                        Items = items,
                        ItemsNew = itemNew,
                        Histories = histories
                    });
                }
                if (orders.Any(e => e.Seen == false))
                {
                    var unseenOrders = await _context.Orders.Where(e => e.Seen == false).ToListAsync();

                    if (unseenOrders.Any())
                    {
                        foreach (var order in unseenOrders)
                        {
                            order.Seen = true;
                        }
                        _context.Orders.UpdateRange(unseenOrders);
                        await _context.SaveChangesAsync();
                    }
                }

                List<OrderSummary> sums = (await _context.Orders.Select(e => new
                OrderSummary
                {
                    Delivery = e.DeliveryFee,
                    VAT = e.VAT,
                    GrandTotal = e.GrandTotal,
                    SubTotal = e.SubTotal

                }).ToListAsync()).GroupBy(e => true).Select(s => new OrderSummary
                {
                    VAT = s.Sum(e => e.VAT),
                    Delivery = s.Sum(d => d.Delivery),
                    GrandTotal = s.Sum(g => g.GrandTotal),
                    SubTotal = s.Sum(sb => sb.SubTotal)
                }).ToList();

                return new OrderResponse
                {
                    VAT = sums.Any() ? sums.First().VAT : 0m,
                    Delivery = sums.Any() ? sums.First().Delivery : 0m,
                    GrandTotal = sums.Any() ? sums.First().GrandTotal : 0m,
                    Orders = FormatOrderNew(result),
                    SubTotal = sums.Any() ? sums.First().SubTotal : 0m,
                    TotalOrder = await _context.Orders.CountAsync(),

                };
                return new()
                {
                    VAT = 0m,
                    Delivery = 0m,
                    GrandTotal = 0m,
                    Orders = new List<OrderModel>(),
                    SubTotal = 0m
                };
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<OrderInitResponse> InitNew(Pagination pagination)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@pagination", pagination);


                OrderResponse order = await OrderDataNew(@pagination);
                List<BaseOption> cities = await _context.Cities.OrderBy(e => e.CityName).Select(e => new BaseOption
                {
                    Id = e.Id,
                    Name = e.CityName
                }).ToListAsync();

                List<BaseOption> states = await _context.States.OrderBy(e => e.StateName).Select(e => new BaseOption
                {
                    Id = e.Id,
                    Name = e.StateName
                }).ToListAsync();
                List<CountryOption> countries = await _context.Countries.Include(f => f.Flag).OrderBy(e => e.Name).Select(e => new CountryOption
                {
                    Id = e.Id,
                    Name = e.Name,
                    Flag = e.Flag.Name
                }).ToListAsync();

                List<OrderModelStatusBase> orderStatuses = await _context.OrderStatuses.OrderByDescending(e => e.IsDefault).ThenBy(e => e.CreatedOn).Select(e => new OrderModelStatusBase
                {
                    Id = e.Id,
                    Label = e.Label,
                    IsDefault = e.IsDefault,
                    ColorCode = e.ColorCode,
                    Icon = e.Icon
                }).ToListAsync();

                List<BaseOption> customers = await _context.Customers.Include(e => e.User).Select(u => new BaseOption
                {
                    Id = u.User.Id,
                    Name = u.User.FullName

                }).ToListAsync();

                List<BaseOption> orderStates = new List<BaseOption>
                        {
                            new BaseOption
                            {
                                Id = string.Empty,
                                Name = OrderState.Open.ToString()
                            },
                            new BaseOption
                            {
                                 Id = string.Empty,
                                Name = OrderState.Closed.ToString()
                            }
                        };

                LeadTimeModel leadTime = await _context.LeadTimes.Select(e => new LeadTimeModel
                {
                    Id = e.Id,
                    UpperBandQuantifier = e.UpperBandQuantifier,
                    LowerBandQuantifier = e.LowerBandQuantifier,
                    Description = e.Description,
                    TimeLimit = e.TimeLimit.ToString()

                }).FirstOrDefaultAsync();

                decimal newOrderCount = decimal.Round(await _context.Orders.Where(e => e.Seen == false).CountAsync(), 0, MidpointRounding.AwayFromZero);
                DefNotificationModel defNotification = await _context.DefNotifications.AnyAsync() ? await _context.DefNotifications.Select(d => new DefNotificationModel
                {
                    Id = d.Id,
                    Message = d.Message,
                    Option = d.NotificationOption.ToString()

                }).FirstOrDefaultAsync()

                         : new DefNotificationModel
                         {
                             Id = null,
                             Message = null,
                             Option = null
                         };


                return new OrderInitResponse
                {
                    Order = order,
                    Cities = cities,
                    States = states,
                    Countries = countries,

                    OrderStatuses = orderStatuses,
                    Customers = customers,
                    OrderStates = orderStates,

                    LeadTime = leadTime,
                    NewOrderCount = newOrderCount,
                    DefNotification = defNotification

                };

            }
            catch (Exception)
            {

                throw;
            }
        }


        private List<OrderModel> FormatOrderNew(List<OrderNew> orders)
        {
            try
            {
                if (orders == null || !orders.Any())
                    return new List<OrderModel>();

                var allImages = _context.OrderCartItemImages.ToList();

                var allSize = _context.Sizes.ToList();

                var formattedOrders = orders.Select(o => new OrderModel
                {
                    Id = o.Id.ToString(),
                    SubTotal = o.SubTotal,
                    ShippingAddress = o.ShippingAddress,
                    City = string.IsNullOrEmpty(o.CityId) ? string.IsNullOrEmpty(o.CityName) ? new BaseOption { Id = string.Empty, Name = string.Empty }
                            : new BaseOption { Id = string.Empty, Name = o.CityName } : new BaseOption { Id = o.City?.Id ?? string.Empty, Name = o.City?.CityName ?? string.Empty },
                    State = string.IsNullOrEmpty(o.StateId) ? new BaseOption { Id = string.Empty, Name = string.Empty } : new BaseOption { Id = o.State?.Id ?? string.Empty, Name = o.State?.StateName ?? string.Empty },
                    Country = string.IsNullOrEmpty(o.CountryId) ? new CountryOption { Id = string.Empty, Name = string.Empty, Flag = string.Empty } : new CountryOption { Id = o.Country.Id, Name = o.Country.Name, Flag = o.Country.Flag.Name },
                    FullName = string.IsNullOrEmpty(o.CustomerName) ? o.Customer == null ? string.Empty : o.Customer.User.FullName : o.CustomerName,
                    CustomerId = o.CustomerId ?? string.Empty,
                    Name = o.Name,
                    InvoiceNumber = o.InvoiceNumber,
                    MobilePhoneNumber = o.MobilePhoneNumber,
                    AdditionalMobileNumber = o.AdditionalPhoneNumber,
                    FullDate = o.CreatedOn.ToString("dddd, dnn MMMM yyyy", useExtendedSpecifiers: true),
                    DeliveryDate = o.DeliveryDate.ToString("dddd, dnn MMMM yyyy", useExtendedSpecifiers: true),
                    SpecDeliveryDate = o.SpeculativeDeliveryDate.ToString("dddd, dnn MMMM yyyy", useExtendedSpecifiers: true),
                    ZipCode = o.ZipCode ?? string.Empty,
                    EmailAddress = o.EmailAddress,
                    OrderState = new BaseOption { Id = string.Empty, Name = o.OrderState.ToString() },
                    PaymentStatus = new BaseOption { Id = string.Empty, Name = o.PaymentStatus.ToString() },
                    Status = string.IsNullOrEmpty(o.StatusId)
                        ? new OrderModelStatusBase { Id = string.Empty, Label = string.Empty, ColorCode = string.Empty, Icon = string.Empty }
                        : new OrderModelStatusBase
                        {
                            Id = o.StatusId,
                            Label = o.Status.Label,
                            ColorCode = o.Status.ColorCode,
                            Icon = o.Status.Icon
                        },
                    Date = o.CreatedOn.ToString(DefaultDateFormat.ddMMyyyy),
                    VAT = decimal.Round(o.VAT, 2, MidpointRounding.AwayFromZero),
                    DeliveryFee = decimal.Round(o.DeliveryFee, 2, MidpointRounding.AwayFromZero),
                    GrandTotal = decimal.Round(o.GrandTotal, 2, MidpointRounding.AwayFromZero),

                    //ItemsNew = o.Items.Select(i =>
                    //{
                    //    var images = allImages
                    //        .Where(img => img.OrderCartItemId == i.id)
                    //        .Select(img => new ItemCardImage
                    //        {
                    //            CroppedImages = img.CroppedItemImage,
                    //            OriginalImages = img.OriginalItemImage
                    //        }).ToList();

                    //    return new CartItemNew
                    //    {
                    //        CategoryId = i.CategoryId,
                    //        TotalAmountPerCategory = i.TotalAmountPerCategory,
                    //        Frames = new List<FrameSet>
                    //{
                    //    new FrameSet
                    //    {
                    //        FrameClass = i.FrameClass,
                    //        Quantity = i.Quantity,
                    //        Amount = i.TotalAmountPerCategory,
                    //        FrameId = string.IsNullOrEmpty(i.FrameId) ? (int?)null : int.Parse(i.FrameId),
                    //        FrameSize = i.FrameSize,
                    //        templateConfigId = i.TemplateConfigId,
                    //        PreviewImage = i.PreviewImage,
                    //        LineColor = i.LineColor,
                    //        UniqueItemId = i.UniqueItemId,
                    //        Images = images
                    //    }
                    //}
                    //    };
                    //}).ToList(),
                    Items = o.Items.Select(i => new OrderModelItem
                    {
                        Amount = i.TotalAmount,

                        Category = new BaseOption { Id = string.Empty, Name = i.Category.ToString() },
                        FrameClass = i.FrameClass,
                        ImageURL = GetRelativeImageUrl(i.ImageURL),
                        PreviewImageURL = GetRelativeImageUrl(i.PreviewImageURL),
                        CroppedImageURL = GetRelativeImageUrl(i.CroppedImageURL),
                        Description = i.Description,
                        Heading = i.Heading,
                        Id = i.Id,
                        ItemName = i.ItemName,
                        Quantity = i.Quantity,
                        DownloadingOriginal = false,
                        DownloadingCropped = false,
                        Size = i.Type == OrderType.Regular ? string.IsNullOrEmpty(i.WallArtSizeId) ?
                string.IsNullOrEmpty(i.SizeId) ? new BaseOption { Id = string.Empty, Name = i.SizeName } : new BaseOption { Id = i.SizeId, Name = i.Size.Name }
                : new BaseOption { Id = i.WallArtSize.Id, Name = i.WallArtSize.Name }
               : i.Type == OrderType.MixedTemplate ? string.IsNullOrEmpty(i.MixedTemplateSizeId) ? new BaseOption { Id = string.Empty, Name = i.SizeName } : new BaseOption { Id = i.MixedTemplateSize.Id, Name = i.MixedTemplateSize.Name }
               : i.Type == OrderType.KidsTemplate ? string.IsNullOrEmpty(i.KidsTemplateSizeId) ? new BaseOption { Id = string.Empty, Name = i.SizeName } : new BaseOption { Id = i.KidsTemplateSize.Id, Name = i.KidsTemplateSize.Name }
               : i.Type == OrderType.UploadTemplate ? string.IsNullOrEmpty(i.RegularTemplateSizeId) ? new BaseOption { Id = string.Empty, Name = i.SizeName } : new BaseOption { Id = i.RegularTemplateSize.Id, Name = i.RegularTemplateSize.Name }
               : i.Type == OrderType.ChristmasTemplate ? string.IsNullOrEmpty(i.ChristmasTemplateSizeId) ? new BaseOption { Id = string.Empty, Name = i.SizeName } : new BaseOption { Id = i.ChristmasTemplateSize.Id, Name = i.ChristmasTemplateSize.Name }
               : new BaseOption { Id = string.Empty, Name = i.SizeName },
                        FrameSizeName = i.SizeName,
                        Images = i.Images.Any() ?
               i.Images.Select(im => new OrderModelItemImage
               {
                   Id = im.Id,
                   Amount = decimal.Round((i.TotalAmount / i.Images.Count()), 2, MidpointRounding.AwayFromZero),
                   Description = string.IsNullOrEmpty(im.WallArtImageId) ? i.Description : im.WallArtImage.Description,
                   FrameClass = i.FrameClass,
                   Downloading = false,
                   Name = string.IsNullOrEmpty(im.WallArtImageId) ? i.ItemName : im.WallArtImage.Name,
                   Quantity = i.Quantity,
                   Category = i.Type == OrderType.Regular ? "WallArt Image" : i.Type == OrderType.MixedTemplate ? "Mixed Template Image" : i.Type == OrderType.ChristmasTemplate ? "Christmas Image" : "Kids Template Image",
                   Size = i.Type == OrderType.Regular ? string.IsNullOrWhiteSpace(i.WallArtSizeId) ? string.Empty : i.WallArtSize.Name
                   : i.Type == OrderType.MixedTemplate ? string.IsNullOrWhiteSpace(i.MixedTemplateSizeId) ? string.Empty : i.MixedTemplateSize.Name
                   : i.Type == OrderType.KidsTemplate ? string.IsNullOrWhiteSpace(i.KidsTemplateSizeId) ? string.Empty : i.KidsTemplateSize.Name
                   : i.Type == OrderType.UploadTemplate ? string.IsNullOrWhiteSpace(i.RegularTemplateSizeId) ? string.Empty : i.RegularTemplateSize.Name
                   : i.Type == OrderType.ChristmasTemplate ? string.IsNullOrWhiteSpace(i.ChristmasTemplateSizeId) ? string.Empty : i.ChristmasTemplateSize.Name
                   : string.Empty,

                   ImageURL = GetRelativeImageUrl(im.ImageURL),
                   CroppedImageURL = GetRelativeImageUrl(im.CroppedImageURL)

               }).ToList() : new List<OrderModelItemImage>()


                    }).ToList(),
                    ItemsNew = o.ItemsNew.GroupBy(i => i.CategoryId)
                        .Select(g =>
                        {
                            var frames = g.Select(i =>
                            {
                                var images = allImages.Where(img => img.OrderCartItemId == i.id)
                                    .Select(img => new ItemCardImage
                                    {
                                        CroppedImages = img.CroppedItemImage,
                                        OriginalImages = img.OriginalItemImage
                                    })
                                    .ToList();
                                var sizeName = allSize.FirstOrDefault(s => s.Id == i.FrameSize)?.Name ?? "Unknown";
                                return new FrameSet
                                {
                                    FrameId = i.FrameId,
                                    templateConfigId = i.TemplateConfigId,
                                    FrameClass = i.FrameClass,
                                    LineColor = i.LineColor,
                                    PreviewImage = i.PreviewImage,
                                    UniqueItemId = i.UniqueItemId,
                                    Images = images,
                                    Amount = i.TotalAmountPerCategory,
                                    Quantity = i.Quantity,
                                    FrameSize = i.FrameSize,
                                    FrameSizeName = sizeName
                                };
                            }).ToList();

                            return new CartItemNew
                            {
                                CategoryId = g.Key,
                                TotalAmountPerCategory = g.Sum(x => x.TotalAmountPerCategory),
                                Frames = frames
                            };
                        })
                        .ToList(),

                    Histories = o.Histories.Any()
                        ? o.Histories.OrderBy(e => e.CreatedOn).Select(e => new OrderModelHistory
                        {
                            ColorCode = e.Status.ColorCode,
                            Date = e.CreatedOn.ToString(DefaultDateFormat.ddMMyyyyhhmmsstt),
                            Icon = e.Status.Icon,
                            FullDate = o.CreatedOn.ToString("dddd, dnn MMMM yyyy", useExtendedSpecifiers: true),
                            NotificationOption = e.NotificationOption.ToString(),
                            OrderId = e.OrderId,
                            Comment = e.Comment,
                            StatusId = e.StatusId,
                            Id = e.Id,
                            Label = e.Status.Label
                        }).ToList()
                        : new List<OrderModelHistory>()

                }).ToList();

                return formattedOrders;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
