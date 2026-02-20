
using api.artpixxel.data.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace api.artpixxel.data.Models
{
    public class KidsTemplateSize : AuditedDeletableEntity
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public decimal Amount { get; set; }
        public bool Default { get; set; }
        public KidsTemplate Template { get; set; }
        public string Description { get; set; }
    }
}

