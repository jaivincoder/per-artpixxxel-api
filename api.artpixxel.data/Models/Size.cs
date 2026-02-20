using api.artpixxel.data.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.artpixxel.data.Models
{
   public class Size : AuditedDeletableEntity
    {
      [Key]
      public string Id { get; set; }
      public string Name { get; set; }
      public SizeType Type { get; set; }
      public double Width { get; set; }
      public double Height { get; set; }
      public decimal Amount { get; set; }
      public bool Default { get; set; }
      public string Description { get; set; }
      public IEnumerable<OrderItem> OrderItems { get; } = new HashSet<OrderItem>();
    }
}
