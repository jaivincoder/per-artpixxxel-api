using api.artpixxel.data.Features.Cities;
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Emails;
using api.artpixxel.data.Features.Newsletters;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
    public interface INewsletterService
    {
        Task<BaseResponse> Subscription(NewsletterSubscription request);
        Task<BaseResponse> Subscribe(NewsletterRequest request);
        Task<BaseBoolResponse> Exists(NewsletterRequest request);

    }
}
