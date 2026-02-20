
using api.artpixxel.data.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.artpixxel.data.Models
{
  public  class MixnMatch  : AuditedDeletableEntity
    {
        [Key]
        public string Id { get; set; }
        public byte[] Image { get; set; }
        public string Name { get; set; }
        public string ImageURL { get; set; }
        public string ImageAbsURL { get; set; }
        public string ImageRelURL { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public decimal Price { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public MixnMatchCategory Category { get; set; }
    }
}
