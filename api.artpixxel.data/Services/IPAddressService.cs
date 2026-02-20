

using Microsoft.AspNetCore.Http;

namespace api.artpixxel.data.Services
{
    public class IPAddressService : IIPAddressService
    {
        private readonly ConnectionInfo _connection;
        public IPAddressService(IHttpContextAccessor httpContextAccessor)
         => _connection = httpContextAccessor.HttpContext?.Connection;
        public string IPAddress()
           => _connection.RemoteIpAddress.ToString();
    }
}
