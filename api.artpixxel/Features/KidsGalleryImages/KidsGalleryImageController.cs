using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.KidsImageGalleries;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.Features.KidsGalleryImages
{
   
    public class KidsGalleryImageController : ApiController
    {
        private readonly IKidsGalleryImageService _kidsGalleryService; 
        public KidsGalleryImageController(IKidsGalleryImageService kidsGalleryService )
        {
            _kidsGalleryService = kidsGalleryService;
        }

        [HttpPost]
        [Route(nameof(BatchCreate))]
        public async Task<KidsImageGalleryCRUDResponse> BatchCreate(List<BaseKidseGalleryImageModel> requests)
           => await _kidsGalleryService.BatchCreate(requests);

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<KidsImageGalleryCRUDResponse> Create(BaseKidseGalleryImageModel request)
            => await _kidsGalleryService.Create(request);

        [HttpGet]
        [Route(nameof(Images))]
        public async Task<KidsImageGalleryCRUDResponse> Images()
            => await _kidsGalleryService.Images();

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<KidsImageGalleryCRUDResponse> Update(BaseKidseGalleryImageModel request)
            => await _kidsGalleryService.Update(request);

        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<BaseResponse> Delete(string id)
            => await _kidsGalleryService.Delete(new BaseId { Id = id});


        [HttpDelete]
        [Route(nameof(BatchDelete))]
        public async Task<BaseResponse> BatchDelete(string ids)
            => await _kidsGalleryService.BatchDelete(new BaseId { Id = ids });

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(Characters))]
        public async Task<PublicKidsCharactersResponse> Characters()
            => await _kidsGalleryService.Characters();  





    }
}
