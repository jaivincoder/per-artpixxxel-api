

using api.artpixxel.data.Models.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.artpixxel.data.Models
{
   public class OrderStatusHistory : AuditedDeletableEntity
    {
        public string Id { get; set; }
        public NotificationOption NotificationOption { get; set; }
        public string OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }
        public string StatusId { get; set; }
        [ForeignKey(nameof(StatusId))]
        public OrderStatus Status { get; set; }
        public string Label { get; set; }  // in case the Status got deleted, we can see construct the history after
        public string ColorCode { get; set; }
        public string Icon { get; set; }
        public string Comment { get; set; }
    }
}
