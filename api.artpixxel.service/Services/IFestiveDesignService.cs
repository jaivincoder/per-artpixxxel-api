using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.FestiveDesigns;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
    public interface IFestiveDesignService
    {
        Task<FestiveDesignCRUDResponse> Create(BaseFestiveDesignModel request);
        Task<FestiveDesignCRUDResponse> BatchCreate(List<BaseFestiveDesignModel> request);
        Task<FestiveDesignCRUDResponse> Update(BaseFestiveDesignModel request);
        Task<BaseResponse> Delete(BaseId request);
        Task<BaseResponse> BatchDelete(BaseId request);
        Task<FestiveDesignCRUDResponse> Designs();
        Task<PublicFeststiveDesignResponse> Public();
    }
}
