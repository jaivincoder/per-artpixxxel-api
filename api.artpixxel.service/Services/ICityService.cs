

using api.artpixxel.data.Features.Cities;
using api.artpixxel.data.Features.Common;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
  public interface ICityService
    {
        Task<CityBaseInit> Init(Pagination pagination);
        Task<CityInit> Cities(CityFilterData filter);
        Task<CityCRUDResponse> Create(CityWriteRequest request);
        Task<CityCRUDResponse> BatchCreate(MultiCityWriteRequest request);
        Task<CityCRUDResponse> BatchDelete(BatchCityDeleteRequest request);
        Task<CityCRUDResponse> BatchUpdate(BatchCityWriteRequest request);
        Task<CityCRUDResponse> Update(CityWriteRequest request);
        Task<BaseBoolResponse> Exists(FullCity request);
        Task<BaseBoolResponse> Duplicate(FullCity request);
        Task<CityCRUDResponse> Delete(CityDeleteRequest request);



    }
}
