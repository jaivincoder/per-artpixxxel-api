using api.artpixxel.data.Features.Checkouts;
using api.artpixxel.data.Features.Common;
using api.artpixxel.repo;
using api.artpixxel.service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.artpixxel.repo.Features.Checkouts
{
    public class CheckoutCombinedService : ICheckoutCombinedService
    {
        private readonly ICheckoutService _checkoutService;
        private readonly ICheckOutNewService _checkOutNewService;

        public CheckoutCombinedService(ICheckoutService checkoutService, ICheckOutNewService checkOutNewService)
        {
            _checkoutService = checkoutService;
            _checkOutNewService = checkOutNewService;
        }

        public async Task<CheckoutCombinedResponse> CheckoutCombined(CheckoutCombinedRequest request)
        {
            if (request == null)
            {
                return new CheckoutCombinedResponse
                {
                    Response = new BaseResponse
                    {
                        Succeeded = false,
                        Title = "Validation Error",
                        Message = "Request is required",
                        Result = RequestResult.Error
                    }
                };
            }

            var hasRegular = request.RegularCart?.Items != null && request.RegularCart.Items.Any();
            var hasCustomMix = request.CustomMixCart?.Items != null && request.CustomMixCart.Items.Any();

            if (!hasRegular && !hasCustomMix)
            {
                return new CheckoutCombinedResponse
                {
                    Response = new BaseResponse
                    {
                        Succeeded = false,
                        Title = "Validation Error",
                        Message = "At least one cart (regular or custom-mix) must have items",
                        Result = RequestResult.Error
                    }
                };
            }

            if (string.IsNullOrEmpty(request.PaymentIntentId))
            {
                return new CheckoutCombinedResponse
                {
                    Response = new BaseResponse
                    {
                        Succeeded = false,
                        Title = "Payment Error",
                        Message = "Payment intent error. Please try again later",
                        Result = RequestResult.Error
                    }
                };
            }

            var trackIds = new List<string>();
            BaseResponse lastError = null;

            if (hasRegular && hasCustomMix)
            {
                var regularSub = request.RegularCart.TotalAmount;
                var customSub = request.CustomMixCart.TotalAmount;
                var combinedSub = regularSub + customSub;
                var ratio = combinedSub > 0 ? regularSub / combinedSub : 1m;

                var regularDelivery = Math.Round(request.DeliveryFee * ratio, 2);
                var customDelivery = request.DeliveryFee - regularDelivery;
                var regularVat = Math.Round(request.Vat * ratio, 2);
                var customVat = request.Vat - regularVat;

                var regularCheckout = new Checkout
                {
                    DeliveryInformation = request.DeliveryInformation,
                    PaymentInformation = request.PaymentInformation,
                    ShippingInformation = request.ShippingInformation,
                    Cart = request.RegularCart,
                    LeadTime = request.LeadTime,
                    UpperLimit = request.UpperLimit,
                    LowerLimit = request.LowerLimit,
                    PaymentIntentId = request.PaymentIntentId,
                    DeliveryFee = regularDelivery,
                    Vat = regularVat
                };

                var customCheckout = new CheckoutNew
                {
                    DeliveryInformation = request.DeliveryInformation,
                    PaymentInformation = request.PaymentInformation,
                    ShippingInformation = request.ShippingInformation,
                    Cart = request.CustomMixCart,
                    LeadTime = request.LeadTime,
                    UpperLimit = request.UpperLimit,
                    LowerLimit = request.LowerLimit,
                    PaymentIntentId = request.PaymentIntentId,
                    DeliveryFee = customDelivery,
                    Vat = customVat
                };

                var regularResult = await _checkoutService.Checkout(regularCheckout);
                if (regularResult.Response.Succeeded && !string.IsNullOrEmpty(regularResult.TrackId))
                    trackIds.Add(regularResult.TrackId);
                else
                    lastError = regularResult.Response;

                var customResult = await _checkOutNewService.CheckoutNew(customCheckout);
                if (customResult.Response.Succeeded && !string.IsNullOrEmpty(customResult.TrackId))
                    trackIds.Add(customResult.TrackId);
                else
                    lastError = customResult.Response;
            }
            else if (hasRegular)
            {
                var checkout = new Checkout
                {
                    DeliveryInformation = request.DeliveryInformation,
                    PaymentInformation = request.PaymentInformation,
                    ShippingInformation = request.ShippingInformation,
                    Cart = request.RegularCart,
                    LeadTime = request.LeadTime,
                    UpperLimit = request.UpperLimit,
                    LowerLimit = request.LowerLimit,
                    PaymentIntentId = request.PaymentIntentId,
                    DeliveryFee = request.DeliveryFee,
                    Vat = request.Vat
                };

                var result = await _checkoutService.Checkout(checkout);
                if (result.Response.Succeeded && !string.IsNullOrEmpty(result.TrackId))
                    trackIds.Add(result.TrackId);
                else
                    lastError = result.Response;
            }
            else
            {
                var checkout = new CheckoutNew
                {
                    DeliveryInformation = request.DeliveryInformation,
                    PaymentInformation = request.PaymentInformation,
                    ShippingInformation = request.ShippingInformation,
                    Cart = request.CustomMixCart,
                    LeadTime = request.LeadTime,
                    UpperLimit = request.UpperLimit,
                    LowerLimit = request.LowerLimit,
                    PaymentIntentId = request.PaymentIntentId,
                    DeliveryFee = request.DeliveryFee,
                    Vat = request.Vat
                };

                var result = await _checkOutNewService.CheckoutNew(checkout);
                if (result.Response.Succeeded && !string.IsNullOrEmpty(result.TrackId))
                    trackIds.Add(result.TrackId);
                else
                    lastError = result.Response;
            }

            var allSucceeded = (hasRegular && hasCustomMix)
                ? trackIds.Count == 2
                : trackIds.Count == 1;

            return new CheckoutCombinedResponse
            {
                TrackIds = trackIds,
                Response = allSucceeded
                    ? new BaseResponse
                    {
                        Succeeded = true,
                        Title = "Success",
                        Message = "Orders created successfully",
                        Result = RequestResult.Success
                    }
                    : (lastError ?? new BaseResponse
                    {
                        Succeeded = false,
                        Title = "Checkout Error",
                        Message = "One or more orders failed to create",
                        Result = RequestResult.Error
                    })
            };
        }
    }
}
