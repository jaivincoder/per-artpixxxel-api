using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Emails;
using api.artpixxel.data.Features.Newsletters;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.artpixxel.Features.Newsletters
{
   
    public class NewsletterController : ApiController
    {
        private readonly INewsletterService _newsletterService;
        public NewsletterController(INewsletterService newsletterService)
        {
            _newsletterService = newsletterService;
        }

        [HttpPost]
        [Route(nameof(Subscription))]
        public async Task<BaseResponse> Subscription(NewsletterSubscription request)
            => await _newsletterService.Subscription(request);


        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(Subscribe))]
        public async Task<BaseResponse> Subscribe(NewsletterRequest request)
            => await _newsletterService.Subscribe(request);

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(Exists))]
        public async Task<BaseBoolResponse> Exists(NewsletterRequest request)
            => await _newsletterService.Exists(request);
    }
}
