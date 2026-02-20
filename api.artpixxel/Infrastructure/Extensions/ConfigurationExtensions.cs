using Microsoft.Extensions.Configuration;

namespace api.artpixxel.Infrastructure.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string GetDefaultConnection(this IConfiguration configuration)
         => configuration.GetConnectionString("DefaultConnection");
    }
}
