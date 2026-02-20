

using System.Security.Claims;
using Microsoft.AspNetCore.Http;


namespace api.artpixxel.data.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly ClaimsPrincipal _user;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _user = httpContextAccessor.HttpContext?.User;
            _httpContextAccessor = httpContextAccessor;
        }

       

        public string GetUsername()
        => _user
            ?.Identity
            ?.Name;

        public string GetUserId()
         => _user
            .GetId();

        public ClaimsPrincipal GetUser()
         => _user;

        public string GetBaseURl()
         => _httpContextAccessor.HttpContext.Request.Headers["origin"];

        public string WebRoot()
      => $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}";
    }
}

