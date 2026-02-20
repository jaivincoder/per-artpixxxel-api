

using api.artpixxel.data.Models;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.Notifications
{

    public class NotificationContent
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public AccessType AccessType { get; set; }
        public string SubjectId { get; set; }
        public NotificationPriority NotificationPriority { get; set; }
    }
   public class NotificationModel
    {
        public NotificationContent NotificationRequest { get; set; }
        public List<string>  Recipients { get; set; }
    }

    public class NotificationStatus
    {
        public bool Sent { get; set; }
        public string NotificationId { get; set; }
    }


    public class NotificationResponseModel
    {
        public string SubjectPhoto { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Time { get; set; }
    }

    public class NotificationResponse
    {
       public List<NotificationResponseModel> Notifications { get; set; }
    }

    public class NewNotificationResponse
    {
        public decimal Notifications { get; set; }
        public decimal Orders { get; set; }
    }

    public class NotificationRequest
    {
        public int PageSize { get; set; }
    }

  
}
