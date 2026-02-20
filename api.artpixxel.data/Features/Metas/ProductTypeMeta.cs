

using System.Collections.Generic;

namespace api.artpixxel.data.Features.Metas
{
    public class ProductTypeMeta : ProductUpdateRequestModel
    {
        public List<ProductTemplateUpdateRequestModel> Templates { get; set; }
    }
}
