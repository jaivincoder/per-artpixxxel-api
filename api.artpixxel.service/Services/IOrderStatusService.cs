

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.OrderStatuses;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
   public interface IOrderStatusService
    {
       Task<OrderStatusCRUDResponse> Set(OrderStatusRequest request);
        Task<OrderStatusHistoryResponse> History(OrderStatusHistoryRequest request);
        Task<DefNotificationResponse> DefNotification(DefNotificationModel request);
        Task<string> SendNotification(OrderStatusNotificationRequest request);
    }
}
