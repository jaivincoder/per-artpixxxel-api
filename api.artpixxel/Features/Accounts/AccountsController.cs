
using api.artpixxel.data.Features.Accounts;
using api.artpixxel.data.Features.Common;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.artpixxel.Features.Accounts
{
    
    public class AccountsController : ApiController
    {
        private readonly IAccountService _accountService;
        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }


        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(Exists))]
        public async Task<BaseBoolResponse> Exists(DuplicateAccount request)
            => await _accountService.Exists(request);

        [HttpPost]
        [Route(nameof(Duplicate))]
        public async Task<BaseBoolResponse> Duplicate(DuplicateAccount request)
            => await _accountService.Duplicate(request);

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(CreateCustomer))]
        public async Task<BaseResponse> CreateCustomer(AccountCreateRequest request)
            => await _accountService.CreateCustomer(request);

        [HttpPost]
        [Route(nameof(UpdateCustomer))]
        public async Task<BaseResponse> UpdateCustomer(CustomerDetail request)
            => await _accountService.UpdateCustomer(request);

        [Route(nameof(ResetPassword))]
        [HttpPost]
        public async Task<BaseResponse> ResetPassword(PasswordReset reset)
            => await _accountService.ResetPassword(reset);

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(Login))]
        public async Task<LoginResponse> Login(LoginRequest model)
            => await _accountService.Login(model);

        [HttpGet]
        [Route(nameof(Customer))]
        public async Task<CustomerInfoData> Customer()
            => await _accountService.Customer();


        [HttpPost]
        [Route(nameof(ResetSelfPassword))]
        public async Task<BaseResponse> ResetSelfPassword(SelfPasswordReset request)
            => await _accountService.ResetSelfPassword(request);

        [HttpGet]
        [Route(nameof(Notices))]
        public async Task<CustomerNotice> Notices()
            => await _accountService.Notices();

        [HttpGet]
        [Route(nameof(Logout))]
        public async Task<LogoutResponse> Logout()
            => await _accountService.Logout();

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(ResetToken))]
        public async Task<BaseResponse> ResetToken(ResetTokenRequest request)
            => await _accountService.ResetToken(request);


        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(NewPassword))]
        public async Task<BaseResponse> NewPassword(PasswordResetRequest request)
            => await _accountService.NewPassword(request);  
    }
}
