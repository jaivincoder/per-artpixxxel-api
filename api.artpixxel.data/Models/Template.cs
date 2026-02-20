

using api.artpixxel.data.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.artpixxel.data.Models
{
    public class Template : AuditedDeletableEntity
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }    
        public bool On { get; set; }
        public string ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
        


    }
}
