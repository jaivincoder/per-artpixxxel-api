

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Customers;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.artpixxel.Features.Customers
{
   
    public class CustomerController : ApiController
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost]
        [Route(nameof(Customers))]
        public async Task<CustomerData> Customers(Pagination pagination)
            => await _customerService.Customers(pagination);


        [HttpPost]
        [Route(nameof(CustomersPaginated))]
        public async Task<CustomersResponse> CustomersPaginated(CustomerFilterData Filter)
            => await _customerService.CustomersPaginated(Filter);

        [HttpPost]
        [Route(nameof(BulkDelete))]
        public async Task<CustomerCRUDResponse> BulkDelete(CustomerBulkDelete request)
            => await _customerService.BulkDelete(request);

        [HttpPost]
        [Route(nameof(BulkUpdate))]
        public async Task<CustomerCRUDResponse> BulkUpdate(CustomerBulkUpdate request)
            => await _customerService.BulkUpdate(request);

        [HttpPost]
        [Route(nameof(Delete))]
        public async Task<CustomerCRUDResponse> Delete(CustomerDelete request)
            => await _customerService.Delete(request);

        [HttpPost]
        [Route(nameof(Update))]
        public async Task<CustomerCRUDResponse> Update(CustomerUpdate request)
            => await _customerService.Update(request);

        [HttpPost]
        [Route(nameof(Customer))]
        public async Task<CustomerResponse> Customer(BaseId request)
            => await _customerService.Customer(request);
    }
}
