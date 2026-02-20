

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.WallArtSizes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
   public interface IWallArtSizeService
    {
        Task<List<WallArtSizeResponse>> WallArtSizes();
        Task<List<BaseOption>> WallArtSizeOptions();
        Task<WallArtSizeCRUDResponse> Create(WallArtSizeBase request);
        Task<WallArtSizeCRUDResponse> BatchCreate(List<WallArtSizeBase> request);
        Task<WallArtSizeCRUDResponse> Update(WallArtSizeBase request);
        Task<WallArtSizeCRUDResponse> Delete(BaseId request);
        Task<BaseBoolResponse> Exists(BaseStringRequest request);
        Task<BaseBoolResponse> Duplicate(BaseOption request);
        Task<BaseBoolResponse> HasDefault();


    }
}
