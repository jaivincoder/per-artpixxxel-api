

using api.artpixxel.data.Features.Accounts;
using api.artpixxel.data.Features.Common;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
    public interface IAccountService
    {
        Task<BaseBoolResponse> Exists(DuplicateAccount request);
        Task<BaseBoolResponse> Duplicate(DuplicateAccount request);
        Task<BaseResponse> CreateCustomer(AccountCreateRequest request);
        Task<BaseResponse> UpdateCustomer(CustomerDetail request);
        Task<BaseResponse> ResetPassword(PasswordReset reset);
        Task<BaseResponse> ResetSelfPassword(SelfPasswordReset request);
        Task<LoginResponse> Login(LoginRequest model);
        Task<CustomerNotice> Notices();
        Task<LogoutResponse> Logout();
        Task<CustomerInfoData> Customer();

        Task<BaseResponse> ResetToken(ResetTokenRequest request);
        Task<BaseResponse> NewPassword(PasswordResetRequest request);






    }
}
