using api.artpixxel.data.Features.Payments;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Stripe;
using System;
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using api.artpixxel.repo.Extensions;
using api.artpixxel.data.Features.Checkouts;
using System.Collections.Generic;

namespace api.artpixxel.repo.Features.Payments
{
    public class PaymentService : IPaymentService
    {

        private readonly ArtPixxelContext _context;
        private readonly AppKeyConfig _appkeys;
       // private readonly IStripeClient client;

        public PaymentService(IOptions<AppKeyConfig> appkeys, ArtPixxelContext context)
        {
          
            _appkeys = appkeys.Value;
            _context = context;
           // StripeConfiguration.ApiKey = _appkeys.SecretKey;
            StripeConfiguration.MaxNetworkRetries = 10;
           // this.client = new StripeClient(_appkeys.SecretKey);

            
        }



        public async Task<PaymentChargeResponse> ChargeToken(PaymentChargeRequest request)
        {
            try
            {

                if (await _context.Metas.AnyAsync(e => e.MetaType == MetaType.PublishableKey))
                {
                    Meta secrKey = await _context.Metas.Where(e => e.MetaType == MetaType.SecretKey).FirstOrDefaultAsync();
                    if(secrKey != null)
                    {
                        string secretKey = secrKey.Value.Decrypt(_appkeys.EncryptionKey);


                        var requestOption = new RequestOptions
                        {
                            ApiKey = secretKey,
                            IdempotencyKey = request.InvoiceId,
                        };

                        //CustomerService customerService = new CustomerService();
                        //Customer customer = await customerService.CreateAsync(options, requestOption);


                        var chargeService = new ChargeService();
                        var chargeOptions = new ChargeCreateOptions
                        {
                            Amount = request.Amount,
                            Source = request.TokenId,
                            Currency = Currency.USD,
                            ReceiptEmail = request.EmailAddress,
                            Description = "Artpixxel Charge"

                        };

                        Charge chargeResult = await chargeService.CreateAsync(chargeOptions, requestOption);

                        if (chargeResult != null)
                        {
                            if (chargeResult.Paid)
                            {
                                return new PaymentChargeResponse
                                {
                                    ChargeId = chargeResult.Id,
                                    Response = new BaseResponse
                                    {
                                        Message = "Successful",
                                        Result = RequestResult.Success,
                                        Succeeded = true,
                                        Title = "Successful"
                                    }
                                };
                            }



                        }



                    }


                }




                   


                return new PaymentChargeResponse
                {
                    ChargeId = string.Empty,
                    Response = new BaseResponse
                    {
                        Message = "Payment unsucessful",
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Title = "An Error Occurred",
                    }
                };





            }
            catch (StripeException ex)
            {

                return new PaymentChargeResponse
                {
                    ChargeId = string.Empty,
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Title = ex.StripeError.DeclineCode
                    }
                };

              
            }
        }


        private async Task<PaymentIntentCheckout> CreatePaymentIntent(PaymentIntentCheckout request, Meta metaClient)
        {
            try
            {
                PaymentIntentService paymentintentService = new PaymentIntentService(new StripeClient(metaClient.Value.Decrypt(_appkeys.EncryptionKey)));
                PaymentIntent paymentIntent;


                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(request.Total * 100),
                    Currency = Currency.USD,
                    PaymentMethodTypes = new List<string> { "card" }
                };

                paymentIntent = await paymentintentService.CreateAsync(options);
                request.PaymentIntentId = paymentIntent.Id;
                request.ClientSecret = paymentIntent.ClientSecret;


                return request;
            }

