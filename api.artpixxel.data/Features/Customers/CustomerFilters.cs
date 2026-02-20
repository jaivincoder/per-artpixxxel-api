

using api.artpixxel.data.Features.Common;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.Customers
{

    public class CustomerFilter
    {
        public List<string> Categories { get; set; }
        public List<string> Cities { get; set; }
        public List<string> Countries { get; set; }
        public string DateRegistered { get; set; }
        public string DateRegisteredMatchMode { get; set; }
        public List<string> States { get; set; }
        public decimal TotalOrder { get; set; }
        public string TotalOrderMatchMode { get; set; }
    }

    public class CustomerFilterData : Filter
    {
        public CustomerFilter Filters { get; set; }
      
    }

    public class CustomerSortField
    {
        public const string FULLNAME = "fullName";
        public const string EMAILADDRESS = "emailAddress";
        public const string USERNAME = "username";
        public const string CATEGORY = "category";
        public const string CITY = "city";
        public const string STATE = "state";
        public const string COUNTRY = "country";
        public const string DATEREGISTERED = "dateRegisteredFilter";
        public const string LASTLOGIN = "lastLogin";
        public const string TOTALORDER = "totalOrder";
    }

}
