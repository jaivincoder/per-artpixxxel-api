
using api.artpixxel.data.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace api.artpixxel.data.Models
{
    public class Notification : AuditedDeletableEntity
    {
        [Key]
        public string Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string SubjectId { get; set; }
        [ForeignKey(nameof(SubjectId))]
        public User Subject { get; set; }
        private ICollection<UserNotification> UserNotifications { get; } = new List<UserNotification>();
        public AccessType AccessType { get; set; }
        public NotificationPriority NotificationPriority { get; set; }

        [NotMapped]
        public IEnumerable<Notification> UsersNotifications => UserNotifications.Select(e => e.Notification);


    }
}
