
using api.artpixxel.data.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace api.artpixxel.data.Models
{
    public class HomeGallery : AuditedDeletableEntity
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public byte[] Image { get; set; }
        public string ImageURL { get; set; }
        public string ImageAbsURL { get; set; }
        public string ImageRelURL { get; set; }
        public string Description { get; set; }     
        public HomeGalleryImageType Type { get; set; }
    }
}
