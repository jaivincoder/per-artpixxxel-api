
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.States;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
   public interface IStateService
    {
        Task<StateBaseInit> Init(Pagination pagination);
        Task<StateInit> States(StateFilterData filter);
        Task<StateCRUDResponse> Create(StateWriteRequest request);
        Task<StateCRUDResponse> BatchUpdate(BatchStateWriteRequest request);
        Task<StateCRUDResponse> Delete(StateDeleteRequest request);
        Task<StateCRUDResponse> BatchDelete(BatchStateDeleteRequest request);
        Task<StateCRUDResponse> BulkCreate(MultiStateWriteRequest request);
        Task<StateCRUDResponse> Update(StateWriteRequest request);
        Task<BaseBoolResponse> Exists(BaseOption request);
        Task<BaseBoolResponse> Duplicate(FullState request);

    }
}
