using api.artpixxel.data.Features.Notifications;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.artpixxel.Features.Notifications
{
   
    public class NotificationController : ApiController
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost]
        [Route(nameof(Notifications))]
        public async Task<NotificationResponse> Notifications(NotificationRequest request)
            => await _notificationService.Notifications(request);

        [HttpGet]
        [Route(nameof(NewNotifications))]
        public async Task<NewNotificationResponse> NewNotifications()
            => await _notificationService.NewNotifications();
    }
}
