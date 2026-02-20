
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.WallArtSizes;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.Features.WallArtSizes
{
   
    public class WallArtSizeController : ApiController
    {
        private readonly IWallArtSizeService _wallArtSizeService;
        public WallArtSizeController(IWallArtSizeService wallArtSizeService)
        {
            _wallArtSizeService = wallArtSizeService;
        }

        [HttpGet]
        [Route(nameof(WallArtSizeOptions))]
        public async Task<List<BaseOption>> WallArtSizeOptions()
            => await _wallArtSizeService.WallArtSizeOptions();

        [HttpGet]
        [Route(nameof(WallArtSizes))]
        public async Task<List<WallArtSizeResponse>> WallArtSizes()
            => await _wallArtSizeService.WallArtSizes();

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<WallArtSizeCRUDResponse> Create(WallArtSizeBase request)
            => await _wallArtSizeService.Create(request);

        [HttpPost]
        [Route(nameof(BatchCreate))]
        public async Task<WallArtSizeCRUDResponse> BatchCreate(List<WallArtSizeBase> request)
            => await _wallArtSizeService.BatchCreate(request);

        [HttpPost]
        [Route(nameof(Duplicate))]
        public async Task<BaseBoolResponse> Duplicate(BaseOption request)
            => await _wallArtSizeService.Duplicate(request);

        [HttpPost]
        [Route(nameof(Exists))]
        public async Task<BaseBoolResponse> Exists(BaseStringRequest request)
            => await _wallArtSizeService.Exists(request);

        [HttpPost]
        [Route(nameof(Update))]
        public async Task<WallArtSizeCRUDResponse> Update(WallArtSizeBase request)
            => await _wallArtSizeService.Update(request);

        [HttpPost]
        [Route(nameof(Delete))]
        public async Task<WallArtSizeCRUDResponse> Delete(BaseId request)
            => await _wallArtSizeService.Delete(request);

        [HttpGet]
        [Route(nameof(HasDefault))]
        public async Task<BaseBoolResponse> HasDefault()
            => await _wallArtSizeService.HasDefault();
    }
}
