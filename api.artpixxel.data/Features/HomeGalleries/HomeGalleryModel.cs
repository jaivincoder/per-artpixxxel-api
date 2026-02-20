

using api.artpixxel.data.Features.Common;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.HomeGalleries
{

    public class PublicHomeGalleryModel
    {
        public string Id { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
    public class BaseHomeGalleryImageModel
    {
       public string Id { get; set; }
       public string Image { get; set; }
       public string Name { get; set; }
       public bool Active { get; set; }
       public string Type { get; set; } 
       public string Description { get; set; }

    }


    public class HomeGalleryImageModel : BaseHomeGalleryImageModel
    {
        public bool Selected { get; set; }
    }


    public class HomeGalleryCRUDResponse
    {
        public BaseResponse Response { get; set; }
        public List<HomeGalleryImageModel> Images { get; set; }
    }


}
