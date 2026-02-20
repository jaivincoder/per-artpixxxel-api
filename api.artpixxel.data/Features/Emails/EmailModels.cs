

using api.artpixxel.data.Features.Accounts;
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.States;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.Emails
{
    public class NewsletterRequest
    {
        public string EmailAddress { get; set; }
    }

    public class EmailListRequest  : NewsletterRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }


    public class EmailListResponse : EmailListRequest
    {
        public string Id { get; set; }
        public string SignupDate { get; set; }
    }


    public class EmailListInit
    {
        public List<EmailListResponse> EmailList { get; set; }
        public decimal TotalEmail { get; set; }

    }


    public class EmailListFilter
    {
        public string SignupDate { get; set; }
        public string SignupDateMatchMode { get; set; }
    }

    public class EmailListFilterData : Filter
    {
        public EmailListFilter Filters { get; set; }

    }


    public class EmailListMultiDeleteRequest
    {
       public List<string> Ids { get; set; }
       public EmailListFilterData Filter { get; set; }
    }

    public class EmailListDeleteRequest
    {
        public string Id { get; set; }
        public EmailListFilterData Filter { get; set; }
    }


    public class EmailListCRUDResponse
    {
        public BaseResponse Response { get; set; }
        public EmailListInit Data { get; set; }
    }

    public class EmailSortField
    {
        public const string FIRSTNAME = "firstName";
        public const string LASTNAME = "lastName";
        public const string EMAILADDRESS = "emailAddress";
        public const string SIGNUPDATEFILTERED = "signupDateFiltered";


    }


}
