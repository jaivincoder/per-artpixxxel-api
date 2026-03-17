

using System;
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

        public string ResolveImageUrl(string storedUrl)
        {
            if (string.IsNullOrEmpty(storedUrl)) return storedUrl;

            var path = storedUrl.Replace("\\", "/");

            if (Uri.TryCreate(path, UriKind.Absolute, out Uri uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                path = uri.AbsolutePath;

            var wwwrootIdx = path.IndexOf("wwwroot/images/", StringComparison.OrdinalIgnoreCase);
            if (wwwrootIdx >= 0)
                path = path.Substring(wwwrootIdx + "wwwroot".Length);

            if (!path.StartsWith("/"))
                path = "/" + path;

            return WebRoot() + path;
        }
    }
}

