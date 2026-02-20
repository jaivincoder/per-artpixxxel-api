using api.artpixxel.data.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace api.artpixxel.data.Models
{
    public class DefNotification :  AuditedDeletableEntity
    {
        [Key]
        public string Id { get; set; }
        public string Message { get; set; }
        public NotificationOption NotificationOption { get; set; }
    }
}
