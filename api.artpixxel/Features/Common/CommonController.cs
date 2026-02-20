using api.artpixxel.data.Features.Common;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.Features.Common
{
    
    public class CommonController : ApiController
    {
        private readonly ICommonService _commonService;
        public CommonController(ICommonService commonService)
        {
            _commonService = commonService;   
        }

       
        [HttpGet]
        [Route(nameof(Countries))]
        public async Task<List<CountryOption>> Countries()
            => await _commonService.Countries();

       
        [HttpPost]
        [Route(nameof(Cities))]
        public async Task<List<BaseOption>> Cities(BaseId request)
            => await _commonService.Cities(request);

       
        [HttpPost]
        [Route(nameof(States))]
        public async Task<List<BaseOption>> States(BaseId request)
            => await _commonService.States(request);

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(LocationInfo))]
        public async Task<LocationModel> LocationInfo()
            => await _commonService.LocationInfo();

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(ShoppingInfo))]
        public async Task<ShoppingInfo> ShoppingInfo()
            => await _commonService.ShoppingInfo();
    }
}
