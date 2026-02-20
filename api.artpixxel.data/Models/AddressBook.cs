
using api.artpixxel.data.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.artpixxel.data.Models
{
   public class AddressBook : AuditedDeletableEntity
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string AdditionalInformation { get; set; }
        public string CityId { get; set; }
        public bool IsDefault { get; set; }
        [ForeignKey(nameof(CityId))]
        public City City { get; set; }
        public string CityName { get; set; }
        public string StateId { get; set; }
        [ForeignKey(nameof(StateId))]
        public State State { get; set; }
        public string CountryId { get; set; }
        [ForeignKey(nameof(CountryId))]
        public Country Country { get; set; }
        public string CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }
    }
}
