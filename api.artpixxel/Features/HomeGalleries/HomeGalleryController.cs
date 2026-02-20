using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.HomeGalleries;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.Features.HomeGalleries
{
    
    public class HomeGalleryController : ApiController
    {
        private readonly IHomeGalleryService _homeGalleryService;

        public HomeGalleryController(IHomeGalleryService homeGalleryService)
        {
            _homeGalleryService = homeGalleryService;
        }

        [HttpPost]
        [Route(nameof(BatchCreate))]
        public async Task<HomeGalleryCRUDResponse> BatchCreate(List<BaseHomeGalleryImageModel> requests)
            => await _homeGalleryService.BatchCreate(requests);


        [HttpDelete]
        [Route(nameof(BatchDelete))]
        public async Task<BaseResponse> BatchDelete(string ids)
            => await _homeGalleryService.BatchDelete(new BaseId { Id = ids });

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<HomeGalleryCRUDResponse> Create(BaseHomeGalleryImageModel request)
            => await _homeGalleryService.Create(request);

        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<BaseResponse> Delete(string id)
            => await _homeGalleryService.Delete(new BaseId { Id = id });

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<HomeGalleryCRUDResponse> Update(BaseHomeGalleryImageModel request)
            => await _homeGalleryService.Update(request);

        [HttpGet]
        [Route(nameof(Images))]
        public async Task<HomeGalleryCRUDResponse> Images()
            => await _homeGalleryService.Images();  



    }
}
