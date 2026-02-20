

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.LeadTimes;
using api.artpixxel.data.Models;
using System;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.Checkouts
{
   

    public class PickupStationInformation
    {
      public string  Id { get; set; }
      public string  Address { get; set; }
      public bool  Checked { get; set; }
    }

    public class DeliveryInformation
    {
       public string  DeliveryMethod { get; set; }
       public string PickupDate { get; set; }
       public PickupStationInformation PickupStation { get; set; }
    }


    public class PaymentInformation
    {
       public string PaymentMethod { get; set; }
       public string Voucher { get; set; }
    }


    public class ShippingInformation
    {
      
       public string AdditionalPhoneNumber { get; set; }
       public string City { get; set; }
       public string EmailAddress { get; set; }
       public string FirstName { get; set; }
       public string LastName { get; set; }
       public string MobilePhoneNumber { get; set; }
       public string Password { get; set; }
       public string ShippingAddress { get; set; }
       public string State { get; set; }
       public string Country { get; set; }
       public bool UserAccount { get; set; }
       public string Username { get; set; }
       public string Zipcode { get; set; }
    }


   public class CheckoutNotification
    {
        public Order Order { get; set; }
        public string AdditionalPhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string ShippingAddress { get; set; }
        public string EmailAddress { get; set; }
        public string FullName { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }

   public  class Checkout
    {
     public DeliveryInformation DeliveryInformation { get; set; }
     public PaymentInformation PaymentInformation { get; set; }
     public ShippingInformation ShippingInformation { get; set; }
     public Cart Cart { get; set; }
     public LeadTimeBase LeadTime { get; set; }
     public string UpperLimit { get; set; }
     public string LowerLimit { get; set; }
     public string PaymentIntentId { get; set; }
     public decimal DeliveryFee { get; set; }
     public decimal Vat { get; set; }

    }


    public class PaymentIntentCheckout
    {
        public DeliveryInformation DeliveryInformation { get; set; }
        public PaymentInformation PaymentInformation { get; set; }
        public ShippingInformation ShippingInformation { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal Vat { get; set; }
        public string Id { get; set; }
        public decimal Total { get; set; }
        public string ClientSecret { get; set; }
        public string PaymentIntentId { get; set; }
    }



    public class PaymentIntentCheckoutResponse
    {
        public PaymentIntentCheckout Checkout { get; set; }
        public BaseResponse Response { get; set; }
    }




    public class CartItem
    {
      public string Id { get; set; }
      public string ItemName { get; set; }
      public int Quantity { get; set; }
      public string FrameClass { get; set; }
      public string Image { get; set; }
      public string Size { get; set; }
      public List<ImageModel> Images { get; set; }
      public decimal Amount { get; set; }
      public string CroppedImage { get; set; }
      public string LineStyle { get; set; }
      public string LineColor { get; set; }
      public string Design { get; set; }
      public string PreviewImage { get; set; }
      public decimal TotalAmount { get; set; }
      public string Category { get; set; }
      public string Type { get; set; }
      public string Heading { get; set; }
      public string Description { get; set; }
    }


    public class Cart
    {
        public List<CartItem> Items { get; set; }
        public decimal TotalAmount { get; set; }
    }


    public class CheckoutResponse
    {
        public BaseResponse Response { get; set; }
        public string TrackId { get; set; }
    }


    public class CheckoutNew
    {
        public DeliveryInformation DeliveryInformation { get; set; }
        public PaymentInformation PaymentInformation { get; set; }
        public ShippingInformation ShippingInformation { get; set; }
        public CartNew Cart { get; set; }
        public LeadTimeBase LeadTime { get; set; }
        public string UpperLimit { get; set; }
        public string LowerLimit { get; set; }
        public string PaymentIntentId { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal Vat { get; set; }

    }

    public class CartNew
    {
        public List<CartItemNew> Items { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class CartItemNew
    {
        public int CategoryId { get; set; }
        public decimal TotalAmountPerCategory { get; set; }
        public List<FrameSet> Frames { get; set; }
        
    }

    public class FrameSet
    {
        public string? FrameId { get; set; }
        public int? templateConfigId { get; set; }
        public string FrameClass { get; set; }
        public string LineColor { get; set; }
        public string PreviewImage { get; set; }
        public string UniqueItemId { get; set; }
        public List<ItemCardImage> Images { get; set; }
        public Decimal Amount { get; set; }
        public int Quantity { get; set; }
        public string FrameSize { get; set; }
        public string FrameSizeName { get; set; }

        public decimal FramePrice { get; set; }
    }
    public class ItemCardImage
    {
        public string CroppedImages { get; set; }
        public string OriginalImages { get; set; }
    }

    /// <summary>
    /// Combined checkout request - can contain regular cart, custom-mix cart, or both.
    /// Backend splits delivery/vat proportionally when both are present.
    /// </summary>
    public class CheckoutCombinedRequest
    {
        public DeliveryInformation DeliveryInformation { get; set; }
        public PaymentInformation PaymentInformation { get; set; }
        public ShippingInformation ShippingInformation { get; set; }
        public LeadTimeBase LeadTime { get; set; }
        public string UpperLimit { get; set; }
        public string LowerLimit { get; set; }
        public string PaymentIntentId { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal Vat { get; set; }
        public Cart RegularCart { get; set; }
        public CartNew CustomMixCart { get; set; }
    }

    public class CheckoutCombinedResponse
    {
        public BaseResponse Response { get; set; }
        public List<string> TrackIds { get; set; } = new List<string>();
    }
   
}
