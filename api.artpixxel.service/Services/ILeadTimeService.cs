
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.LeadTimes;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
  public  interface ILeadTimeService
    {
        Task<LeadTimeResponse> Set(LeadTimeModel request);
    }
}
