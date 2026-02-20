

using api.artpixxel.data.Features.Common;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.Cities
{

    public class CityBase
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal DeliveryFee { get; set; }
        public string Description { get; set; }

    }
    public class CityModel : CityBase
    {
       
        public CountryOption Country { get; set; }
        public StateOption State { get; set; }
    }


    public class CityRequest : CityBase
    {
        public string Country { get; set; }
        public string State { get; set; }
    }


    public class CityBaseInit : CityInit
    {
        public List<CountryOption> Countries { get; set; }
        public List<StateOption> States { get; set; }

    }


    public class CityCRUDResponse
    {
        public BaseResponse Response { get; set; }
        public CityInit CityInit { get; set; }
    }

    public class CityInit
    {
        public List<CityModel> Cities { get; set; }
        public decimal TotalCity { get; set; }

    }

    public class CityWriteRequest
    {
        public CityFilterData Filter { get; set; }
        public CityRequest City { get; set; }
    }

    public class FullCity : BaseOption
    {
        public string StateId { get; set; }
        
    }

    public class CityDeleteRequest
    {
        public string Id { get; set; }
        public CityFilterData Filter { get; set; }
    }


    public class MultiCityWriteRequest
    {
        public CityFilterData Filter { get; set; }
        public List<CityRequest> Requests { get; set; }
    }


    public class BatchCityDeleteRequest
    {
        public List<string> Ids { get; set; }
        public CityFilterData Filter { get; set; }
    }


    public class BatchCity
    {
        public List<string> BatchIds { get; set; }
        public string BatchCountry { get; set; }
        public string BatchState { get; set; }
        public decimal BatchDeliveryFee { get; set; }
        public string BatchDescription { get; set; }
    }

    public class BatchCityWriteRequest
    {
        public BatchCity Request { get; set; }
        public CityFilterData Filter { get; set; }
    }
}


