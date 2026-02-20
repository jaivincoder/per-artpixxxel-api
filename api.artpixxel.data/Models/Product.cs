

using api.artpixxel.data.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.artpixxel.data.Models
{
    public class Product : AuditedDeletableEntity
    {
        [Key]
        public string Id { get; set; }
        public ProductType Type { get; set; }
        public bool On { get; set; }
        public IEnumerable<Template> Templates { get; } = new HashSet<Template>();
    }
}
