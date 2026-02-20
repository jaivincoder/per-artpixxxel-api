using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Frames;
using api.artpixxel.data.Models;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace api.artpixxel.Features.Frames
{
    public class FrameController : ApiController
    {
        private readonly IFrameService _frameService;

        public FrameController(IFrameService frameService)
        {
            _frameService = frameService;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(GetAllFrame))]
        public async Task<IActionResult> GetAllFrame()
        //=> await _frameService.GetAll();
        {
            try
            {
                var response = await _frameService.GetAll();

                if (!response.Succeeded)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Return a 500 Internal Server Error response
                return StatusCode(500, new BaseResponse
                {
                    Succeeded = false,
                    Title = "Server Error",
                    Message = "An unexpected error occurred.",
                    Result = ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(GetFrameByFrameType) + "/{frameType}")]
        public async Task<IActionResult> GetFrameByFrameType(string frameType)
        //=> await _frameService.GetByFrameType(frameType);
        {
            try
            {
                var response = await _frameService.GetByFrameType(frameType);

                if (!response.Succeeded)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Return a 500 Internal Server Error response
                return StatusCode(500, new BaseResponse
                {
                    Succeeded = false,
                    Title = "Server Error",
                    Message = "An unexpected error occurred.",
                    Result = ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(GetFrameByNameAndType))]
        public async Task<IActionResult> GetFrameByNameAndType([FromQuery] string type, [FromQuery] string name)
        //=> await _frameService.GetFrameByName(type, name);
        {
            try
            {
                FrameCRUDResponse response = await _frameService.GetFrameByName(type, name);

                if (!response.Succeeded)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Return a 500 Internal Server Error response
                return StatusCode(500, new BaseResponse
                {
                    Succeeded = false,
                    Title = "Server Error",
                    Message = "An unexpected error occurred.",
                    Result = ex.Message
                });
            }
        }

        [HttpPost]
        [Route(nameof(AddFrames))]
        public async Task<IActionResult> AddFrames([FromForm] FrameRequestNew request)
        //   =>await _frameService.AddFrames(request);
        {
            try
            {
                var response = await _frameService.AddFrames(request);

                if (!response.Succeeded)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Return a 500 Internal Server Error response
                return StatusCode(500, new BaseResponse
                {
                    Succeeded = false,
                    Title = "Server Error",
                    Message = "An unexpected error occurred.",
                    Result = ex.Message
                });
            }
        }

        [HttpPut]
        [Route(nameof(UpdateFrame) + "/{id}")]
        public async Task<IActionResult> UpdateFrame(string id, [FromForm] FrameRequestNew request)
        //=>await _frameService.Update(id, request);
        {
            try
            {
                var response = await _frameService.Update(id, request);

                if (!response.Succeeded)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Return a 500 Internal Server Error response
                return StatusCode(500, new BaseResponse
                {
                    Succeeded = false,
                    Title = "Server Error",
                    Message = "An unexpected error occurred.",
                    Result = ex.Message
                });
            }
        }

        [HttpDelete]
        [Route(nameof(DeleteFrame) + "/{id}")]
        public async Task<IActionResult> DeleteFrame(string id)
        //=> await _frameService.Delete(new FrameDeleteRequest { Id = id });
        {
            try
            {
                var response = await _frameService.Delete(new FrameDeleteRequest { Id = id });

                if (!response.Succeeded)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Return a 500 Internal Server Error response
                return StatusCode(500, new BaseResponse
                {
                    Succeeded = false,
                    Title = "Server Error",
                    Message = "An unexpected error occurred.",
                    Result = ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(GetFrameByCategory) + "/{categoryId}")]
        public async Task<IActionResult> GetFrameByCategory(int categoryId)
        //=> await _frameService.GetByCategory(categoryId);
        {
            try
            {
                var response = await _frameService.GetByCategory(categoryId);

                if (!response.Succeeded)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Return a 500 Internal Server Error response
                return StatusCode(500, new BaseResponse
                {
                    Succeeded = false,
                    Title = "Server Error",
                    Message = "An unexpected error occurred.",
                    Result = ex.Message
                });
            }
        }

    }
}
