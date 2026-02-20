

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.MixnMatches;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
   public interface IMixnMatchService
    {
        Task<MixnMatchResponse> MixnMatches(PaginationFilter Filter);
        Task<MixMatchCRUDResponse> Create(MixnMatchRequest request);
        Task<MixMatchCRUDResponse> CreateMultiple(MultipleMixnMatchRequest request);
        Task<MixMatchCRUDResponse> Update(MixnMatchRequest request);
        Task<MixMatchCRUDResponse> Delete(MixMatchDeleteRequest request);
        Task<MixMatchCRUDResponse> BatchDelete(MixnMatchBatchDelete request);
        Task<MixMatchCRUDResponse> BatchUpdate(MixnMatchBatchUpdate request);
        Task<MixnMatchModel> Public();
        


    }
}
