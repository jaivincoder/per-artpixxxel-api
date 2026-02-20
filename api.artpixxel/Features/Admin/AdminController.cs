
using api.artpixxel.data.Features.Accounts;
using api.artpixxel.data.Features.Admin;
using api.artpixxel.data.Features.Common;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.artpixxel.Features.Admin
{
   
    public class AdminController : ApiController
    {
        private readonly IAdminServices _adminService;
        public AdminController(IAdminServices adminService)
        {
            _adminService = adminService;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(AllRoles))]
        public async Task<BaseStringRequest> AllRoles()
            => await _adminService.AllRoles();


        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(Login))]
        public async Task<LoginResponse> Login(LoginRequest model)
            => await _adminService.Login(model);

        [HttpGet]
        [Route(nameof(Admin))]
        public async Task<AdminData> Admin()
            => await _adminService.Admin();

        [HttpPost]
        [Route(nameof(PasswordCheck))]
        public async Task<PasswordCheckResponse> PasswordCheck(PasswordCheck request)
            => await _adminService.PasswordCheck(request);


        [HttpPost]
        [Route(nameof(Duplicate))]
        public async Task<BaseBoolResponse> Duplicate(DuplicateAccount request)
            => await _adminService.Duplicate(request);

        [HttpPost]
        [Route(nameof(Update))]
        public async Task<BaseResponse> Update(UserInfoUpdate request)
            => await _adminService.Update(request);
    }
}
