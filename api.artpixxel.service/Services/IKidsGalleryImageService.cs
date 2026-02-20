
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.KidsImageGalleries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
    public interface IKidsGalleryImageService
    {
        Task<KidsImageGalleryCRUDResponse> Create(BaseKidseGalleryImageModel request);
        Task<KidsImageGalleryCRUDResponse> BatchCreate(List<BaseKidseGalleryImageModel> request);
        Task<KidsImageGalleryCRUDResponse> Update(BaseKidseGalleryImageModel request);
        Task<BaseResponse> Delete(BaseId request);
        Task<BaseResponse> BatchDelete(BaseId request);
        Task<KidsImageGalleryCRUDResponse> Images();
        Task<PublicKidsCharactersResponse> Characters();
    }
}



       