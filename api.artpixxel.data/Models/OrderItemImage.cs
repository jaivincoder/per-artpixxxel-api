

using api.artpixxel.data.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.artpixxel.data.Models
{
  public  class OrderItemImage : AuditedDeletableEntity
    {
        public string Id { get; set; }
        public byte[] Image { get; set; }
        public string ImageURL { get; set; }
        public string ImageAbsURL { get; set; }
        public string ImageRelURL { get; set; }
        public string CroppedImageURL { get; set; }
        public string CroppedImageAbsURL { get; set; }
        public string CroppedImageRelURL { get; set; }
        public string WallArtImageId { get; set; }
        [ForeignKey(nameof(WallArtImageId))]
        public WallArtImage WallArtImage { get; set; }
        public string OrderItemId { get; set; }
        [ForeignKey(nameof(OrderItemId))]
        public OrderItem OrderItem { get; set; }
        
    }
}
