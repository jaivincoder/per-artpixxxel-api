using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Countries;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.artpixxel.Features.Countries
{
   
    public class CountryController : ApiController
    {
        private readonly ICountryService _countryService;
        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        [Route(nameof(Init))]
        public async Task<CountryInit> Init()
            => await _countryService.Init();

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<CountryCRUDResponse> Create(CountryWriteRequest request)
            => await _countryService.Create(request);

        [HttpPost]
        [Route(nameof(BulkCreate))]
        public async Task<CountryCRUDResponse> BulkCreate(MultiCountryWriteRequest request)
            => await _countryService.BulkCreate(request);

        [HttpPost]
        [Route(nameof(Delete))]
        public async Task<CountryCRUDResponse> Delete(CountryDeleteRequest request)
            => await _countryService.Delete(request);

        [HttpPost]
        [Route(nameof(Update))]
        public async Task<CountryCRUDResponse> Update(CountryWriteRequest request)
            => await _countryService.Update(request);

        [HttpPost]
        [Route(nameof(Duplicate))]
        public async Task<BaseBoolResponse> Duplicate(BaseOption request)
            => await _countryService.Duplicate(request);
        [HttpPost]
        [Route(nameof(Exists))]
        public async Task<BaseBoolResponse> Exists(BaseStringRequest request)
            => await _countryService.Exists(request);
    }
}
