

using api.artpixxel.data.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.artpixxel.data.Models
{
   public class CustomerCategory : AuditedDeletableEntity
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string ColorCode { get; set; }
        public bool IsDefault { get; set; }
        public string Description { get; set; }
        public IEnumerable<Customer> Customers { get; } = new HashSet<Customer>();
    }
}
