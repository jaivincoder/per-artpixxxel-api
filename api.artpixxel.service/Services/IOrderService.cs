
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Orders;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
   public interface IOrderService
    {
        Task<OrderInitResponse> Init(Pagination pagination);
        Task<OrderInitResponse> InitNew(Pagination pagination);
        Task<OrderResponse> Orders(OrderFilterData filter);
        Task<CustomerOrder> Customer(Pagination pagination);
        Task<OrderGroup> Open(SearchFilter pagination);
        Task<OrderGroup> Closed(SearchFilter pagination);
        Task<OrderGroup> SearchOpen(SearchFilter request);
        Task<OrderGroup> SearchClosed(SearchFilter request);
        Task<BaseResponse> Pay(string id);
    }
}
