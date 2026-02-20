

using api.artpixxel.data.Features.Common;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.States
{
    public class StateFilter
    {
       
        public List<string> Countries { get; set; }
        public decimal DeliveryFee { get; set; }
        public string DeliveryFeeMatchMode { get; set; }
    }

    public class StateFilterData : Filter
    {
        public StateFilter Filters { get; set; }

    }

    public class StateSortField
    {
        public const string NAME = "name";
        public const string COUNTRY = "country";
        public const string CITYCOUNT = "cityCount";
        public const string DELIVERYFEE = "deliveryFee";


    }
}
