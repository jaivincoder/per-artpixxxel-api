using api.artpixxel.data;
using api.artpixxel.data.Features.Common;
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
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using api.artpixxel.data.Features.FrameCategories;
using static api.artpixxel.data.Models.Permissions;

namespace api.artpixxel.repo.Features.FrameCategories
{
    public class FrameCategoriesService : IFrameCategoriesService
    {
        private readonly ArtPixxelContext _context;
        private readonly IWebHostEnvironment _environment;

        public FrameCategoriesService(ArtPixxelContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<GetFrameCategoryListResponse> GetFrameCategories()
        {
            try
            {
                var frameCategories = await _context.FrameCategories.Where(fc => fc.IsDeleted == false).ToListAsync();
                var linecolor = await _context.line_colors_Master.Where(l => l.IsDeleted == false).ToListAsync();
                var GalleryImage = await _context.GalleryImages.Where(g => g.IsDeleted == false).ToListAsync();

                var result = frameCategories.Select(fc => new GetFrameCategory
                {
                    Id = fc.Id,
                    Label = fc.Label,
                    GallaryTitle = fc.GallaryTitle,
                    CategoryType = fc.CategoryType,
                    IsActive= fc.IsActive,
                    LineColors = linecolor.Where(lc => lc.CategoryId == fc.Id).Select(lc => new LineColor
                    {
                        Color = lc.Color,
                        IsActive=lc.IsActive
                    }).ToList(),

                    GalleryImage = GalleryImage.Where(gi => gi.CategoryId == fc.Id)
                    .Select(gi => new GetGalleryImage
                    {
                        Id = gi.Id,
                        CategoryId = gi.CategoryId,
                        ImageUrl = gi.ImageUrl,
                        ImageName = gi.ImageName,
                        DisplayOrder = gi.DisplayOrder,
                        IsActive=gi.IsActive,
                    }).ToList()
                }).ToList();
                return new GetFrameCategoryListResponse
                {
                    Data = result,
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Frame Categorie Retrieved successfully"
                };
            }
            catch(Exception ex)
            {
                return new GetFrameCategoryListResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
        }
        public async Task<GetFrameCategoryResponse> GetFrameCategoryById(int id)
        {
            try
            {
                var frameCategory = await _context.FrameCategories.Where(fc => fc.Id == id).FirstOrDefaultAsync();

                if (frameCategory == null)
                {
                    return new GetFrameCategoryResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = "Frame category not found."
                    };
                }

                var lineColors = await _context.line_colors_Master.Where(lc => lc.CategoryId == id )
                    .Select(lc => new LineColor
                    {
                        Color = lc.Color,
                        IsActive= lc.IsActive,
                    })
                    .ToListAsync();

                var galleryImages = await _context.GalleryImages.Where(gi => gi.CategoryId == id)
                    .Select(gi => new GetGalleryImage
                    {
                        Id = gi.Id,
                        CategoryId = gi.CategoryId,
                        ImageUrl = gi.ImageUrl,
                        ImageName = gi.ImageName,
                        DisplayOrder = gi.DisplayOrder,
                        IsActive= gi.IsActive,
                    })
                    .ToListAsync();

                var result = new GetFrameCategory
                {
                    Id = frameCategory.Id,
                    Label = frameCategory.Label,
                    GallaryTitle = frameCategory.GallaryTitle,
                    IsActive= frameCategory.IsActive,
                    CategoryType = frameCategory.CategoryType,
                    LineColors = lineColors,
                    GalleryImage = galleryImages
                };
                return new GetFrameCategoryResponse
                {
                    Data = result,
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Frame Categorie Retrieved successfully"
                };
            }
            catch(Exception ex)
            {
                return new GetFrameCategoryResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
        }
        public async Task<FrameCategoryCRUDResponse> AddFrameCategories(FrameCategoryResponseDto request)
        {
            try
            {
                var type = request.CategoryType;
                if (type != framecategories.CategoryTypeLinearArt && type != framecategories.CategoryTypeArtMat)
                {
                    return new FrameCategoryCRUDResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = $"CategoryType must be either {framecategories.CategoryTypeLinearArt} or {framecategories.CategoryTypeArtMat}"
                    };
                }
                if (type == framecategories.CategoryTypeLinearArt)
                {
                    if (request.LineColors == null || !request.LineColors.Any() || request.LineColors.Count<=0 || request.LineColors[0]==null)
                    {
                        return new FrameCategoryCRUDResponse
                        {
                            Succeeded = false,
                            Result = RequestResult.Error,
                            Message = "At least one LineColor is required for CategoryType 'LinearArt'."
                        };
                    }
                }

                var isExisting = await _context.FrameCategories
                    .AnyAsync(fc => fc.Label.ToLower() == request.Label.Trim().ToLower() && fc.IsDeleted == false);

                if (isExisting)
                {
                    return new FrameCategoryCRUDResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = "With same name Frame Category already exists."
                    };
                }



                    var frame = new frame_categories
                {
                    Label = request.Label,
                    GallaryTitle = request.GallaryTitle,
                    IsActive=request.IsActive,
                    CategoryType = request.CategoryType,
                };
                _context.FrameCategories.Add(frame);
                await _context.SaveChangesAsync();

                if (request.CategoryType == framecategories.CategoryTypeLinearArt && request.LineColors != null && request.LineColors.Any() && request.LineColors.Count >= 0 && request.LineColors[0] != null)
                {
                    foreach (var color in request.LineColors)
                    {
                        var lineColorEntity = new line_colors_Master
                        {
                            CategoryId = frame.Id,
                            Color = color.Color,
                            IsActive = color.IsActive,
                        };
                        _context.line_colors_Master.Add(lineColorEntity);
                    }
                    await _context.SaveChangesAsync();
                }
            
              
                var lineColorsNew = await _context.line_colors_Master.Where(x => x.CategoryId == frame.Id)
                 .Select(x => new LineColor
                 {
                     Color = x.Color,
                     IsActive = x.IsActive
                 }).ToListAsync();
                var galleryImages = await _context.GalleryImages.Where(x => x.CategoryId == frame.Id)
                .Select(x => new GetGalleryImage
                {
                    Id = x.Id,
                    CategoryId = x.CategoryId,
                    ImageName = x.ImageName,
                    DisplayOrder = x.DisplayOrder,
                    ImageUrl = x.ImageUrl,
                    IsActive = x.IsActive
                }).ToListAsync();

                var responseModel = new FrameCategoryModel
                {
                    Id = frame.Id,
                    Label = frame.Label,
                    GalleryTitle = frame.GallaryTitle,
                    CategoryType=frame.CategoryType,
                    LineColors = lineColorsNew,
                    GalleryImage = galleryImages
                };
                return new FrameCategoryCRUDResponse
                {
                    Data = responseModel,
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Frame Categorie added successfully"
                };
            }
            catch(Exception ex)
            {
                return new FrameCategoryCRUDResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
            
        }
        public async Task<FrameCategoryCRUDResponse> UpdateFrameCategories(FrameCategoryResponseDto request)
        {
            try
            {
                var type = request.CategoryType;
                if (type != framecategories.CategoryTypeLinearArt && type != framecategories.CategoryTypeArtMat)
                {
                    return new FrameCategoryCRUDResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = $"CategoryType must be either {framecategories.CategoryTypeLinearArt} or {framecategories.CategoryTypeArtMat}"
                    };
                }
                if (type == framecategories.CategoryTypeLinearArt)
                {
                    if (request.LineColors == null || !request.LineColors.Any() || request.LineColors.Count <= 0 || request.LineColors[0] == null)
                    {
                        return new FrameCategoryCRUDResponse
                        {
                            Succeeded = false,
                            Result = RequestResult.Error,
                            Message = "At least one LineColor is required for CategoryType 'LinearArt'."
                        };
                    }
                }
                var frame = await _context.FrameCategories.FirstOrDefaultAsync(fc => fc.Id == request.Id );
                if (frame == null)
                {
                    return new FrameCategoryCRUDResponse
                        {
                            Succeeded = false,
                            Result = RequestResult.Error,
                            Message = "Frame category not found."
                    };
                }
                var oldType = frame.CategoryType;
                frame.Label = request.Label;
                frame.GallaryTitle = request.GallaryTitle;
                frame.IsActive = request.IsActive;
                frame.CategoryType = request.CategoryType;

                bool isTypeChanged = oldType != request.CategoryType;

                if (isTypeChanged && request.CategoryType == framecategories.CategoryTypeArtMat)
                {
                    var allColors = await _context.line_colors_Master
                        .Where(c => c.CategoryId == frame.Id)
                        .ToListAsync();

                    if (allColors.Any())
                        _context.line_colors_Master.RemoveRange(allColors);
                }

                if (isTypeChanged && request.CategoryType == framecategories.CategoryTypeLinearArt)
                {
                    var allImages = await _context.GalleryImages
                        .Where(g => g.CategoryId == frame.Id)
                        .ToListAsync();

                    if (allImages.Any())
                        _context.GalleryImages.RemoveRange(allImages);
                }

                await _context.SaveChangesAsync();

                var oldColors = await _context.line_colors_Master.Where(f => f.CategoryId == request.Id).ToListAsync();
                if (oldColors.Any())
                {
                    _context.line_colors_Master.RemoveRange(oldColors);
                    await _context.SaveChangesAsync();
                }

                if (request.CategoryType == framecategories.CategoryTypeLinearArt && request.LineColors != null && request.LineColors.Count > 0)
                {
                    var newColors = request.LineColors
                        .Select(rc => new line_colors_Master
                        {
                            CategoryId = request.Id,
                            Color = rc.Color.Trim(),
                            IsActive = rc.IsActive
                        })
                        .ToList();

                    await _context.line_colors_Master.AddRangeAsync(newColors);
                    await _context.SaveChangesAsync();
                }

                var lineColorsNew = await _context.line_colors_Master.Where(x => x.CategoryId == frame.Id )
                       .Select(x => new LineColor
                       {
                           Color = x.Color,
                           IsActive = x.IsActive
                       }).ToListAsync();
                

                var galleryImages = await _context.GalleryImages.Where(x => x.CategoryId == frame.Id)
                 .Select(x => new GetGalleryImage
                 {
                     Id = x.Id,
                     CategoryId = x.CategoryId,
                     ImageName = x.ImageName,
                     DisplayOrder = x.DisplayOrder,
                     ImageUrl = x.ImageUrl,
                     IsActive = x.IsActive
                 }).ToListAsync();

                var responseModel = new FrameCategoryModel
                {
                    Id = frame.Id,
                    Label = frame.Label,
                    GalleryTitle = frame.GallaryTitle,
                    CategoryType=frame.CategoryType,
                    LineColors = lineColorsNew,
                    GalleryImage = galleryImages
                };
                return new FrameCategoryCRUDResponse
                {
                    Data = responseModel,
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Frame Categorie Updated successfully"
                };
            }
            catch(Exception ex) 
            {
                return new FrameCategoryCRUDResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
        }
        public async Task<FrameCategoryCRUDResponse> DeleteFrameCategory(int id)
        {
            try
            {
                var frame = await _context.FrameCategories.FirstOrDefaultAsync(fc => fc.Id == id );
                if (frame == null)
                {
                    return new FrameCategoryCRUDResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = "Frame category not found."
                    };
                }
                _context.FrameCategories.Remove(frame);
                var linecolors = await _context.line_colors_Master.Where(f => f.CategoryId == id).ToListAsync();
                if (linecolors.Any())
                {
                    _context.line_colors_Master.RemoveRange(linecolors);
                }
                await _context.SaveChangesAsync();
                return new FrameCategoryCRUDResponse
                {
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Frame Categorie Delete successfully"
                };
            }
            catch (Exception ex)
            {
                return new FrameCategoryCRUDResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
            
            
        }


    }
}
