
using api.artpixxel.data.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace api.artpixxel.data.Models
{
   public class Carousel : AuditedDeletableEntity
    {
      [Key]
      public string Id { get; set; }
      public string  Heading { get; set; }
      public string HeadingColour { get; set; }
      public string BackgroundColour { get; set; }
      public string BodyText { get; set; }
      public string BodyTextColour { get; set; }
      public CarouselType Type { get; set; }
      public string Link { get; set; }
      public bool Active { get; set; }
      public string LinkLabel { get; set; }
      public string LinkLabelColour { get; set; }
      public byte[] Image { get; set; }
      public string ImageURL { get; set; }
      public string ImageRelURL { get; set; }
      public string ImageAbsURL { get; set; }
    }
}
