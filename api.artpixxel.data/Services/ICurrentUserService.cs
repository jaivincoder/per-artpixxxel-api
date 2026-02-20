

using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace api.artpixxel.data.Services
{
   public interface ICurrentUserService
    {
        string GetUsername();
        string GetUserId();
        ClaimsPrincipal GetUser();
        string GetBaseURl();
        string WebRoot();
    }
}
