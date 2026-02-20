using api.artpixxel.data.Features.Cities;
using api.artpixxel.data.Features.Common;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.artpixxel.Features.Cities
{
    
    public class CityController : ApiController
    {
        private readonly ICityService _cityService;
        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpPost]
        [Route(nameof(Init))]
        public async Task<CityBaseInit> Init(Pagination pagination)
            => await _cityService.Init(pagination);

        [HttpPost]
        [Route(nameof(Cities))]
        public async Task<CityInit> Cities(CityFilterData filter)
             => await _cityService.Cities(filter);

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<CityCRUDResponse> Create(CityWriteRequest request)
            => await _cityService.Create(request);
        [HttpPost]
        [Route(nameof(Update))]
        public async Task<CityCRUDResponse> Update(CityWriteRequest request)
            => await _cityService.Update(request);


        [HttpPost]
        [Route(nameof(Exists))]
        public async Task<BaseBoolResponse> Exists(FullCity request)
            => await _cityService.Exists(request);

        [HttpPost]
        [Route(nameof(Duplicate))]
        public async Task<BaseBoolResponse> Duplicate(FullCity request)
            => await _cityService.Duplicate(request);

        [HttpPost]
        [Route(nameof(Delete))]
        public async Task<CityCRUDResponse> Delete(CityDeleteRequest request)
            => await _cityService.Delete(request);

        [HttpPost]
        [Route(nameof(BatchCreate))]
        public async Task<CityCRUDResponse> BatchCreate(MultiCityWriteRequest request)
            => await _cityService.BatchCreate(request);

        [HttpPost]
        [Route(nameof(BatchDelete))]
        public async Task<CityCRUDResponse> BatchDelete(BatchCityDeleteRequest request)
            => await _cityService.BatchDelete(request);

        [HttpPost]
        [Route(nameof(BatchUpdate))]
        public async Task<CityCRUDResponse> BatchUpdate(BatchCityWriteRequest request)
            => await _cityService.BatchUpdate(request);


    }
}
