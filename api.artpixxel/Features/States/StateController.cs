using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.States;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.artpixxel.Features.States
{
   
    public class StateController : ApiController
    {
        private readonly IStateService _stateService;
        public StateController(IStateService stateService)
        {
            _stateService = stateService;
        }

        [HttpPost]
        [Route(nameof(Init))]
        public async Task<StateBaseInit> Init(Pagination pagination)
            => await _stateService.Init(pagination);


        [HttpPost]
        [Route(nameof(States))]
        public async Task<StateInit> States(StateFilterData filter)
            => await _stateService.States(filter);

        [HttpPost]
        [Route(nameof(Duplicate))]
        public async Task<BaseBoolResponse> Duplicate(FullState request)
            => await _stateService.Duplicate(request);


        [HttpPost]
        [Route(nameof(Exists))]
        public async Task<BaseBoolResponse> Exists(BaseOption request)
            => await _stateService.Exists(request);

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<StateCRUDResponse> Create(StateWriteRequest request)
            => await _stateService.Create(request);

        [HttpPost]
        [Route(nameof(Update))]
        public async Task<StateCRUDResponse> Update(StateWriteRequest request)
            => await _stateService.Update(request);

        [HttpPost]
        [Route(nameof(BulkCreate))]
        public async Task<StateCRUDResponse> BulkCreate(MultiStateWriteRequest request)
            => await _stateService.BulkCreate(request);

        [HttpPost]
        [Route(nameof(Delete))]
        public async Task<StateCRUDResponse> Delete(StateDeleteRequest request)
            => await _stateService.Delete(request);

        [HttpPost]
        [Route(nameof(BatchUpdate))]
        public async Task<StateCRUDResponse> BatchUpdate(BatchStateWriteRequest request)
            => await _stateService.BatchUpdate(request);

        [HttpPost]
        [Route(nameof(BatchDelete))]
        public async Task<StateCRUDResponse> BatchDelete(BatchStateDeleteRequest request)
            => await _stateService.BatchDelete(request);
    }
}
