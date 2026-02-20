
using api.artpixxel.data.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.artpixxel.data.Models
{
   public class WallArt : AuditedDeletableEntity
    {
     public string Id { get; set; }
     public decimal Amount { get; set; }
     public string CategoryId { get; set; }
     [ForeignKey(nameof(CategoryId))]
     public WallArtCategory Category { get; set; }
     public int  Rating { get; set; }
     public string WallArtSizeId { get; set; }
     [ForeignKey(nameof(WallArtSizeId))]
     public WallArtSize WallArtSize { get; set; }
     public string  Header { get; set; }
     public byte[] Image { get; set; }
     public string ImageURL { get; set; }
     public string ImageAbsURL { get; set; }
     public string ImageRelURL { get; set; }
     public bool FixedSize { get; set; }
     public string Description { get; set; }
     public IEnumerable<WallArtImage> Images { get; } = new HashSet<WallArtImage>();
        
    }
}
