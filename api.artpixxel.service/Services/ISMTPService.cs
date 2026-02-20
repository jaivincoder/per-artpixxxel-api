

using api.artpixxel.data.Features.Accounts;
using api.artpixxel.data.Features.Checkouts;
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Contacts;
using api.artpixxel.data.Features.OrderStatuses;
using api.artpixxel.data.Features.SMTPs;
using api.artpixxel.data.Models;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
   public interface ISMTPService
    {
        
        Task<BaseBoolResponse> Exists(SMTPModel request);
        Task<BaseBoolResponse> Duplicate(SMTPModel request);
        Task<SMTPInit> Init(Pagination pagination);
        Task<SMTPCRUDResponse> Create(SMTPWriteRequest request);
        Task<SMTPCRUDResponse> Update(SMTPWriteRequest request);
        Task<SMTPCRUDResponse> Delete(SMTPDeleteRequest request);
        Task<EmailSendStatus> SendWelcomeMail(AccountModel request);
        Task<EmailSendStatus> SendPasswordResetToken(TokentNotificationModel request);
        Task<EmailSendStatus> SendContactNotification(ContactRequest contact);
        Task<EmailSendStatus> SendAdminOrderNotification(CheckoutNotification checkoutNotification );
        Task<EmailSendStatus> SendOrderStatusNotification(OrderStatusMail statusMail);
        Task<EmailSendStatus> SendCustomerOrderNotification(CheckoutNotification checkoutNotification);



    }
}