            catch(Exception)
            {
                return request;
            }
        }

        public async Task<PaymentIntentCheckoutResponse> CreateUpdatePaymentIntent(PaymentIntentCheckout request)
        {
            try
            {

                if(await _context.Metas.AnyAsync(e => e.MetaType == MetaType.SecretKey))
                {
                    Meta metaClient = await _context.Metas.Where(e => e.MetaType == MetaType.SecretKey).FirstOrDefaultAsync();
                    if(metaClient != null)
                    {
                        PaymentIntentService paymentintentService = new PaymentIntentService(new StripeClient(metaClient.Value.Decrypt(_appkeys.EncryptionKey)));
                       // PaymentIntent paymentIntent;
                        string initId = request.PaymentIntentId;

                        // create a payment intent
                        if (string.IsNullOrEmpty(request.PaymentIntentId))
                        {
                            //var options = new PaymentIntentCreateOptions
                            //{
                            //    Amount = (long)(request.Total * 100),
                            //    Currency = Currency.USD,
                            //    PaymentMethodTypes = new List<string> { "card" }
                            //};

                            //paymentIntent = await paymentintentService.CreateAsync(options);
                            //request.PaymentIntentId = paymentIntent.Id;
                            //request.ClientSecret = paymentIntent.ClientSecret;


                            request = await CreatePaymentIntent(request, metaClient);


                        }
                        else
                        {
                            var options = new PaymentIntentUpdateOptions
                            {
                                Amount = (long)(request.Total * 100),
                              
                            };

                              await paymentintentService.UpdateAsync(request.PaymentIntentId, options);
                            
                        }



                        return new PaymentIntentCheckoutResponse
                        {
                            Checkout = request,
                            Response = new BaseResponse
                            {
                                Message = string.IsNullOrEmpty(initId) ? "Payment intent successfully created": "Payment intent successfully updated",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Title"
                            }

                        };
                    }
                }



                return new PaymentIntentCheckoutResponse
                {
                    Checkout = request,
                    Response = new BaseResponse
                    {
                        Message = "Payment intent failed. No client key found.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Validation Key Error"
                    }
                };

               


            }
            catch (Exception ex)
            {

                // possible double charges

                //if (await _context.Metas.AnyAsync(e => e.MetaType == MetaType.SecretKey))
                //{
                //    Meta metaClient = await _context.Metas.Where(e => e.MetaType == MetaType.SecretKey).FirstOrDefaultAsync();
                //    if (metaClient != null)
                //    {
                //        request = await CreatePaymentIntent(request, metaClient);
                //    }
                    
                //}
                   

                return new PaymentIntentCheckoutResponse
                {
                    Checkout = request,
                    Response = new BaseResponse
                    {
                        Message= ex.Message,
                        Title = ex.Source,
                        Result = RequestResult.Error,
                        Succeeded= true,
                    }
                };
            }
        }

        //public async Task<PublishableKeyResponse> PaymentIntent(PaymentIntentRequest intentRequest)
        //{
        //    try
        //    {


        //        var tokenOptions = new TokenCreateOptions
        //        {
        //            Card = new TokenCardOptions
        //            {
        //                Number = "4242424242424242",
        //                ExpMonth = "10",
        //                ExpYear = "2023",
        //                Cvc = "314",
        //            },
        //        };
        //        var tokenservice = new TokenService();
        //       Token token =  tokenservice.Create(tokenOptions);






        //        PaymentIntentCreateOptions options = new()
        //        {
        //            Amount = intentRequest.Amount,
        //            Currency = intentRequest.Currency,
        //            PaymentMethodTypes =  intentRequest.PaymentMethodTypes 


        //        };



        //        PaymentIntentService service = new PaymentIntentService(this.client);
        //        var paymentIntent = await service.CreateAsync(options);


        //        return new PublishableKeyResponse();
        //        ////per request
        //        //var requestOption = new RequestOptions
        //        //{
        //        //    ApiKey = _appkeys.SecretKey,
        //        //    IdempotencyKey = Guid.NewGuid().ToString(),
        //        //};

        //        //return new PublishableKeyResponse 
        //        //{ 
        //        //  ClientSecret = paymentIntent.ClientSecret
        //        //};
        //    }
        //    catch (StripeException)
        //    {

        //        throw ;
        //    }
        //}

        public async Task<PublishableKeyResponse> Pub()
        {
            try
            {

                if (await _context.Metas.AnyAsync(e => e.MetaType == MetaType.PublishableKey))
                {
                    Meta pubKey = await _context.Metas.Where(e => e.MetaType == MetaType.PublishableKey).FirstOrDefaultAsync();
                    if(pubKey != null)
                    {
                        return new PublishableKeyResponse
                        {
                            PubKey = pubKey.Value.Decrypt(_appkeys.EncryptionKey)
                        };
                    }

                }


                return new PublishableKeyResponse
                {
                    PubKey = string.Empty
                };

              
                   
               
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
