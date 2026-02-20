

using api.artpixxel.data.Features.Common;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.Countries
{
   public class CountryResponse
    {
     public string Id { get; set; }
     public BaseOption  Flag { get; set; }
     public decimal DeliveryFee { get; set; }
     public FlagOption Name { get; set; }
     public string Description { get; set; }
     public int StateCount { get; set; }
    }

    public class CountryBaseInit
    {
        public List<FlagOption> Options { get; set; }
        public List<CountryResponse> Countries { get; set; }
    }

    public class CountryInit : CountryBaseInit
    {
        public List<FlagOption> Flags { get; set; }
    }

    public class CountryCRUDResponse 
    {   
        public CountryBaseInit Country { get; set; }
        public BaseResponse Response { get; set; }
    }


    public class CountryDeleteRequest
    {
        public string Id { get; set; }
    }


    public class CountryWriteRequest
    {
      public string Id { get; set; }
      public string Flag { get; set; }
      public decimal DeliveryFee { get; set; }
      public string Name { get; set; }
      public string  Description { get; set; }
    }


    public class MultiCountryWriteRequest
    {
        public List<CountryWriteRequest> Countries { get; set; }
    }
}
