

using api.artpixxel.data.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.artpixxel.data.Models
{
   public class OrderItem : AuditedDeletableEntity
    {  
        [Key]
        public string Id { get; set; }
        public OrderItemCategory Category { get; set; }
        public OrderType Type { get; set; } 
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string FrameClass { get; set; }
        public string LineStyle { get; set; }
        public string LineColor { get; set; }
        public string Design { get; set; }
        public byte[] Image { get; set; }
        public string ImageURL { get; set; }
        public string ImageAbsURL { get; set; }
        public string ImageRelURL { get; set; }
        public string OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }
        public string WallArtSizeId { get; set; }
        [ForeignKey(nameof(WallArtSizeId))]
        public WallArtSize WallArtSize { get; set; }
        public string SizeId { get; set; }
        public string SizeName { get; set; }
        [ForeignKey(nameof(SizeId))]
        public Size Size { get; set; }
        public string MixnMatchId { get; set; }
        [ForeignKey(nameof(MixnMatchId))]
        public MixnMatch MixnMatch { get; set; }
        public string WallArtId { get; set; }
        [ForeignKey(nameof(WallArtId))]
        public WallArt WallArt { get; set; }
       
        public string MixedTemplateSizeId { get; set; }
        [ForeignKey(nameof(MixedTemplateSizeId))]
        public MixedTemplateSize MixedTemplateSize { get; set; }


        public string ChristmasTemplateSizeId { get; set; }
        [ForeignKey(nameof(ChristmasTemplateSizeId))]
        public ChristmasTemplateSize ChristmasTemplateSize { get; set; }

        public string KidsTemplateSizeId { get; set; }
        [ForeignKey(nameof(KidsTemplateSizeId))]
        public KidsTemplateSize KidsTemplateSize { get; set; }


        public string RegularTemplateSizeId { get; set; }
        [ForeignKey(nameof(RegularTemplateSizeId))]
        public RegularTemplateSize RegularTemplateSize { get; set; }


        public string TempId { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }

        public byte[] CroppedImage { get; set; }
        public string CroppedImageURL { get; set; }
        public string CroppedImageAbsURL { get; set; }

        public byte[] PreviewImage { get; set; }
        public string PreviewImageURL { get; set; }
        public string PreviewImageAbsURL { get; set; }

        public string Heading { get; set; }
        public string Description { get; set; }
        public IEnumerable<OrderItemImage> Images { get; } = new HashSet<OrderItemImage>();
    }
}
