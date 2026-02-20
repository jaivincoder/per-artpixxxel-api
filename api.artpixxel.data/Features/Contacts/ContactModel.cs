

using api.artpixxel.data.Features.Common;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.Contacts
{
    public class MultipleContactRequest
    {
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
    }


    public class ContactRequest : MultipleContactRequest
    {
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }

    }

    public class ContactListResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string CreatedOn { get; set; }
    }

    public class ContactListInit
    {
        public List<ContactListResponse> ContactList { get; set; }
        public decimal TotalCount { get; set; }
    }
}
