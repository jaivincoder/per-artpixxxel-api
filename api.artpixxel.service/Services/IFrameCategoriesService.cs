using api.artpixxel.data.Features.Cities;
using api.artpixxel.data.Features.FrameCategories;
using api.artpixxel.data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
    public interface IFrameCategoriesService
    {
      
        Task<GetFrameCategoryListResponse> GetFrameCategories();
        Task<GetFrameCategoryResponse> GetFrameCategoryById(int id);
        Task<FrameCategoryCRUDResponse> AddFrameCategories(FrameCategoryResponseDto request);
        Task<FrameCategoryCRUDResponse> UpdateFrameCategories(FrameCategoryResponseDto request);
        Task<FrameCategoryCRUDResponse> DeleteFrameCategory(int id);
    }
}
