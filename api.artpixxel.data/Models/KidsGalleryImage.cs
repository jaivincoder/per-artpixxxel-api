

using api.artpixxel.data.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace api.artpixxel.data.Models
{
    public class KidsGalleryImage : AuditedDeletableEntity
    {
        [Key]
        public string Id { get; set; }
        public byte[] Image { get; set; }
        public string Name { get; set; }
        public string ImageURL { get; set; }
        public string ImageAbsURL { get; set; }
        public string ImageRelURL { get; set; }
        public string CroppedImageURL { get; set; }

        public string CroppedImageAbsURL { get; set; }

        public string CroppedImageRelURL { get; set; }
        public string Description { get; set; }
    }

    
}
