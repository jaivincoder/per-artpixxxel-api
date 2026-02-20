

using api.artpixxel.data.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.artpixxel.data.Models
{
   public class WallArtImage : AuditedDeletableEntity
    {
        public string Id { get; set; }
        public byte[] Image { get; set; }
        public string ImageURL { get; set; }
        public string ImageAbsURL { get; set; }
        public string ImageRelURL { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string WallArtId { get; set; }
        [ForeignKey(nameof(WallArtId))]
        public WallArt WallArt { get; set; }
    }
}
