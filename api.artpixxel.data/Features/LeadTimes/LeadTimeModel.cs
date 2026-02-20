

using api.artpixxel.data.Features.Common;

namespace api.artpixxel.data.Features.LeadTimes
{

    public class LeadTimeBase
    {
       
        public string TimeLimit { get; set; }
        public int LowerBandQuantifier { get; set; }
        public int UpperBandQuantifier { get; set; }
       
    }

    public class LeadTimeModel : LeadTimeBase
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }

    public class LeadTimeResponse
    {
        public LeadTimeModel LeadTime { get; set; }
        public BaseResponse Response { get; set; }
    }
}
