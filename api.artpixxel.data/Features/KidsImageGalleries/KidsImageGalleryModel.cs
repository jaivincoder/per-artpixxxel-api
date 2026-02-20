using api.artpixxel.data.Features.Common;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.KidsImageGalleries
{


    public class BaseKidsGalleryImage
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
    }


    public class PublicKidsGalleryImage : BaseKidsGalleryImage
    {
        public string ImageString { get; set; }
    }


    public class BaseKidseGalleryImageModel : BaseKidsGalleryImage
    {
       
        public string Description { get; set; }
    }


    public class KidsGalleryImageModel : BaseKidseGalleryImageModel
    {
        public bool Selected { get; set; }
    }



    public class KidsImageGalleryCRUDResponse
    {
        public BaseResponse Response { get; set; }
        public List<KidsGalleryImageModel> Images { get; set; }
    }


    public class PublicKidsCharactersResponse
    {
        public List<PublicKidsGalleryImage> Characters { get; set; }
        public BaseResponse Response { get; set; }
    }

    



}


