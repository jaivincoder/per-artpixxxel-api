


using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Notifications;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.Admin
{
  
    public class UserInfo 
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Username { get; set; }
        public string Dob { get; set; }
        public string MobileNumber { get; set; }
        public string Gender { get; set; }
        public string HomeAddress { get; set; }
        public string Photo { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        
      
    }


    public class AdminData
    {
        public UserInfo UserInfo { get; set; }
        public List<CountryOption> Countries { get; set; }
        public List<BaseOption> States { get; set; }
        public List<NotificationResponseModel> Notifications { get; set; }
        public decimal Orders { get; set; }

    }

    public class UserInfoUpdate : UserInfo
    {
      public string Password { get; set; }
    }

}
