

using api.artpixxel.data.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.artpixxel.data.Models
{
   public class Order : AuditedDeletableEntity
    {
       
        [Key]
        public string Id { get; set; }
        public string AdditionalPhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public string Name { get; set; }
        public string StatusId { get; set; }
        [ForeignKey(nameof(StatusId))]
        public OrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public OrderState OrderState { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal VAT { get; set; }
        public string ShippingAddress { get; set; }
        public string ZipCode { get; set; }
        public string CityId { get; set; }
        [ForeignKey(nameof(CityId))]
        public City City { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; } 
        public DateTime SpeculativeDeliveryDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal GrandTotal { get; set; }
      
        public string StateId { get; set; }
        [ForeignKey(nameof(StateId))]
        public State State { get; set; }
        public string CountryId { get; set; }
        [ForeignKey(nameof(CountryId))]
        public Country Country { get; set; }
        public bool Seen { get; set; } 
        public string PaymentIntentId { get; set; }
        public IEnumerable<OrderItem> Items { get; } = new HashSet<OrderItem>();
        public IEnumerable<OrderCartItems> ItemsNew { get; } = new HashSet<OrderCartItems>();
        public IEnumerable<OrderStatusHistory> Histories { get; } = new HashSet<OrderStatusHistory>();
        

    }

    public class OrderNew
    {
        public string Id { get; set; }
        public string AdditionalPhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string Name { get; set; }
        public string StatusId { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public OrderState OrderState { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal VAT { get; set; }
        public string ShippingAddress { get; set; }
        public string ZipCode { get; set; }
        public string CityId { get; set; }
        public City City { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
        public DateTime SpeculativeDeliveryDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime CreatedOn { get; set; }
        public string StateId { get; set; }
        public State State { get; set; }
        public string CountryId { get; set; }
        public Country Country { get; set; }
        public bool Seen { get; set; }
        public string PaymentIntentId { get; set; }
        public IEnumerable<OrderItem> Items { get; set; } = new HashSet<OrderItem>();
        public IEnumerable<OrderCartItems> ItemsNew { get; set; } = new HashSet<OrderCartItems>();
        public IEnumerable<OrderStatusHistory> Histories { get; set; } = new HashSet<OrderStatusHistory>();
    }
}
