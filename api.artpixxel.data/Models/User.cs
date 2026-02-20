
using api.artpixxel.data.Models.Base;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace api.artpixxel.data.Models
{
    public class User : IdentityUser, IEntity
    {
        #region Properties

        
        /// <summary>
        /// Employee's  first name
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Employee's middle name
        /// </summary>
        public string MiddleName { get; set; }
        /// <summary>
        /// Employee's last name
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Employee's date of birth
        /// </summary>
        public DateTime? DOB { get; set; }
        /// <summary>
        /// Employee's gender
        /// </summary>
        public Gender Gender { get; set; }
        /// <summary>
        /// Employee's marital status
        /// </summary>
        public byte[] Passport { get; set; }
        public string PassportURL { get; set; }
        public string PassportAbsURL { get; set; }
        public string PassportRelURL { get; set; }
        public string Token { get; set; }

        public DateTime? LastLogin { get; set; }
        /// <summary>
        /// Employee's residential address
        /// </summary>
        public string HomeAddress { get; set; }

        /// <summary>
        /// The Date the user was created
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// The user that created the user. Can be nullable
        /// </summary>
        public string CreatedBy { get; set; }
        /// <summary>
        /// The date the user details was modified
        /// </summary>
        public DateTime? ModifiedOn { get; set; }
        /// <summary>
        /// The user user that modified the user information
        /// </summary>
        public string ModifiedBy { get; set; }
        /// <summary>
        /// Employee's state of origin id
        /// </summary>
        public string StateId { get; set; }
        /// <summary>
        /// Employee's state of origin
        /// </summary>
        [ForeignKey(nameof(StateId))]
        public virtual State State { get; set; }

        public string FullName
        {
            get { return string.IsNullOrEmpty(MiddleName) ? FirstName + " " + LastName : FirstName + " " + MiddleName + " " + LastName; }
        }

        public bool IsOnline { get; set; }
        public bool IsAdmin { get; set; }
        public string CurrentActivity { get; set; }
        public string CurrentChat { get; set; }
       
        private ICollection<UserNotification> UserNotifications { get; } = new List<UserNotification>();
       


        private ICollection<UserMessage> UserMessages { get; } = new List<UserMessage>();
        private ICollection<UserSentMessage> UserSentMessages { get; } = new List<UserSentMessage>();



        [NotMapped]
        public IEnumerable<Message> Sent => UserSentMessages.Select(e => e.Message);

        [NotMapped]
        public IEnumerable<Message> Messages => UserMessages.Select(e => e.Message);
        [NotMapped]
       
        public IEnumerable<Notification> Notifications => UserNotifications.Select(e => e.Notification);

        #endregion
    }
}
