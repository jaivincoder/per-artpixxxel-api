

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.CustomerCategories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
   public interface ICustomerCategoryService
    {
        Task<List<CustomerCategoryResponse>> Categories();
        Task<CustomerCategoryCRUDResponse> Create(CustomerCategoryRequest request);
        Task<CustomerCategoryCRUDResponse> Delete(BaseId request);
        Task<CustomerCategoryCRUDResponse> BatchDelete(CustomerCategoryBatchDelete request);
        Task<CustomerCategoryCRUDResponse> BatchCreate(List<CustomerCategoryRequest> request);
        Task<CustomerCategoryCRUDResponse> Update(CustomerCategoryRequest request);
        Task<BaseBoolResponse> Exists(BaseStringRequest request);
        Task<BaseBoolResponse> Duplicate(BaseOption request);

    }
}
