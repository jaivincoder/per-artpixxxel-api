

using api.artpixxel.data.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.artpixxel.data.Models
{
    public class State : AuditedDeletableEntity
    {
        [Key]
        public string Id { get; set; }
        public string StateName { get; set; }
        public decimal DeliveryFee { get; set; }
        public string CountryId { get; set; }
        [ForeignKey(nameof(CountryId))]
        public Country Country { get; set; }
        public string Description { get; set; }
        public IEnumerable<City> Cities { get; } = new HashSet<City>();

    }
}
