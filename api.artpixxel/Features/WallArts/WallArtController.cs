
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.WallArts;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.artpixxel.Features.WallArts
{
  
    public class WallArtController : ApiController
    {
        private readonly IWallArtService _wallArtService;
        public WallArtController(IWallArtService wallArtService)
        {
            _wallArtService = wallArtService;
        }

        [HttpPost]
        [Route(nameof(WallArts))]
        public async Task<WallArtResponse> WallArts(PaginationFilter Filter)
            => await _wallArtService.WallArts(Filter);

        [HttpPost]
        [Route(nameof(Setup))]
        public async Task<WallArtSetup> Setup(PaginationFilter Filter)
            => await _wallArtService.Setup(Filter);


        [HttpPost]
        [Route(nameof(Create))]
        public async Task<WallArtCRUDResponse> Create(WallArtCreateRequest request)
            => await _wallArtService.Create(request);

        [HttpPost]
        [Route(nameof(DeleteWallImage))]
        public async Task<WallArtCRUDResponse> DeleteWallImage(PaginatedDeleteRequest request)
            => await _wallArtService.DeleteWallImage(request);

        [HttpPost]
        [Route(nameof(UpdateWallImage))]
        public async Task<WallArtCRUDResponse> UpdateWallImage(WallArtImageRequest request)
            => await _wallArtService.UpdateWallImage(request);

        [HttpPost]
        [Route(nameof(CreateWallImage))]
        public async Task<WallArtCRUDResponse> CreateWallImage(WallArtImageRequest request)
            => await _wallArtService.CreateWallImage(request);

        [HttpPost]
        [Route(nameof(BulkDeleteWallImages))]
        public async Task<WallArtCRUDResponse> BulkDeleteWallImages(BulkWallArtImageDeleteRequest request)
            => await _wallArtService.BulkDeleteWallImages(request);
        [HttpGet]
        [Route(nameof(CategorySize))]
        public async Task<WallArtCategorySize> CategorySize()
            => await _wallArtService.CategorySize();

        [HttpPost]
        [Route(nameof(Update))]
        public async Task<WallArtCRUDResponse> Update(WallArtCreateRequest request)
            => await _wallArtService.Update(request);

        [HttpPost]
        [Route(nameof(Delete))]
        public async Task<WallArtCRUDResponse> Delete(PaginatedDeleteRequest request)
            => await _wallArtService.Delete(request);

        [HttpPost]
        [Route(nameof(BulkUpdate))]
        public async Task<WallArtCRUDResponse> BulkUpdate(BulkWallArtUpdateRequest request)
            => await _wallArtService.BulkUpdate(request);

        [HttpPost]
        [Route(nameof(BulkDelete))]
        public async Task<WallArtCRUDResponse> BulkDelete(BulkWallArtDeleteRequest request)
            => await _wallArtService.BulkDelete(request);

        [HttpPost]
        [Route(nameof(BulkCreate))]
        public async Task<WallArtCRUDResponse> BulkCreate(WallArtBulkCreateRequest request)
            => await _wallArtService.BulkCreate(request);


        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(PublicInit))]
        public async Task<PublicWallartInit> PublicInit(Pagination pagination)
            => await _wallArtService.PublicInit(pagination);

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(PublicFilter))]
        public async Task<PublicWallartInit> PublicFilter(PublicWallArtFilter request)
            => await _wallArtService.PublicFilter(request);

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(PublicSearch))]
        public async Task<PublicWallArtBase> PublicSearch(PublicWallArtFilter request)
            => await _wallArtService.PublicSearch(request);

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(PublicLoadMore))]
        public async Task<PublicWallArtBase> PublicLoadMore(PublicWallArtFilter request)
            => await _wallArtService.PublicLoadMore(request);
    }
}
