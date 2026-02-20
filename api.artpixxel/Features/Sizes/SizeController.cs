using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Sizes;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.Features.Sizes
{
    
    public class SizeController : ApiController
    {
        private readonly ISizeService _sizeService;
        public SizeController(ISizeService sizeService)
        {
            _sizeService = sizeService;
        }

        [HttpPost]
        [Route(nameof(BatchCreate))]
        public async Task<SizeCRUDResponse> BatchCreate(List<SizeModel> requests)
            => await _sizeService.BatchCreate(requests);

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<SizeCRUDResponse> Create(SizeModel request)
            => await _sizeService.Create(request);

        [HttpPost]
        [Route(nameof(Duplicate))]
        public async Task<BaseBoolResponse> Duplicate(DuplicateSize request)
            => await _sizeService.Duplicate(request);

        [HttpPost]
        [Route(nameof(Exists))]
        public async Task<BaseBoolResponse> Exists(DuplicateSize request)
            => await _sizeService.Exists(request);

        [HttpGet]
        [Route(nameof(Sizes))]
        public async Task<List<SizeModel>> Sizes()
            => await _sizeService.Sizes();

        [HttpPost]
        [Route(nameof(Update))]
        public async Task<SizeCRUDResponse> Update(SizeModel request)
            => await _sizeService.Update(request);

        [HttpPost]
        [Route(nameof(Delete))]
        public async Task<SizeCRUDResponse> Delete(BaseId request)
            => await _sizeService.Delete(request);
    }
}
