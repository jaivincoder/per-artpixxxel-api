

using api.artpixxel.data.Features.Common;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.AddressBooks
{
    public class CustomerAddressBook
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string AdditionalInformation { get; set; }
        public BaseOption City { get; set; }
        public BaseOption State { get; set; }
        public CountryOption Country { get; set; }
        public bool IsDefault { get; set; }
        public string CustomerId { get; set; }

    }

    public class AddressBookCRUDResponse
    {
        public List<CustomerAddressBook> AddressBooks { get; set; }
        public BaseResponse Response { get; set; }
    }

    public class AddressBookRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string AdditionalInformation { get; set; }
        public string CityName { get; set; }
        public bool IsDefault { get; set; }
        public string StateId { get; set; }
        public string CountryId { get; set; }
    }
    public class AddressBookDelete : BaseId
    {
        public string CustomerId { get; set; }
    }
    
}
