

using api.artpixxel.data.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.artpixxel.data.Models
{
  public  class MixnMatchCategory : AuditedDeletableEntity
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<MixnMatch> MixnMatches { get; } = new HashSet<MixnMatch>();
    }
}
