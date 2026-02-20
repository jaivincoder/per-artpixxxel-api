using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.OrderStatuses;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.artpixxel.Features.OrderStatuses
{
    
    public class OrderStatusController : ApiController
    {
        private readonly IOrderStatusService _orderStatusService;
        public OrderStatusController(IOrderStatusService orderStatusService)
        {
            _orderStatusService =   orderStatusService; 
        }

        [HttpPost]
        [Route(nameof(Set))]
        public async Task<OrderStatusCRUDResponse> Set(OrderStatusRequest request)
            => await _orderStatusService.Set(request);

        [HttpPost]
        [Route(nameof(History))]
        public async Task<OrderStatusHistoryResponse> History(OrderStatusHistoryRequest request)
            => await _orderStatusService.History(request);


        [HttpPost]
        [Route(nameof(DefNotification))]
        public async Task<DefNotificationResponse> DefNotification(DefNotificationModel request)
            => await _orderStatusService.DefNotification(request);
    }
}
