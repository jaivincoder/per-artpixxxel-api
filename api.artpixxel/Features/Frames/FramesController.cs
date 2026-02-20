using api.artpixxel.service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.artpixxel.Features.Frames
{
    [Authorize]
    public class FramesController
    { }
    //public class FramesController : ApiController
    //{
    //    private readonly IFrameService frameService;

    //    public FramesController(IFrameService frameService)
    //    {
    //        this.frameService = frameService;
    //    }

    //    [HttpGet]
    //    public async Task<IActionResult> GetAll()
    //        => Ok(await this.frameService.GetAllFrames());
    //}
}