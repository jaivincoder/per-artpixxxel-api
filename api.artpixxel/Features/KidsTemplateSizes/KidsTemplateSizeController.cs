using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.TemplatesSizes;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.Features.KidsTemplateSizes
{
   
    public class KidsTemplateSizeController : ApiController
    {
        private readonly IKidsTemplateSizeService _kidsTemplateSizeService;
        public KidsTemplateSizeController(IKidsTemplateSizeService kidsTemplateSizeService)
        {
            _kidsTemplateSizeService = kidsTemplateSizeService;
        }

        [HttpPost]
        [Route(nameof(BatchCreate))]
        public async Task<TemplateSizCRUDResponse> BatchCreate(List<TemplateSizeModel> request)
            => await _kidsTemplateSizeService.BatchCreate(request);

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<TemplateSizCRUDResponse> Create(TemplateSizeModel request)
            => await _kidsTemplateSizeService.Create(request);

        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<BaseResponse> Delete(string id)
           => await _kidsTemplateSizeService.Delete(new BaseId { Id = id });

        [HttpPost]
        [Route(nameof(Duplicate))]
        public async Task<BaseBoolResponse> Duplicate(DuplicateTemplateSize request)
            => await _kidsTemplateSizeService.Duplicate(request);

        [HttpPost]
        [Route(nameof(Exists))] 
        public async Task<BaseBoolResponse> Exists(DuplicateTemplateSize request) => await _kidsTemplateSizeService.Exists(request);

        [HttpGet]
        [Route(nameof(Sizes))]
        public async Task<TemplateSizCRUDResponse> Sizes(string template)
            => await _kidsTemplateSizeService.Sizes(new BaseStringRequest { Name = template });

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<TemplateSizCRUDResponse> Update(TemplateSizeModel request)
            => await _kidsTemplateSizeService.Update(request);  

        [HttpGet]
        [Route(nameof(Statistics))]
        public async Task<TemplateStatisticsResponse> Statistics()
            => await _kidsTemplateSizeService.Statistics(); 
    }
}
