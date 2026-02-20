

using api.artpixxel.data.Features.Checkouts;
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.LeadTimes;
using api.artpixxel.data.Features.OrderStatuses;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.Orders
{
    public class OrderModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string CustomerId { get; set; }
        public string ShippingAddress { get; set; }
        public decimal SubTotal { get; set; }
        public BaseOption State { get; set; }
        public string InvoiceNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string AdditionalMobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public string DeliveryDate { get; set; }
        public string Name { get; set; }
        public string SpecDeliveryDate { get; set; }
        public string ZipCode { get; set; }
        public BaseOption City { get; set; }
        public OrderModelStatusBase Status { get; set; }
        public CountryOption Country { get; set; }
        public BaseOption PaymentStatus { get; set; }
        public BaseOption OrderState { get; set; }
        public List<OrderModelItem> Items { get; set; }
        public List<CartItemNew> ItemsNew { get; set; }

        public List<OrderModelHistory> Histories { get; set; }
        public decimal VAT { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public string Date { get; set; }
        public string FullDate { get; set; }


    }

    public class OrderModelNew
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string CustomerId { get; set; }
        public string ShippingAddress { get; set; }
        public decimal SubTotal { get; set; }
        public BaseOption State { get; set; }
        public string InvoiceNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string AdditionalMobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public string DeliveryDate { get; set; }
        public string Name { get; set; }
        public string SpecDeliveryDate { get; set; }
        public string ZipCode { get; set; }
        public BaseOption City { get; set; }
        public OrderModelStatusBase Status { get; set; }
        public CountryOption Country { get; set; }
        public BaseOption PaymentStatus { get; set; }
        public BaseOption OrderState { get; set; }
        public List<OrderModelItem> Items { get; set; }
        public List<OrderModelHistory> Histories { get; set; }
        public decimal VAT { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public string Date { get; set; }
        public string FullDate { get; set; }


    }


    public class OrderModelHistory : OrderModelStatusBase
    {
        
        public string Date { get; set; }
        public string FullDate { get; set; }
        public string StatusId { get; set; }
        public string OrderId { get; set; }
        public string NotificationOption { get; set; }
        public string Comment { get; set; }
    }


    public class OrderModelItem
    {
        public string Id { get; set; }
       
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string FrameClass { get; set; }
        public string ImageURL { get; set; }

        public string PreviewImageURL { get;set; }
        public string CroppedImageURL { get; set; }
        public BaseOption Size { get; set; }
        public string FrameSizeName { get;set;}
        public string Heading { get; set; }
        public string Description { get; set; }
        public BaseOption Category { get; set; }
        public decimal Amount { get; set; }
        public bool DownloadingOriginal { get; set; }
        public bool DownloadingCropped { get; set; }
        public List<OrderModelItemImage> Images { get; set; }

    }


    public class OrderModelItemImage
    {
        public string Id { get; set; }
        public string ImageURL { get; set; }

        public string CroppedImageURL { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public bool Downloading { get; set; }
        public string FrameClass { get; set; }
        public string Size { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }




    }


    public class OrderInitResponse
    {
        public OrderResponse Order { get; set; }
        public List<BaseOption> Cities { get; set; }
        public List<BaseOption> States { get; set; }
        public List<CountryOption> Countries { get; set; }
        public List<OrderModelStatusBase> OrderStatuses { get; set; }
        public List<BaseOption> Customers { get; set; }
        public List<BaseOption> OrderStates { get; set; }
        public LeadTimeModel LeadTime { get; set; }
        public decimal NewOrderCount { get; set; }
        public DefNotificationModel DefNotification { get; set; }
    }


    public class OrderSummary
    {
        public decimal VAT { get; set; }
        public decimal Delivery { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GrandTotal { get; set; }
       
    }

    public class OrderResponse : OrderSummary
    {
        public List<OrderModel> Orders { get; set; }
        public decimal TotalOrder { get; set; }

    }

    public class OrderGroupBase
    {
        public List<OrderModel> Orders { get; set; }
    }

    public class OrderGroup : OrderGroupBase
    {
        public decimal Count { get; set; }
       
    }


    public class CustomerOrder
    {
        public OrderGroup Closed { get; set; }
        public OrderGroup Open { get; set; }
    }

}
