

using api.artpixxel.data.Features.AddressBooks;
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Customers;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.Accounts
{
   public class DuplicateAccount
    {
      public string EmailAddress { get; set; }
      public string MobileNumber { get; set; }
      public string Username { get; set; }
    }


    public class AccountCreateRequest : DuplicateAccount
    {
       public string FirstName { get; set; }
       public string LastName { get; set; }
       public string Password { get; set; }
       public bool Terms { get; set; }
       public bool Newsletter { get; set; }
    }


    public class AccountModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
    }

    public class PasswordReset
    {
        public string UserId { get; set; }
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
        public bool NotifyUser { get; set; }
    }


    public class SelfPasswordReset
    {
       public string CurrentPassword { get; set; }
       public string NewPassword { get; set; }
    }

    public class CustomerInfo
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string AdditionalMobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Gender { get; set; }
        public bool Newsletter { get; set; }
        public string Dob { get; set; }
        public List<CustomerAddressBook> AddressBooks { get; set; }
        public string Username { get; set; }
        public string Photo { get; set; }
    }

    public class CustomerInfoData
    {
        public CustomerInfo Customer { get; set; }
        public List<BaseOption> States { get; set; }
        public List<BaseOption> Cities { get; set; }
        public List<CountryOption> Countries { get; set; }
        public decimal Notifications { get; set; }
        
    }


    public class CustomerDetail
    {
     
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public string MobileNumber { get; set; }
      public string EmailAddress { get; set; }
      public string Gender { get; set; }
      public string Dob { get; set; }
      public string Photo { get; set; }
      public string Username { get; set; }
      public string AdditionalMobileNumber { get; set; }
    }

    
    public class CustomerNotice
    {
        public int NewMessages { get; set; } 
    }


    public class ResetTokenRequest
    {
        public string EmailAddress { get; set; }
    }

    public class TokentNotificationModel
    {
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string Token { get; set; }
    }
    

    public class PasswordResetRequest
    {
        public string Password { get; set; }
        public string Token { get; set; }
    }


}
