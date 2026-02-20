

using api.artpixxel.data.Features.Notifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
   public interface INotificationService
    {
        Task SendNotification(NotificationModel request);
        Task<NotificationResponse> Notifications(NotificationRequest request);
        Task<List<NotificationResponseModel>> Notifications();
        Task<NewNotificationResponse> NewNotifications();
    }
}
