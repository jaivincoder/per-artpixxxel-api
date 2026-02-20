using api.artpixxel.data.Features.Common;
using System.Collections.Generic;


namespace api.artpixxel.data.Features.FestiveDesigns
{
   

    public class PublicFestiveDesignModel
    {
        public string Id { get; set; }
        public string Image { get; set; }
        public string ImageString { get; set; } 
        public string Name { get; set; }
        public string Category { get; set; }
    }

    public class PublicFeststiveDesignResponse
    {
        public List<PublicFestiveDesignModel> Designs { get; set; }
        public BaseResponse Response { get; set; }
    }
    public class BaseFestiveDesignModel 
    {
        public string Id { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }

    }


    public class FestiveDesignModel : BaseFestiveDesignModel
    {
        public bool Selected { get; set; }
    }


    public class FestiveDesignCRUDResponse
    {
        public BaseResponse Response { get; set; }
        public List<FestiveDesignModel> Designs { get; set; }
    }
}
