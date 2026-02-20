

using api.artpixxel.data.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.artpixxel.data.Models
{
   public class City : AuditedDeletableEntity
    {
        [Key]
        public string Id { get; set; }
        public string CityName { get; set; }
        public decimal DeliveryFee { get; set; }
        public string StateId { get; set; }
        public string Description { get; set; }
        [ForeignKey(nameof(StateId))]
        public State State { get; set; }
    }
}
