

using api.artpixxel.data.Features.Common;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.Customers
{
   public class CustomerModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string DOB { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public string Gender { get; set; }
        public string Passport { get; set; }
        public string LastLogin { get; set; }
        public string LastLoginDate { get; set; }
        public string HomeAddress { get; set; }
        public BaseOption State { get; set; }
        public CountryOption Country { get; set; }
        public bool IsOnline { get; set; }
        public string CategoryId { get; set; }
        public decimal TotalOrder { get; set; }
        public decimal AverageOrder { get; set; }
        public decimal LastOrder { get; set; }
        public CustomerCategoryOption Category { get; set; }
        public string AdditionalMobileNumber { get; set; }
        public string DateRegistered { get; set; }
        public BaseOption City { get; set; }
    }

    public class CustomersResponse
    {
        public List<CustomerModel> Customers { get; set; }
        public decimal TotalCount { get; set; }
    }


    public class CustomerResponse
    {
        public CustomerModel Customer { get; set; }
        public BaseResponse Response { get; set; }
    }


    public class CustomerCategoryOption : CountOption
    {
        public string Color { get; set; }
    }


    public class CustomerData
    {
        public CustomersResponse CustomerResponse { get; set; }
        public List<BaseOption> Cities { get; set; }
        public List<CountryOption> Countries { get; set; }
        public List<BaseOption> States { get; set; }
        public List<CustomerCategoryOption> Categories { get; set; }
    }

    public class CustomerBulkDelete
    {
        public List<string> Ids { get; set; }
        public CustomerFilterData FilterData { get; set; }
        
    }



    public class CustomerDelete
    {
        public string Id { get; set; }
        public CustomerFilterData FilterData { get; set; }
    }


    public class CustomerUpdate : CustomerDelete
    {
        public string CategoryId { get; set; }
    }


    public class CustomerBulkUpdate : CustomerBulkDelete
    {
        public string CategoryId { get; set; }
    }


    public class CustomerCRUDResponse
    {
        public BaseResponse Response { get; set; }
        public CustomersResponse Customer { get; set; }
    }
 


   

   


}
