

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.WallartCategories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
    public interface IWallArtCategoryService
    {
        Task<List<WallArtCategoryResponse>> WallArtCategories();
        Task<WallArtCategoryCRUDResponse> Create(WallArtCategoryBase request);
        Task<WallArtCategoryCRUDResponse> BatchCreate(List<WallArtCategoryBase> requests);
        Task<WallArtCategoryCRUDResponse> Update(WallArtCategoryBase request);
        Task<BaseBoolResponse> Exists(BaseStringRequest request);
        Task<BaseBoolResponse> Duplicate(BaseOption request);
        Task<WallArtCategoryCRUDResponse> Delete(BaseId request);



    }
}
