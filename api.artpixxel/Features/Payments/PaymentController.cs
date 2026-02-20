using api.artpixxel.data.Features.Checkouts;
using api.artpixxel.data.Features.Payments;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.artpixxel.Features.Payments
{
    
    public class PaymentController : ApiController
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        [Route(nameof(Pub))]
        [AllowAnonymous]
        public async Task<PublishableKeyResponse> Pub()
            => await _paymentService.Pub();


        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(CreateUpdatePaymentIntent))]
        public async Task<PaymentIntentCheckoutResponse> CreateUpdatePaymentIntent(PaymentIntentCheckout request)
            => await _paymentService.CreateUpdatePaymentIntent(request);

    }
}
