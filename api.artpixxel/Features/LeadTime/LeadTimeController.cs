using api.artpixxel.data.Features.LeadTimes;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.artpixxel.Features.LeadTime
{
   
    public class LeadTimeController : ApiController
    {
        private readonly ILeadTimeService _leadTimeService;
        public LeadTimeController(ILeadTimeService leadTimeService)
        {
            _leadTimeService = leadTimeService;
        }

        [HttpPost]
        [Route(nameof(Set))]
        public async Task<LeadTimeResponse> Set(LeadTimeModel request)
            => await _leadTimeService.Set(request);
    }
}
