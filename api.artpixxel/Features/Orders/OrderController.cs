using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Orders;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.artpixxel.Features.Orders
{
    
    public class OrderController : ApiController
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(Init))]
        public async Task<OrderInitResponse> Init(Pagination pagination)
            => await _orderService.Init(pagination);

        [HttpPost]
        [Route(nameof(InitNew))]
        public async Task<OrderInitResponse> InitNew(Pagination pagination)
            => await _orderService.InitNew(pagination);

        [HttpPost]
        [Route(nameof(Orders))]
        public async Task<OrderResponse> Orders(OrderFilterData filter)
            => await _orderService.Orders(filter);

        [HttpPost]
        [Route(nameof(Customer))]
        public async Task<CustomerOrder> Customer(Pagination pagination)
            => await _orderService.Customer(pagination);

        [HttpPost]
        [Route(nameof(Open))]
        public async Task<OrderGroup> Open(SearchFilter filter)
            => await _orderService.Open(filter);


        [HttpPost]
        [Route(nameof(Closed))]
        public async Task<OrderGroup> Closed(SearchFilter filter)
          => await _orderService.Closed(filter);

        [HttpPost]
        [Route(nameof(SearchOpen))]
        public async Task<OrderGroup> SearchOpen(SearchFilter request)
            => await _orderService.SearchOpen(request);

        [HttpPost]
        [Route(nameof(SearchClosed))]
        public async Task<OrderGroup> SearchClosed(SearchFilter request)
            => await _orderService.SearchClosed(request);

        [AllowAnonymous]
        [HttpGet]
        [Route("Pay/{id}")]
        public async Task<BaseResponse> Pay(string id)
            => await _orderService.Pay(id);
    }
}
