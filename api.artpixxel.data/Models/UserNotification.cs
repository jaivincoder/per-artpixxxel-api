

using api.artpixxel.data.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.artpixxel.data.Models
{
    public class UserNotification : AuditedDeletableEntity
    {

        public User User { get; set; }
        public string UserId { get; set; }
        public string NotificationId { get; set; }
      
        [ForeignKey(nameof(NotificationId))]
        public Notification Notification { get; set; }

        public ReadStatus ReadStatus { get; set; }
    }
}
