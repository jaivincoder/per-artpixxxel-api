using api.artpixxel.data.Features.Checkouts;
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Payments;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
   public  interface IPaymentService
    {
        Task<PublishableKeyResponse> Pub();
        Task<PaymentChargeResponse> ChargeToken(PaymentChargeRequest request);
        Task<PaymentIntentCheckoutResponse> CreateUpdatePaymentIntent(PaymentIntentCheckout request);
    }
}
