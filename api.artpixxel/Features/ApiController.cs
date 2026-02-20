
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Artpixxel.server.Features
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public abstract class ApiController : ControllerBase
    {
    }
}
