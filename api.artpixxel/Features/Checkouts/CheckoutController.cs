using api.artpixxel.data.Features.Checkouts;
using api.artpixxel.data.Features.Common;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace api.artpixxel.Features.Checkouts
{
  
    public class CheckoutController : ApiController
    {
        private readonly ICheckoutService _checkoutService;
        private readonly ICheckOutNewService _checkOutNewService;
        public CheckoutController(ICheckoutService checkoutService, ICheckOutNewService checkOutNewService)
        {
            _checkoutService = checkoutService;
            _checkOutNewService = checkOutNewService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(Checkout))]
        public async Task<CheckoutResponse> Checkout(Checkout checkout)
            => await _checkoutService.Checkout(checkout);


        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(CheckoutNew))]
        public async Task<IActionResult> CheckoutNew(CheckoutNew checkout)
        //=> await _checkOutNewService.CheckoutNew(checkout);
        {
            try
            {
                if (checkout == null)
                    throw new ArgumentNullException(nameof(checkout));

                if (checkout.Cart == null)
                    throw new ArgumentNullException($"{nameof(checkout.Cart)} is null.");

                if (checkout.Cart.Items == null || !checkout.Cart.Items.Any())
                    throw new ArgumentException("Cart.Items is null or empty", nameof(checkout));

                if (checkout.ShippingInformation == null)
                    throw new ArgumentNullException($"{nameof(checkout.ShippingInformation)} is null", nameof(checkout));
                CheckoutResponse response = await _checkOutNewService.CheckoutNew(checkout);

                if (!response.Response.Succeeded)
                {
                    return BadRequest(response.Response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Return a 500 Internal Server Error response
                return StatusCode(500, new BaseResponse
                {
                    Succeeded = false,
                    Title = "Server Error",
                    Message = "An unexpected error occurred.",
                    Result = ex.Message
                });
            }
        }
    }
}
