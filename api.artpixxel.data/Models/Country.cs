using api.artpixxel.data.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.artpixxel.data.Models
{
    public class Country : AuditedDeletableEntity
    {
        [Key]
        public string Id { get; set; }
        public string FlagId { get; set; }
        public string Name { get; set; }
        [ForeignKey(nameof(FlagId))]
        public Flag Flag { get; set; }
        public decimal DeliveryFee { get; set; }
        public string Description { get; set; }
        public IEnumerable<State> States { get; } = new HashSet<State>();
    }
}
