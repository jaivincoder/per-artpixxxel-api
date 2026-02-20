using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.TemplatesSizes;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.Features.ChristmasTemplateSizes
{
    
    public class ChristmasTemplateSizeController : ApiController
    {
        private readonly IChristmasTemplateSizeService _christmasTemplateSizeService;
        public ChristmasTemplateSizeController(IChristmasTemplateSizeService christmasTemplateSizeService)
        {
            _christmasTemplateSizeService = christmasTemplateSizeService;
        }

        [HttpGet]
        [Route(nameof(Sizes))]
        public async Task<TemplateSizCRUDResponse> Sizes(string template)
           => await _christmasTemplateSizeService.Sizes(new BaseStringRequest { Name = template });

        [HttpPost]
        [Route(nameof(Exists))]
        public async Task<BaseBoolResponse> Exists(DuplicateTemplateSize request)
            => await _christmasTemplateSizeService.Exists(request);

        [HttpPost]
        [Route(nameof(Duplicate))]
        public async Task<BaseBoolResponse> Duplicate(DuplicateTemplateSize request)
            => await _christmasTemplateSizeService.Duplicate(request);

        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<BaseResponse> Delete(string id)
            => await _christmasTemplateSizeService.Delete(new BaseId { Id = id });

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<TemplateSizCRUDResponse> Update(TemplateSizeModel request)
            => await _christmasTemplateSizeService.Update(request);

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<TemplateSizCRUDResponse> Create(TemplateSizeModel request)
            => await _christmasTemplateSizeService.Create(request);

        [HttpPost]
        [Route(nameof(BatchCreate))]
        public async Task<TemplateSizCRUDResponse> BatchCreate(List<TemplateSizeModel> request)
            => await _christmasTemplateSizeService.BatchCreate(request);

        [HttpGet]
        [Route(nameof(Statistics))]
        public async Task<TemplateStatisticsResponse> Statistics()
           => await _christmasTemplateSizeService.Statistics();



    }
}
