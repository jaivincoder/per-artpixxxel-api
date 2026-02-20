

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.WallartCategories;
using api.artpixxel.data.Features.WallArtSizes;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.WallArts
{
    public class WallArtBase
    {
       public string WallartId { get; set; }
     
       public string WallartHeader { get; set; }
       public string  WallartCategoryCategoryId { get; set; }
       public string WallartSizeSizeId { get; set; }
       public bool WallartFixedSize { get; set; }
       public string WallartImage { get; set; }
       public string WallartDescription { get; set; }
    }

    public class WallArtModel : WallArtBase
    {
       public string  WallartCategoryCategoryName { get; set; }
       public decimal WallartAmount { get; set; }
        public int WallartRating { get; set; }
       public string WallartSizeSizeName { get; set; }
       public string WallartImageURL { get; set; }
       public decimal WallartImagesCount { get; set; }
       public  List<WallArtImageResponse> WallartImages { get; set; }
    }

    public class WallArtImageBase
    {
        public string WallartImageId { get; set; }
        public string WallartImageImage { get; set; }
        public string WallartImageName { get; set; }
        public string WallartImageDescription { get; set; }
        public string WallartImageWallArtId { get; set; }
    }

    public class WallArtImageResponse : WallArtImageBase
    {
      
       public string WallartImageImageURL { get; set; }
      
    }


    public class WallArtResponse
    {
        public List<WallArtModel> WallArts { get; set; }
        public decimal TotalCount { get; set; }
    }


    public class WallArtCRUDResponse
    {
        public WallArtResponse WallArt { get; set; }
        public BaseResponse Response { get; set; }
    }

    public class WallArtCategorySize
    {
        public List<WallArtSizeOption> Sizes { get; set; }
        public List<WallArtCategoryOption> Categories { get; set; }
    }
    public class WallArtSetup : WallArtCategorySize
    {
        public WallArtResponse WallArts { get; set; }
       
    }

    public class WallArtRequest : WallArtBase
    {
        public List<WallArtImageBase> WallartImages { get; set; }
    }


  

    public class WallArtImageRequest
    {
        public WallArtImageBase WallArtImage { get; set; }
        public PaginationFilter PaginationFilter { get; set; }
    }

    public class WallArtCreateRequest
    {
        public WallArtRequest WallArt { get; set; }
        public PaginationFilter PaginationFilter { get; set; }

    }


    public class WallArtBulkCreateRequest
    {
        public List<WallArtRequest> WallArts { get; set; }
        public PaginationFilter PaginationFilter { get; set; }

    }

    public class BulkWallArtImageDeleteRequest
    {
        public List<string> WallImageIds { get; set; }
        public PaginationFilter PaginationFilter { get; set; }

    }

    public class WallArtBulkUpdate
    {
       public string  CategoryId { get; set; }
       public bool? FixedSize { get; set; }
       public string SizeId { get; set; }
       public string  Description { get; set; }
    }

    public class BulkWallArtDeleteRequest
    {
        public List<string> WallartIds { get; set; }
        public PaginationFilter PaginationFilter { get; set; }

    }

    public class BulkWallArtUpdateRequest : BulkWallArtDeleteRequest
    {
        public WallArtBulkUpdate Value { get; set; }
    }



    public class PublicWallArt
    {
       public string  Id { get; set; }
       public string Key { get; set; }
       public int  Quantity { get; set; }
       public decimal Amount { get; set; }
       public decimal TotalAmount { get; set; }
       public string Category { get; set; }
       public int Rating { get; set; }
       public string Size { get; set; }
       public bool Favourite { get; set; }
       public string FrameClass { get; set; }
       public string Header { get; set; }
       public string Image { get; set; }
       public bool FixedSize { get; set; }
       public string Description { get; set; }
       public List<ImageModel> Images { get; set; }
    }


    public class PublicWallArtBase
    {
        public List<PublicWallArt> WallArts { get; set; }
    }

    public class PublicWallartInit : PublicWallArtBase
    {
        public List<PublicWallArtSize> Sizes { get; set; }
        public List<PublicWallArtCategory> Categories { get; set; }

    }


    public class PublicWallArtFilter
    {
       public string Hashtag { get; set; }
       public string Search { get; set; }
       public string Category { get; set; }
       public Pagination Pagination { get; set; }
    }

}
