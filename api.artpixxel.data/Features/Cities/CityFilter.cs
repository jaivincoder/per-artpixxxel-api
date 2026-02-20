using api.artpixxel.data.Features.Common;
using System.Collections.Generic;


namespace api.artpixxel.data.Features.Cities
{
    public class CityFilter
    {

        public List<string> Countries { get; set; }
        public List<string> States { get; set; }
        public decimal DeliveryFee { get; set; }
        public string DeliveryFeeMatchMode { get; set; }
    }

    public class CityFilterData : Filter
    {
        public CityFilter Filters { get; set; }

    }

    public class CitySortField
    {
        public const string NAME = "name";
        public const string COUNTRY = "country";
        public const string STATE = "state";
        public const string DELIVERYFEE = "deliveryFee";


    }
}
