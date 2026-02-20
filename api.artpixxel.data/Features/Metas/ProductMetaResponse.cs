
using api.artpixxel.data.Features.Common;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.Metas
{
    public class ProductMetaResponse
    {
       public List<ProductTypeMeta> Products { get; set; }
        public BaseResponse Response { get; set; }
    }
}
