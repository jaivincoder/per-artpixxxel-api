

using api.artpixxel.data.Models.Base;
using System.Collections.Generic;

namespace api.artpixxel.data.Models
{
   public class WallArtCategory : AuditedDeletableEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
        public string ImageURL { get; set; }
        public string ImageAbsURL { get; set; }
        public string ImageRelURL { get; set; }
        public string Description { get; set; }
        public IEnumerable<WallArt> WallArts { get; } = new HashSet<WallArt>();
    }
}
