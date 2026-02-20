using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.MixnMatches;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.artpixxel.Features.MixnMatches
{
   
    public class MixnMatchController : ApiController
    {
        private readonly  IMixnMatchService _mixnmatch;
        public MixnMatchController(IMixnMatchService mixnmatch)
        {

            _mixnmatch = mixnmatch;
        }

        [HttpPost]
        [Route(nameof(MixnMatches))]
        public async Task<MixnMatchResponse> MixnMatches(PaginationFilter filter)
               => await _mixnmatch.MixnMatches(filter);

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<MixMatchCRUDResponse> Create(MixnMatchRequest request)
            => await _mixnmatch.Create(request);

        [HttpPost]
        [Route(nameof(CreateMultiple))]
        public async Task<MixMatchCRUDResponse> CreateMultiple(MultipleMixnMatchRequest request)
            => await _mixnmatch.CreateMultiple(request);

        [HttpPost]
        [Route(nameof(Delete))]
        public async Task<MixMatchCRUDResponse> Delete(MixMatchDeleteRequest request)
            => await _mixnmatch.Delete(request);

        [HttpPost]
        [Route(nameof(Update))]
        public async Task<MixMatchCRUDResponse> Update(MixnMatchRequest request)
            => await _mixnmatch.Update(request);

        [HttpPost]
        [Route(nameof(BatchDelete))]
        public async Task<MixMatchCRUDResponse> BatchDelete(MixnMatchBatchDelete request)
            => await _mixnmatch.BatchDelete(request);

        [HttpPost]
        [Route(nameof(BatchUpdate))]
        public async Task<MixMatchCRUDResponse> BatchUpdate(MixnMatchBatchUpdate request)
            => await _mixnmatch.BatchUpdate(request);

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(Public))]
        public async Task<MixnMatchModel> Public()
            => await _mixnmatch.Public();
    }
}
