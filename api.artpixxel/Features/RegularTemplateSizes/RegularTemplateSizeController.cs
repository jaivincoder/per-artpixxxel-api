using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.TemplatesSizes;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.Features.RegularTemplateSizes
{
    
    public class RegularTemplateSizeController : ApiController
    {
        private readonly IRegularTemplateSizeService _regularTemplateSizeService;
        public RegularTemplateSizeController(IRegularTemplateSizeService regularTemplateSizeService)
        {
            _regularTemplateSizeService = regularTemplateSizeService;
        }

        [HttpGet]
        [Route(nameof(Sizes))]
        public async Task<TemplateSizCRUDResponse> Sizes(string template)
            => await _regularTemplateSizeService.Sizes(new BaseStringRequest { Name = template });

        [HttpPost]
        [Route(nameof(Exists))]
        public async Task<BaseBoolResponse> Exists(DuplicateTemplateSize request)
            => await _regularTemplateSizeService.Exists(request);

        [HttpPost]
        [Route(nameof(Duplicate))]
        public async Task<BaseBoolResponse> Duplicate(DuplicateTemplateSize request)
            => await _regularTemplateSizeService.Duplicate(request);

        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<BaseResponse> Delete(string id)
            => await _regularTemplateSizeService.Delete(new BaseId { Id = id });

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<TemplateSizCRUDResponse> Update(TemplateSizeModel request)
            => await _regularTemplateSizeService.Update(request);

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<TemplateSizCRUDResponse> Create(TemplateSizeModel request)
            => await _regularTemplateSizeService.Create(request);

        [HttpPost]
        [Route(nameof(BatchCreate))]
        public async Task<TemplateSizCRUDResponse> BatchCreate(List<TemplateSizeModel> request)
            => await _regularTemplateSizeService.BatchCreate(request);

        [HttpGet]
        [Route(nameof(Statistics))]
        public async Task<TemplateStatisticsResponse> Statistics()
           => await _regularTemplateSizeService.Statistics();


    }
}
