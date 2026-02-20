
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.MixnMatchCategories;
using api.artpixxel.data.Features.MixnMatches;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.Features.MixnMatchCategories
{
    
    public class MixnMatchCategoryController : ApiController
    {
        private readonly IMixnMatchCategoryService _mixnMatchCategoryService;
        public MixnMatchCategoryController(IMixnMatchCategoryService mixnMatchCategoryService)
        {
            _mixnMatchCategoryService = mixnMatchCategoryService;
        }

        [HttpGet]
        [Route(nameof(Categories))]
        public async Task<List<BaseOption>> Categories()
            => await _mixnMatchCategoryService.Categories();

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<MixNMatchCategoryCRUDResponse> Create(MixnMatchCategoryRequest request)
            => await _mixnMatchCategoryService.Create(request);

        [HttpPost]
        [Route(nameof(CreateCategory))]
        public async Task<MixnMatchCategoryCRUD> CreateCategory(MixnMatchCategoryRequest request)
            => await _mixnMatchCategoryService.CreateCategory(request);

        [HttpPost]
        [Route(nameof(Exists))]
        public async Task<BaseBoolResponse> Exists(BaseStringRequest request)
            => await _mixnMatchCategoryService.Exists(request);

        [HttpPost]
        [Route(nameof(Duplicate))]
        public async Task<BaseBoolResponse> Duplicate(BaseOption request)
            => await _mixnMatchCategoryService.Duplicate(request);

        [HttpPost]
        [Route(nameof(CreateMultiple))]
        public async Task<MixNMatchCategoryCRUDResponse> CreateMultiple(List<MixnMatchCategoryRequest> requests)
            => await _mixnMatchCategoryService.CreateMultiple(requests);

        [HttpGet]
        [Route(nameof(MixnMatchCategories))]
        public async Task<List<MixnMatchCategoryBase>> MixnMatchCategories()
            => await _mixnMatchCategoryService.MixnMatchCategories();

        [HttpPost]
        [Route(nameof(Delete))]
        public async Task<MixnMatchCategoryCRUD> Delete(BaseId request)
            => await _mixnMatchCategoryService.Delete(request);

        [HttpPost]
        [Route(nameof(Update))]
        public async Task<MixnMatchCategoryCRUD> Update(MixnMatchCategoryRequest request)
            => await _mixnMatchCategoryService.Update(request);

        [HttpPost]
        [Route(nameof(CreateMultipleCategories))]
        public async Task<MixnMatchCategoryCRUD> CreateMultipleCategories(List<MixnMatchCategoryRequest> requests)
            => await _mixnMatchCategoryService.CreateMultipleCategories(requests);

        [HttpPost]
        [Route(nameof(CategoryMixnMatches))]
        public async Task<MixnMatchResponse> CategoryMixnMatches(PaginationFilter paginationFilter)
            => await _mixnMatchCategoryService.CategoryMixnMatches(paginationFilter);

       
    }
}
