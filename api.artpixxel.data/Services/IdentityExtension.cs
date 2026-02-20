

using System.Linq;
using System.Security.Claims;

namespace api.artpixxel.data.Services
{
    public static class IdentityExtension
    {
        public static string GetId(this ClaimsPrincipal user)
            => user
            .Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
            ?.Value;

    }
}
