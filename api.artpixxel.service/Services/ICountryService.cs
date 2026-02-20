

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Countries;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
    public interface ICountryService
    {
        Task<CountryInit> Init();
        Task<CountryCRUDResponse> Create(CountryWriteRequest request);
        Task<CountryCRUDResponse> BulkCreate(MultiCountryWriteRequest request);
  
        Task<CountryCRUDResponse> Update(CountryWriteRequest request);
        Task<CountryCRUDResponse> Delete(CountryDeleteRequest request);
        Task<BaseBoolResponse> Exists(BaseStringRequest request);
        Task<BaseBoolResponse> Duplicate(BaseOption request);
    }
}
