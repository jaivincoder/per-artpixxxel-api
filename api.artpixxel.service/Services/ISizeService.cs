using api.artpixxel.data.Features.Sizes;
using System.Threading.Tasks;
using System.Collections.Generic;
using api.artpixxel.data.Features.Common;

namespace api.artpixxel.service.Services
{
   public interface ISizeService
    {
        Task<SizeCRUDResponse> Create(SizeModel request);
        Task<SizeCRUDResponse> Update(SizeModel request);
        Task<SizeCRUDResponse> Delete(BaseId request);
        Task<SizeCRUDResponse> BatchCreate(List<SizeModel> request);
        Task<BaseBoolResponse> Exists(DuplicateSize request);
        Task<BaseBoolResponse> Duplicate(DuplicateSize request);
        Task<List<SizeModel>> Sizes();
        

    }
}
