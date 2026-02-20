

using Microsoft.AspNetCore.Identity;
using System;

namespace api.artpixxel.data.Models
{
    public class UserRole : IdentityRole
    {
        public UserRole() : base() { }
        public UserRole(string roleName) : base(roleName) { }
        public UserRole(string roleName, string description, DateTime creationDate, string createdBy) : base(roleName)
        {
            this.Description = description;
            this.CreationDate = creationDate;
            this.CreatedBy = createdBy;
            

        }
        public string Description { get; set; }
      
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; }



    }
}
