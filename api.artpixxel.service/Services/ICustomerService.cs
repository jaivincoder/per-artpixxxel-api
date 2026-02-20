

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Customers;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
    public interface ICustomerService
    {
        Task<CustomerData> Customers(Pagination pagination);
        Task<CustomersResponse> CustomersPaginated(CustomerFilterData filter);
        Task<CustomerResponse> Customer(BaseId request);
        Task<CustomerCRUDResponse> BulkDelete(CustomerBulkDelete request);
        Task<CustomerCRUDResponse> BulkUpdate(CustomerBulkUpdate request);
        Task<CustomerCRUDResponse> Update(CustomerUpdate request);
        Task<CustomerCRUDResponse> Delete(CustomerDelete request);

    }
}
