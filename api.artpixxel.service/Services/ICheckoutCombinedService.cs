using api.artpixxel.data.Features.Checkouts;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
    public interface ICheckoutCombinedService
    {
        Task<CheckoutCombinedResponse> CheckoutCombined(CheckoutCombinedRequest request);
    }
}
