

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.MixnMatchCategories;
using api.artpixxel.data.Features.MixnMatches;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
   public interface IMixnMatchCategoryService
    {
        
        Task<MixNMatchCategoryCRUDResponse> Create(MixnMatchCategoryRequest request);
        Task<MixnMatchCategoryCRUD> CreateCategory(MixnMatchCategoryRequest request);
        Task<MixnMatchCategoryCRUD> CreateMultipleCategories(List<MixnMatchCategoryRequest> request);
        Task<MixnMatchCategoryCRUD> Update(MixnMatchCategoryRequest request);
        Task<MixNMatchCategoryCRUDResponse> CreateMultiple(List<MixnMatchCategoryRequest> request);
        Task<List<BaseOption>> Categories();
        Task<BaseBoolResponse> Exists(BaseStringRequest request);
        Task<BaseBoolResponse> Duplicate(BaseOption request);
        Task<List<MixnMatchCategoryBase>> MixnMatchCategories();
        Task<MixnMatchCategoryCRUD> Delete(BaseId request);
        Task<MixnMatchResponse> CategoryMixnMatches(PaginationFilter PaginationFilter);
    }
}
