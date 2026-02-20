

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Emails;
using api.artpixxel.data.Features.States;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
   public interface IEmailService
    {
        Task<BaseResponse> AddToEmailList(NewsletterRequest request);
        Task AddToEmailList(string emailAddress);
        Task AddToEmailList(EmailListRequest request);
        Task<EmailListInit> Init(Pagination pagination);
        Task<EmailListInit> MailList(EmailListFilterData filter);
        Task<EmailListCRUDResponse> MultiDelete(EmailListMultiDeleteRequest request);
        Task<EmailListCRUDResponse> Delete(EmailListDeleteRequest request);
        Task<List<EmailListResponse>> Export(EmailListFilterData filter);



    }
}
