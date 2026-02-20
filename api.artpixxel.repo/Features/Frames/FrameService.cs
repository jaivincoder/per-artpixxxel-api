using api.artpixxel.data;
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Frames;
using api.artpixxel.data.Models;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using static api.artpixxel.data.Models.Permissions;

namespace api.artpixxel.repo.Features.Frames
{
    public class FrameService : IFrameService
    {
        private readonly ArtPixxelContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public FrameService(ArtPixxelContext context, IWebHostEnvironment environment, IConfiguration configuration)
        {
            _context = context;
            _environment = environment;
            _configuration = configuration;
        }
        public async Task<FrameGetResponse> GetAll()
        {
            try
            {

                var FramesList = await (from f in _context.Frames 
                                        join fc in _context.FrameCategories on f.CategoryId equals fc.Id
                                        orderby fc.CategoryType, f.DisplayOrder, f.Name
                                        select new
                                        {
                                            Frame = f,
                                            CategoryType = fc.CategoryType
                                        }).ToListAsync();
                var frameList = FramesList.Select(f => new FrameDto
                {
                    Id = f.Frame.Id,
                    FrameSet = f.Frame.FrameSet,
                    FramePrice=f.Frame.FramePrice,
                    FrameType = f.Frame.FrameType,
                    SvgPath = f.Frame.SvgPath,
                    Thumbnail = f.Frame.Thumbnail,
                    Name = f.Frame.Name,
                    CategoryId = f.Frame.CategoryId,
                    CategoryType = f.CategoryType,
                    DisplayOrder = f.Frame.DisplayOrder,
                    IsActive= f.Frame.IsActive,
                }).ToList();
                return new FrameGetResponse
                {
                    Data = frameList,
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Frames Retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new FrameGetResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
        }
        public async Task<FrameCRUDResponse> GetFrameByName(string type, string name)
        {
            try
            {
                var result = await _context.Frames.FirstOrDefaultAsync(f => f.FrameType == type && f.Name == name);
                return new FrameCRUDResponse
                {
                    Data = result,
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Frame Retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new FrameCRUDResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }

        }

        public async Task<FrameResponse> GetByFrameType(string frameType)
        {
            try
            {
                var frames = await _context.Frames .Where(f => f.FrameType.ToLower() == frameType.ToLower()).ToListAsync();

                return new FrameResponse
                {
                    Data = frames,
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Frames retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new FrameResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
        }
        public async Task<FrameCRUDResponse> AddFrames(FrameRequestNew request)
        {
            bool exists = await _context.Frames.AnyAsync(f => f.Name.ToLower() == request.Name.ToLower() && !f.IsDeleted);
            if (exists)
            {
                return new FrameCRUDResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = "Frame name already exists"
                };
            }

            int nextFrameId = 1;
            var lastFrame = await _context.Frames.IgnoreQueryFilters().OrderByDescending(f => f.FrameId).FirstOrDefaultAsync();
            if (lastFrame != null)
                nextFrameId = lastFrame.FrameId + 1;

            var SVGPath = "";
            if(request.SvgPath != null)
            {
                SVGPath = await SaveFile(request.SvgPath);
            }
            var Thumbnail = "";
            if (request.Thumbnail !=null)
            {
                Thumbnail = await SaveFile(request.Thumbnail);
            }

            var frame = new data.Models.Frame
            {
                Id = Guid.NewGuid().ToString(),
                FrameId = nextFrameId,
                FrameType = request.FrameType,
                FrameSet = request.FrameSet,
                FramePrice=request.FramePrice,
                Name = request.Name,
                SvgPath = SVGPath,
                IsCharacter = request.FrameType == "kids-space" ? request.IsCharacter : null,
                DisplayOrder = request.DisplayOrder,
                IsActive=request.IsActive,
                Thumbnail = Thumbnail,
                CategoryId = request.CategoryId
            };

            try
            {
                _context.Frames.Add(frame);
                await _context.SaveChangesAsync();
                return new FrameCRUDResponse
                {
                    Data = frame,
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Frame Created successfully"
                };
            }
            catch (Exception ex)
            {
                return new FrameCRUDResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
        }

        public async Task<FrameCRUDResponse> Update(string id, FrameRequestNew request)
        {
            try
            {
                var frame = await _context.Frames.FirstOrDefaultAsync(f => f.Id == id);
                if (frame == null)
                {
                    return new FrameCRUDResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = "Frame not found"
                    };
                }
                var svgpath = "";
                if(request.SvgPath != null)
                {
                    svgpath = await SaveFile(request.SvgPath);
                    frame.SvgPath = svgpath;
                }
                var Thumbnail = "";
                if(request.Thumbnail != null)
                {
                    Thumbnail = await SaveFile(request.Thumbnail);
                    frame.Thumbnail= Thumbnail;
                }
                frame.FrameType = request.FrameType;
                frame.FrameSet=request.FrameSet;
                frame.FramePrice = request.FramePrice;
                frame.Name=request.Name;
                frame.IsCharacter=request.IsCharacter;
                frame.IsActive=request.IsActive;
                frame.DisplayOrder=request.DisplayOrder;
                frame.CategoryId=request.CategoryId;
                await _context.SaveChangesAsync();

                return new FrameCRUDResponse
                {
                    Data = frame,
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Frame updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new FrameCRUDResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
        }

        public async Task<FrameCRUDResponse> Delete(FrameDeleteRequest request)
        {
            try
            {
                var frame = await _context.Frames.FindAsync(request.Id);
                if (frame == null)
                {
                    return new FrameCRUDResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = "Frame not found"
                    };
                }

                _context.Frames.Remove(frame);
                await _context.SaveChangesAsync();

                return new FrameCRUDResponse
                {
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Frame deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new FrameCRUDResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var uploadBase = _configuration["FileStorage:UploadFolder"] ?? "";

            var folderName = "Frames";
            var uploadsFolder = Path.Combine(_environment.ContentRootPath, uploadBase, folderName);
            Directory.CreateDirectory(uploadsFolder);

            var dateTimePrefix = DateTime.Now.ToString("ddMMyyyy_HHmmss");
            var fileName = $"{dateTimePrefix}_{Path.GetFileName(file.FileName)}";

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/{folderName}/{fileName}";
        }

        public async Task<FrameResponse> GetByCategory(int categoryId)
        {
            try
            {
                var frames = await _context.Frames.Where(f => f.CategoryId == categoryId).ToListAsync();
                return new FrameResponse
                {
                    Data = frames,
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Frames retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new FrameResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
        }
    }
}