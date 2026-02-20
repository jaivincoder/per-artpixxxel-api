

using api.artpixxel.data.Features.Common;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.States
{
   public class StateModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal DeliveryFee { get; set; }
        public CountryOption Country { get; set; }
        public string Description { get; set; }
        public decimal CityCount { get; set; } 
    }

    public class StateBaseInit : StateInit
    {
        public List<CountryOption> Options { get; set; }
       
    }

    public class StateInit 
    {
        public List<StateModel> States { get; set; }
        public decimal TotalState { get; set; }
       
    }

    public class FullState: BaseOption
    {
      public string CountryId { get; set; }
    }


    public class StateRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public decimal DeliveryFee { get; set; }
        public string Description { get; set; }
    }

    public class StateDeleteRequest
    {
        public string Id { get; set; }
        public StateFilterData Filter { get; set; }
    }

    public class StateWriteRequest
    {
        public StateFilterData Filter { get; set; }
        public StateRequest Request { get; set; }
    }


    public class MultiStateWriteRequest
    {
        public StateFilterData Filter { get; set; }
        public List<StateRequest> Requests { get; set; }
    }

    public class StateCRUDResponse
    {
        public BaseResponse Response { get; set; }
        public StateInit StateInit { get; set; }
    }


    public class BatchState
    {
      public List<string> BatchIds { get; set; }
      public string BatchCountry { get; set; }
      public decimal BatchDeliveryFee { get; set; }
      public string  BatchDescription { get; set; }
    }

   
    public class BatchStateWriteRequest
    {   public BatchState Request { get; set; }
        public StateFilterData Filter { get; set; }
    }

    public class BatchStateDeleteRequest
    {
        public List<string> Ids { get; set; }
        public StateFilterData Filter { get; set; }
    }

}
