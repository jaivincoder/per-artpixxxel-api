

using api.artpixxel.data.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.artpixxel.data.Models
{
   public class OrderStatus : AuditedDeletableEntity
    {
        [Key]
        public string Id { get; set; }
        public bool IsDefault { get; set; }
        public string Label { get; set; }
        public string ColorCode { get; set; }
        public string Icon { get; set; }
        public IEnumerable<Order> Orders { get; } = new HashSet<Order>();

    }
}
