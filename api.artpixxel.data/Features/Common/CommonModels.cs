

using api.artpixxel.data.Features.Cities;
using api.artpixxel.data.Features.HomeGalleries;
using api.artpixxel.data.Features.LeadTimes;
using api.artpixxel.data.Features.Metas;
using api.artpixxel.data.Features.Sizes;
using api.artpixxel.data.Features.States;
using api.artpixxel.data.Features.WallArtSizes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.artpixxel.data.Features.Common
{
    public class BaseStringRequest
    {
        public string Name { get; set; }
    }

    public class BaseOption : BaseId
    {
        public string Name { get; set; }

    }


    public class FlagOption : BaseOption
    {
        public string Country { get; set; }
    }

    public class StateOption : BaseOption
    {
        public string Country { get; set; }
    }

    public class CountryOption :BaseOption
    {
        public string Flag { get; set; }
    }

    public class CountOption : BaseOption
    {
        public decimal Count { get; set; }
    }
    
    public class BaseId
    {
        public string Id { get; set; }
    }


    public class PaginatedDeleteRequest
    {
        public BaseId Id { get; set; }
        public PaginationFilter PaginationFilter { get; set; }
    }
    public class PaginationFilter
    {
        public Pagination Pagination { get; set; }
        public BaseOption Category { get; set; }
    }
    public class Pagination
    {
        public int Skip { get; set; }
        public int PageSize { get; set; }
    }

    public class SearchFilter : Pagination
    {
        
        public string Search { get; set; }
    }

    public class BaseResponse
    {
        public bool Succeeded { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Result { get; set; }
    }


    public class PasswordCheckResponse
    {
        public bool SamePassword { get; set; }
        public string Message { get; set; }
    }
    public class ValidityBoolResponse
    {
        public bool Valid { get; set; }
        public string Message { get; set; }
    }

    public class BaseBoolResponse
    {
        public bool Exist { get; set; }
        public string Message { get; set; }
    }

    public class BaseBoolStatus
    {
        public bool Status { get; set; }
    }

    public class FileMeta
    {
        public byte[] ImageByte { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        
    }

    public class Filter
    {
        public int PageSize { get; set; }
        public int Skip { get; set; }
        public string Search { get; set; }
        public string SortField { get; set; }
        public int SortOrder { get; set; }
    }



    public class LoginRequest
    {

        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }




    public class LoginResponse : BaseResponse
    {
        public string Token { get; set; }

    }

    public class LogoutResponse
    {
        public bool LoggedIn { get; set; }

    }

    public class PasswordCheck
    {
        public string Password { get; set; }
    }

    public class ImageModel
    {
       public string Id { get; set; }
       public string Image { get; set; }
        public string CroppedImage { get; set; }
       public decimal? Price { get; set; }
       public string Name { get; set; }
       public string Description { get; set; }
       public string Type { get; set; } 
       
       
    }


    public class DeliveryCountry : CountryOption
    {
        public decimal DeliveryFee { get; set; }
    }

    public class DeliveryState : FullState
    {
        public decimal DeliveryFee { get; set; }
    }

    public class DeliveryCity : FullCity
    {
        public decimal DeliveryFee { get; set; }
    }

    public class LocationModel
    {
        public List<DeliveryCity> Cities { get; set; }
        public List<DeliveryState> States { get; set; }
        public List<DeliveryCountry> Countries { get; set; }
    }


    public class ShoppingInfo
    {
        public decimal UploadedImagePrice { get; set; }
        public decimal KidsGalleryImagePrice { get; set; }
        public LeadTimeBase LeadTime { get; set; }
        public decimal VAT { get; set; }
        public List<PublicSize> Sizes { get; set; }
        public List<PublicTemplateSize> TemplateSizes { get; set; }
        public List<PublicWallArtSize> WallArtSizes { get; set; }
        public List<PublicHomeGalleryModel> Galleries { get; set; }
        public List<ProductTypeMeta> Products { get; set; }
    }


   
}
