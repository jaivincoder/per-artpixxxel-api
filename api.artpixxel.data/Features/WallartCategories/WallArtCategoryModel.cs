
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.WallArts;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.WallartCategories
{
   public class WallArtCategoryBase
    {
      public string WallartCategoryId { get; set; }
      public string WallartCategoryName { get; set; }
      public string WallartCategoryImage { get; set; }
      public string WallartCategoryDescription { get; set; }
     


    }

    public class WallArtCategoryResponse : WallArtCategoryBase
    {

        public decimal WallartCategoryWallartCount { get; set; }
    }


    public class WallArtCategoryOption
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal WallArtCount { get; set; }
    }

    public class WallArtCategoryCRUDResponse
    {
        public List<WallArtCategoryResponse> WallArtCategories { get; set; }
        public BaseResponse Response { get; set; }
    }


    public class PublicWallArtCategory
    {
        public string Label { get; set; }
        public string Image { get; set; }
    }


}
