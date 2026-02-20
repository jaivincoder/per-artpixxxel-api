

using api.artpixxel.data.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.artpixxel.data.Models
{
    public class Customer : AuditedDeletableEntity
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public string CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public CustomerCategory Category { get; set; }
        public string AdditionalMobileNumber { get; set; }
        public string Location { get; set; }
        public string CityId { get; set; }
        public string CityName { get; set; }
        [ForeignKey(nameof(CityId))]
        public City City { get; set; }
        public decimal TotalOrder { get; set; }
        public decimal AverageOrder { get; set; }
        public decimal LastOrder { get; set; }
        public bool SubscribedNewsletter { get; set; }
        public IEnumerable<Order> Orders { get; } = new HashSet<Order>();
        public IEnumerable<AddressBook> AddressBooks { get; } = new HashSet<AddressBook>();

    }
}
