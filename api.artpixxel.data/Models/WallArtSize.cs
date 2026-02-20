

using api.artpixxel.data.Models.Base;
using System.Collections.Generic;

namespace api.artpixxel.data.Models
{
   public class WallArtSize : AuditedDeletableEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public bool IsDefault { get; set; }
        public string Description { get; set; }
        public IEnumerable<WallArt> WallArts { get; } = new HashSet<WallArt>();
        public IEnumerable<OrderItem> OrderItems { get; } = new HashSet<OrderItem>();
    }
}
