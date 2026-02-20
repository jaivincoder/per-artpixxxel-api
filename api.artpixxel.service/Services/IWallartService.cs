
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.WallArts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
  public  interface IWallArtService
    {
        Task<WallArtResponse> WallArts(PaginationFilter Filter);
        Task<WallArtSetup> Setup(PaginationFilter Filter);
        Task<WallArtCategorySize> CategorySize();
        Task<WallArtCRUDResponse> Create(WallArtCreateRequest request);
        Task<WallArtCRUDResponse> BulkCreate(WallArtBulkCreateRequest request);
        Task<WallArtCRUDResponse> Update(WallArtCreateRequest request);
        Task<WallArtCRUDResponse> Delete(PaginatedDeleteRequest request);
        Task<WallArtCRUDResponse> DeleteWallImage(PaginatedDeleteRequest request);
        Task<WallArtCRUDResponse> UpdateWallImage(WallArtImageRequest request);
        Task<WallArtCRUDResponse> CreateWallImage(WallArtImageRequest request);
        Task<WallArtCRUDResponse> BulkDeleteWallImages(BulkWallArtImageDeleteRequest request);
        Task<WallArtCRUDResponse> BulkUpdate(BulkWallArtUpdateRequest request);
        Task<WallArtCRUDResponse> BulkDelete(BulkWallArtDeleteRequest request);
        Task<PublicWallartInit> PublicInit(Pagination pagination);
        Task<PublicWallartInit> PublicFilter(PublicWallArtFilter request);
        Task<PublicWallArtBase> PublicSearch(PublicWallArtFilter request);
        Task<PublicWallArtBase>PublicLoadMore(PublicWallArtFilter request);

    }
}
