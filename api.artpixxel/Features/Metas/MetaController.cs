
using api.artpixxel.data.Features.Metas;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.artpixxel.Features.Prices
{
    
    public class MetaController : ApiController
    {
        private readonly IMetaService _priceService;
        public MetaController(IMetaService priceService)
        {
            _priceService = priceService;
        }

        [HttpPost]
        [Route(nameof(Set))]
        public async Task<MetaResponse> Set(MetaBase request)
            => await _priceService.Set(request);

        [HttpGet]
        [Route(nameof(MetaData))]
        public async Task<MetaModelBase> MetaData()
            => await _priceService.MetaData();


        [HttpPost]
        [Route(nameof(Template))]
        public async Task<ProductMetaResponse> Template(ProductTemplateUpdateRequestModel request)
            => await _priceService.Template(request);


        [HttpPost]
        [Route(nameof(Product))]
        public async Task<ProductMetaResponse> Product(ProductUpdateRequestModel request)
            => await _priceService.Product(request);    



    }
}
