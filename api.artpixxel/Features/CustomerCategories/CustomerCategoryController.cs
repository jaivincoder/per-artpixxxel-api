
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.CustomerCategories;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.Features.CustomerCategories
{
   
    public class CustomerCategoryController : ApiController
    {
        private readonly ICustomerCategoryService _customerCategoryService;
        public CustomerCategoryController(ICustomerCategoryService customerCategoryService)
        {
            _customerCategoryService = customerCategoryService;
        }

        [HttpGet]
        [Route(nameof(Categories))]
        public async Task<List<CustomerCategoryResponse>> Categories()
            => await _customerCategoryService.Categories();

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<CustomerCategoryCRUDResponse> Create(CustomerCategoryRequest request)
            => await _customerCategoryService.Create(request);

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<CustomerCategoryCRUDResponse> Update(CustomerCategoryRequest request)
            => await _customerCategoryService.Update(request);

        [HttpPost]
        [Route(nameof(Duplicate))]
        public async Task<BaseBoolResponse> Duplicate(BaseOption request)
            => await _customerCategoryService.Duplicate(request);

        [HttpPost]
        [Route(nameof(Exists))]
        public async Task<BaseBoolResponse> Exists(BaseStringRequest request)
            => await _customerCategoryService.Exists(request);

        [HttpPost]
        [Route(nameof(BatchCreate))]
        public async Task<CustomerCategoryCRUDResponse> BatchCreate(List<CustomerCategoryRequest> request)
            => await _customerCategoryService.BatchCreate(request);

        [HttpPost]
        [Route(nameof(Delete))]
        public async Task<CustomerCategoryCRUDResponse> Delete(BaseId request)
            => await _customerCategoryService.Delete(request);


        [HttpPost]
        [Route(nameof(BatchDelete))]
        public async Task<CustomerCategoryCRUDResponse> BatchDelete(CustomerCategoryBatchDelete request)
            => await _customerCategoryService.BatchDelete(request);
    }
}
