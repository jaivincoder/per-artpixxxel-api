using api.artpixxel.data.Features.Frames;
using api.artpixxel.data.Models;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
    public interface IFrameService
    {

        Task<FrameCRUDResponse> GetFrameByName(string type, string name);
        Task<FrameGetResponse> GetAll();
        Task<FrameResponse> GetByFrameType(string frameType);
        Task<FrameCRUDResponse> AddFrames(FrameRequestNew request);
        Task<FrameCRUDResponse> Update(string id, FrameRequestNew request);
        Task<FrameCRUDResponse> Delete(FrameDeleteRequest request);
        Task<FrameResponse> GetByCategory(int categoryId);

    }
}