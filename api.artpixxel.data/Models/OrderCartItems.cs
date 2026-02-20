using api.artpixxel.data.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.artpixxel.data.Models
{
    public class OrderCartItems : AuditedDeletableEntity
    {
        [Key]
        public int id { get; set; }
        public int CategoryId { get; set; }
        public string OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }
        public decimal TotalAmountPerCategory { get; set; }
        public string? FrameId { get; set; }
        public int? TemplateConfigId { get; set; }
        public string FrameClass { get; set; }
        public string LineColor { get; set; }
        public string PreviewImage { get; set; }
        public string UniqueItemId { get; set; }
        public Decimal Amount { get; set; }
        public int Quantity { get; set; }
        public string FrameSize { get; set; }

    }
}
