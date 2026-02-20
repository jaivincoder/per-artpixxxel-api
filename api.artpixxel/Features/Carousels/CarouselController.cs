using api.artpixxel.data.Features.Carousels;
using api.artpixxel.data.Features.Common;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.artpixxel.Features.Carousels
{
   
    public class CarouselController : ApiController
    {
        private readonly ICarouselService _carouselService;
        public CarouselController(ICarouselService carouselService)
        {
            _carouselService = carouselService;
        }

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<CarouselCRUDResponse> Create(CarouselRequest request)
            => await _carouselService.Create(request);

        [HttpPost]
        [Route(nameof(BatchCreate))]
        public async Task<CarouselCRUDResponse> BatchCreate(BatchCarouselRequest request)
            => await _carouselService.BatchCreate(request);

        [HttpPost]
        [Route(nameof(Update))]
        public async Task<CarouselCRUDResponse>Update(CarouselRequest request)
            => await _carouselService.Update(request);

        [HttpPost]
        [Route(nameof(Carousels))]
        public async Task<CarouselResponse> Carousels(Pagination pagination)
            => await _carouselService.Carousels(pagination);

        [HttpPost]
        [Route(nameof(Delete))]
        public async Task<CarouselCRUDResponse> Delete(CarouselDeleteRequest request)
            => await _carouselService.Delete(request);

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(PublicCarousels))]
        public async Task<CarouselResponse> PublicCarousels()
            => await _carouselService.PublicCarousels();

        [HttpPost]
        [Route(nameof(CarouselsPaginated))]
        public async Task<CarouselResponse> CarouselsPaginated(Filter filter)
            => await _carouselService.CarouselsPaginated(filter);
    }
}
