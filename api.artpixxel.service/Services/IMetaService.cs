

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Metas;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
  public  interface IMetaService
    {
        Task<MetaResponse> Set(MetaBase request);
        Task<MetaModelBase> MetaData();
        Task<ProductMetaResponse> Template(ProductTemplateUpdateRequestModel request);
        Task<ProductMetaResponse> Product(ProductUpdateRequestModel request);
       
    }
}
