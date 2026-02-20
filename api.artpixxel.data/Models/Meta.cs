

using api.artpixxel.data.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace api.artpixxel.data.Models
{
   public class Meta :   AuditedDeletableEntity
    {
        [Key]
        public string Id { get; set; }
        public MetaType MetaType { get; set; }
        public string Value { get; set; }
    }
}
