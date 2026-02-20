

using api.artpixxel.data.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace api.artpixxel.data.Models
{
   public class LeadTime :  AuditedDeletableEntity
    {
        [Key]
        public string Id { get; set; }
        public TimeLimit TimeLimit { get; set; }
        public int LowerBandQuantifier { get; set; }
        public int UpperBandQuantifier { get; set; }
        public string Description { get; set; }
    }
}
