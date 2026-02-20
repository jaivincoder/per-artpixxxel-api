
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.WallartCategories;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.Features.WallArtCategories
{
   
    public class WallArtCategoryController : ApiController
    {

        private readonly IWallArtCategoryService _wallArtCategoryService;
        public WallArtCategoryController(IWallArtCategoryService wallArtCategoryService)
        {
            _wallArtCategoryService = wallArtCategoryService;
        }

        [HttpGet]
        [Route(nameof(WallArtCategories))]
        public async Task<List<WallArtCategoryResponse>> WallArtCategories()
            => await _wallArtCategoryService.WallArtCategories();

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<WallArtCategoryCRUDResponse> Create(WallArtCategoryBase request)
            => await _wallArtCategoryService.Create(request);

        [HttpPost]
        [Route(nameof(Duplicate))]
        public async Task<BaseBoolResponse> Duplicate(BaseOption request)
            => await _wallArtCategoryService.Duplicate(request);

        [HttpPost]
        [Route(nameof(Exists))]
        public async Task<BaseBoolResponse> Exists(BaseStringRequest request)
            => await _wallArtCategoryService.Exists(request);

        [HttpPost]
        [Route(nameof(Update))]
        public async Task<WallArtCategoryCRUDResponse> Update(WallArtCategoryBase request)
            => await _wallArtCategoryService.Update(request);

        [HttpPut]
        [Route(nameof(Delete))]
        public async Task<WallArtCategoryCRUDResponse> Delete(BaseId request)
            => await _wallArtCategoryService.Delete(request);

        [HttpPost]
        [Route(nameof(BatchCreate))]
        public async Task<WallArtCategoryCRUDResponse> BatchCreate(List<WallArtCategoryBase> requests)
            => await _wallArtCategoryService.BatchCreate(requests);
    }
}
