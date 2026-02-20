using api.artpixxel.data.Features.Cities;
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.FrameCategories;
using api.artpixxel.data.Models;
using api.artpixxel.repo.Features.Cities;
using api.artpixxel.repo.Features.FrameCategories;
using api.artpixxel.repo.Features.Frames;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using Stripe;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace api.artpixxel.Features.Framecategories
{
    public class FramecategoriesController : ApiController
    {
        private readonly IFrameCategoriesService _FrameCategoriesService;

        public FramecategoriesController(IFrameCategoriesService FrameCategoriesService)
        {
            _FrameCategoriesService = FrameCategoriesService;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(GetFrameCategories))]
        public async Task<IActionResult> GetFrameCategories()
        //=> await _FrameCategoriesService.GetFrameCategories();
        {
            try
            {
                var response = await _FrameCategoriesService.GetFrameCategories();

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
        [Route(nameof(GetFrameCategoryById) + "/{id}")]
        public async Task<IActionResult> GetFrameCategoryById(int id)
        //  => await _FrameCategoriesService.GetFrameCategoryById(id);
        {
            try
            {
                var response = await _FrameCategoriesService.GetFrameCategoryById(id);

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
        [Route(nameof(AddFrameCategories))]
        public async Task<IActionResult> AddFrameCategories([FromBody] FrameCategoryResponseDto request)
        //=> await _FrameCategoriesService.AddFrameCategories(request); 
        {
            try
            {
                var response = await _FrameCategoriesService.AddFrameCategories(request);

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
        [Route(nameof(UpdateFrameCategories))]
        public async Task<IActionResult> UpdateFrameCategories([FromBody] FrameCategoryResponseDto request)
        // => await _FrameCategoriesService.UpdateFrameCategories(request);
        {
            try
            {
                var response = await _FrameCategoriesService.UpdateFrameCategories(request);

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
        [Route(nameof(DeleteFrameCategory) + "/{id}")]
        public async Task<IActionResult> DeleteFrameCategory(int id)
        // => await _FrameCategoriesService.DeleteFrameCategory(id);
        {
            try
            {
                var response = await _FrameCategoriesService.DeleteFrameCategory(id);

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
