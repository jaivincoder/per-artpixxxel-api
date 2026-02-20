using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.GalleryImages;
using api.artpixxel.data.Models;
using api.artpixxel.repo.Features.Frames;
using api.artpixxel.repo.Features.TemplateConfigs;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.Features.GalleryImages
{
    public class GalleryImagesController : ApiController
    {
        private readonly IGalleryImageService _GalleryImageService;

        public GalleryImagesController(IGalleryImageService GalleryImageService)
        {
            _GalleryImageService = GalleryImageService;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(GetGalleryImages))]
        public async Task<IActionResult> GetGalleryImages()
        //=> await _GalleryImageService.GetGalleryImages();
        {
            try
            {
                var response = await _GalleryImageService.GetGalleryImages();

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
        [Route(nameof(GetGalleryImageById) + "/{id}")]
        public async Task<IActionResult> GetGalleryImageById(int id)
        //    => await _GalleryImageService.GetGalleryImageById(id);
        {
            try
            {
                var response = await _GalleryImageService.GetGalleryImageById(id);

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
        [Route(nameof(AddGalleryImage))]
        public async Task<IActionResult> AddGalleryImage([FromBody] GalleryImagesModel request)
        //=> await _GalleryImageService.AddGalleryImage(request);
        {
            try
            {
                var response = await _GalleryImageService.AddGalleryImage(request);

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
        [Route(nameof(UpdateGalleryImage) + "/{id}" + "/{CategoryId}")]
        public async Task<IActionResult> UpdateGalleryImage(int id, int CategoryId, [FromForm] GalleryImageUpdateRequest request)
        //=> await _GalleryImageService.UpdateGalleryImage(id, CategoryId, request);
        {
            try
            {
                var response = await _GalleryImageService.UpdateGalleryImage(id, CategoryId, request);

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
        [Route(nameof(DeleteGalleryImage) + "/{id}")]
        public async Task<IActionResult> DeleteGalleryImage(int id)
        //=> await _GalleryImageService.DeleteGalleryImage(id);
        {
            try
            {
                var response = await _GalleryImageService.DeleteGalleryImage(id);

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
