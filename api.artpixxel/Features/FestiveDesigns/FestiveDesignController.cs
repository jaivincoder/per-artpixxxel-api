using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.FestiveDesigns;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.Features.FestiveDesigns
{
    
    public class FestiveDesignController : ApiController
    {
        private readonly IFestiveDesignService _festiveDesignService;
        public FestiveDesignController(IFestiveDesignService festiveDesignService)
        {
            _festiveDesignService = festiveDesignService;
        }


        [HttpPost]
        [Route(nameof(BatchCreate))]
        public async Task<FestiveDesignCRUDResponse> BatchCreate(List<BaseFestiveDesignModel> requests)
            => await _festiveDesignService.BatchCreate(requests);

        [HttpDelete]
        [Route(nameof(BatchDelete))]
        public async Task<BaseResponse> BatchDelete(string ids)
            => await _festiveDesignService.BatchDelete(new BaseId { Id = ids });

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<FestiveDesignCRUDResponse> Create(BaseFestiveDesignModel request)
            => await _festiveDesignService.Create(request);


        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<BaseResponse> Delete(string id)
            => await _festiveDesignService.Delete(new BaseId { Id = id });

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<FestiveDesignCRUDResponse> Update(BaseFestiveDesignModel request)
            => await _festiveDesignService.Update(request);

        [HttpGet]
        [Route(nameof(Designs))]
        public async Task<FestiveDesignCRUDResponse> Designs()
            => await _festiveDesignService.Designs();


        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(Public))]
        public async Task<PublicFeststiveDesignResponse> Public()
            => await _festiveDesignService.Public();


    }
}
