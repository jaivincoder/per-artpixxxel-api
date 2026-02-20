

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.HomeGalleries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
    public interface IHomeGalleryService
    {
        Task<HomeGalleryCRUDResponse> Create(BaseHomeGalleryImageModel request);
        Task<HomeGalleryCRUDResponse> BatchCreate(List<BaseHomeGalleryImageModel> request);
        Task<HomeGalleryCRUDResponse> Update(BaseHomeGalleryImageModel request);
        Task<BaseResponse> Delete(BaseId request);
        Task<BaseResponse> BatchDelete(BaseId request);
        Task<HomeGalleryCRUDResponse> Images();
    }
}
