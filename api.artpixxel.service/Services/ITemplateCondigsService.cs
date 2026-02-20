using api.artpixxel.data.Features.TemplateConfing;
using api.artpixxel.data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
    public interface ITemplateCondigsService
    {
        Task<GetTemplateConfingListResponse> GetTemplatesAsync();
        Task<GetTemplateConfingResponse> GetTemplatesById(int id);
        Task<GetTemplateConfingResponse> GetTemplatesByTemplateKey(string TemplateKey);
        Task<TemplateConfligCRUDResponse> AddTemplateConfigs(TemplateConfingModel request);
        Task<TemplateConfligCRUDResponse> UpdateTemplateConfigs(int id, TemplateConfingModel request);
        Task<GetTemplateConfingResponse> DeleteTemplateConfigs(int id);
        Task<GetTemplateConfingListResponse> GetTemplatesByCategoryId(int CategoryId);
    }
}
