using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.TemplateConfing;
using api.artpixxel.data.Models;
using api.artpixxel.Data;
using api.artpixxel.repo.Features.FrameCategories;
using api.artpixxel.repo.Features.GalleryImages;
using api.artpixxel.repo.Features.TemplateConfigs;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.Features.TemplateConfigs
{
    public class TemplateConfigController : ApiController
    {
        private readonly ITemplateCondigsService _TemplateConfigService;

        public TemplateConfigController(ITemplateCondigsService TemplateConfigService)
        {
            _TemplateConfigService = TemplateConfigService;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetTemplateConfigs")]
        public async Task<IActionResult> GetTemplateConfigs()
        //    => await _TemplateConfigService.GetTemplatesAsync();
        {
            try
            {
                var response = await _TemplateConfigService.GetTemplatesAsync();

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
        [Route(nameof(GetTemplateConfigsById) + "/{id}")]
        public async Task<IActionResult> GetTemplateConfigsById(int id)
        //=> await _TemplateConfigService.GetTemplatesById(id);
        {
            try
            {
                var response = await _TemplateConfigService.GetTemplatesById(id);

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
        [Route(nameof(GetTemplateConfigsByTemplateKey) + "/{TemplateKey}")]
        public async Task<IActionResult> GetTemplateConfigsByTemplateKey(string TemplateKey)
        //=> await _TemplateConfigService.GetTemplatesByTemplateKey(TemplateKey);
        {
            try
            {
                var response = await _TemplateConfigService.GetTemplatesByTemplateKey(TemplateKey);

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
        [Route(nameof(AddTemplateConfigs))]
        public async Task<IActionResult> AddTemplateConfigs([FromBody] TemplateConfingModel request)
        //  => await _TemplateConfigService.AddTemplateConfigs(request);
        {
            try
            {
                var response = await _TemplateConfigService.AddTemplateConfigs(request);

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
        [Route(nameof(UpdateTemplateConfigs) + "/{id}")]
        public async Task<IActionResult> UpdateTemplateConfigs(int id, [FromBody] TemplateConfingModel request)
        //   => await _TemplateConfigService.UpdateTemplateConfigs(id, request);
        {
            try
            {
                var response = await _TemplateConfigService.UpdateTemplateConfigs(id, request);

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
        [Route(nameof(DeleteTemplateConfigs) + "/{id}")]
        public async Task<IActionResult> DeleteTemplateConfigs(int id)
        //   => await _TemplateConfigService.DeleteTemplateConfigs(id);
        {
            try
            {
                var response = await _TemplateConfigService.DeleteTemplateConfigs(id);

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
        [Route(nameof(GetTemplateConfigsByCategoryId) + "/{CategoryId}")]
        public async Task<IActionResult> GetTemplateConfigsByCategoryId(int CategoryId)
        //  => await _TemplateConfigService.GetTemplatesByCategoryId(CategoryId);
        {
            try
            {
                var response = await _TemplateConfigService.GetTemplatesByCategoryId(CategoryId);

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
