

using api.artpixxel.data.Features.Accounts;
using api.artpixxel.data.Features.Admin;
using api.artpixxel.data.Features.Common;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
   public interface IAdminServices
    {
        Task<BaseStringRequest> AllRoles();
        Task<LoginResponse> Login(LoginRequest model);
        Task<AdminData> Admin();
        Task<PasswordCheckResponse> PasswordCheck(PasswordCheck request);
        Task<BaseBoolResponse> Duplicate(DuplicateAccount request);
        Task<BaseResponse> Update(UserInfoUpdate request);
    }
}
