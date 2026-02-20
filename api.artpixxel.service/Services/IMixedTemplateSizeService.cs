

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.TemplatesSizes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
    public interface IMixedTemplateSizeService
    {
        Task<TemplateSizCRUDResponse> Create(TemplateSizeModel request);
        Task<TemplateSizCRUDResponse> Update(TemplateSizeModel request);
        Task<BaseResponse> Delete(BaseId request);
        Task<TemplateSizCRUDResponse> BatchCreate(List<TemplateSizeModel> request);
        Task<BaseBoolResponse> Exists(DuplicateTemplateSize request);
        Task<BaseBoolResponse> Duplicate(DuplicateTemplateSize request);
        Task<TemplateSizCRUDResponse> Sizes(BaseStringRequest request);
        Task<TemplateStatisticsResponse> Statistics();


       
      
    }
}
