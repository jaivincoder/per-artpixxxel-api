using api.artpixxel.data.Features.GalleryImages;
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Models;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using api.artpixxel.data.Features.FrameCategories;
using System.Text.RegularExpressions;
using api.artpixxel.data.Features.TemplateConfing;
using System.Net.Http;




namespace api.artpixxel.repo.Features.GalleryImages
{
    public class GalleryImageService : IGalleryImageService
    {
        private readonly ArtPixxelContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public GalleryImageService(ArtPixxelContext context, IWebHostEnvironment environment, IConfiguration configuration)
        {
            _context = context;
            _environment = environment;
            _configuration = configuration;
        }
        public async Task<GetGalleryImageListResponse> GetGalleryImages()
        {
            try
            {
                var images = await _context.GalleryImages.OrderBy(g => g.DisplayOrder).ToListAsync();
                var grouped = images
                    .GroupBy(g => g.CategoryId)
                    .Select(g => new GalleryImagesGroupDto
                    {
                        CategoryId = g.Key,

                        galleryImage = g.Select(img => new GalleryImagesItemDto
                        {
                            ImagePath = img.ImageUrl,
                            ImageName = img.ImageName,
                            OrderNo = img.DisplayOrder,
                            IsActive = img.IsActive,
                        }).ToList()
                    }).ToList();
                return new GetGalleryImageListResponse
                {
                    Data = grouped,
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Gallery Images Retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new GetGalleryImageListResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
        }

        public async Task<GetGalleryImageResponse> GetGalleryImageById(int id)
        {
            try
            {
                var image = await _context.GalleryImages.Where(g => g.Id == id).FirstOrDefaultAsync();
                if (image == null)
                {
                    return new GetGalleryImageResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = "Gallery Images not found."
                    };
                } 

                var grouped = new GalleryImagesGroupDto
                {
                    CategoryId = image.CategoryId,
                    galleryImage = new List<GalleryImagesItemDto>
                    {
                        new GalleryImagesItemDto
                        {
                            ImagePath = image.ImageUrl,
                            ImageName = image.ImageName,
                            OrderNo = image.DisplayOrder,
                            IsActive = image.IsActive,
                        }
                    }
                };
                return new GetGalleryImageResponse
                {
                    Data = grouped,
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Gallery Images Retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new GetGalleryImageResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
        }

        public async Task<GalleryImagesCRUDResponse> AddGalleryImage(GalleryImagesModel request)
        {
            try
            {
                var category = await _context.FrameCategories.Where(f => f.Id == request.CategoryId).Select(f => new { f.CategoryType }).FirstOrDefaultAsync();
                if (category == null)
                {
                    return new GalleryImagesCRUDResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = "Invalid category ID."
                    };
                }
                if (category.CategoryType != framecategories.CategoryTypeArtMat)
                {
                    return new GalleryImagesCRUDResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = $"CategoryType must be {framecategories.CategoryTypeArtMat}"
                    };
                }

                if (request.galleryImage == null || !request.galleryImage.Any())
                {
                    return new GalleryImagesCRUDResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = "No images provided."
                    };
                }

                //if (request.ImageUrl == null || !request.ImageUrl.Any())
                //{
                //    return new GalleryImagesCRUDResponse
                //    {
                //        Succeeded = false,
                //        Result = RequestResult.Error,
                //        Message = "No images provided."
                //    };
                //}
                List<GalleryImagesItemDto> data = new List<GalleryImagesItemDto>();
                for (int i = 0; i < request.galleryImage.Count; i++)
                {
                    var imageFile = request.galleryImage[i].ImagePath;
                    var displayOrder = request.galleryImage[i].DisplayOrder;


                    var clean = CleanBase64String(imageFile);

                    string savedPath = await SaveBase64File(clean, "GalleryImage");
                    string fileName = Path.GetFileName(savedPath);

                    var galleryImage = new api.artpixxel.data.Models.GalleryImages
                    {
                        CategoryId = request.CategoryId,
                        IsActive=request.IsActive,
                        ImageName = fileName,
                        ImageUrl = savedPath,
                        DisplayOrder = displayOrder
                    };
                    _context.GalleryImages.Add(galleryImage);

                    data.Add(new GalleryImagesItemDto
                    {
                        ImagePath = savedPath,
                        ImageName= fileName,
                        OrderNo = displayOrder,
                        IsActive = request.IsActive,
                    });
                }
                await _context.SaveChangesAsync();
                var grouped = new GalleryModalAddData
                {
                    CategoryId = request.CategoryId,
                    galleryImage = data
                };

                return new GalleryImagesCRUDResponse
                {
                    Data = grouped,
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Gallery images added successfully"
                };
            }
            catch (Exception ex)
            {
                return new GalleryImagesCRUDResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
        }

        public async Task<GetGalleryImageResponse> UpdateGalleryImage(int id,int CategoryId, GalleryImageUpdateRequest request)
        {
            try
            {
                var existingImage = await _context.GalleryImages.FirstOrDefaultAsync(g => g.Id == id && g.CategoryId == CategoryId);

                if (existingImage == null)
                {
                    return new GetGalleryImageResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = "Gallery Images not found."
                    };
                }

                var category = await _context.FrameCategories.Where(f => f.Id == CategoryId).Select(f => new { f.CategoryType }).FirstOrDefaultAsync();

                if (category == null)
                {
                    return new GetGalleryImageResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = "Invalid CategoryId."
                    };
                }

                if (category.CategoryType != framecategories.CategoryTypeArtMat)
                {
                    return new GetGalleryImageResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = $"CategoryType must be {framecategories.CategoryTypeArtMat}"
                    };
                }

                var ImageURl = "";
                var FileName = "";
                if (request.ImagePath != null)
                {
                    ImageURl = await SaveFile(request.ImagePath);
                    
                    FileName = Path.GetFileName(ImageURl);
                    existingImage.ImageUrl = ImageURl;
                    existingImage.ImageName = FileName;
                    existingImage.IsActive = request.IsActive;
                }
                existingImage.DisplayOrder = request.DisplayOrder;
                _context.GalleryImages.Update(existingImage);
                await _context.SaveChangesAsync();
                var grouped = new GalleryImagesGroupDto
                {
                    CategoryId = CategoryId,
                    galleryImage = new List<GalleryImagesItemDto>
                    {
                        new GalleryImagesItemDto
                        {
                            ImagePath =  existingImage.ImageUrl,
                            ImageName = existingImage.ImageName,
                            OrderNo =  existingImage.DisplayOrder,
                            IsActive = existingImage.IsActive,
                        }
                    }
                };
                return new GetGalleryImageResponse
                {
                    Data = grouped,
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Gallery Images Updated successfully",
                  
                };
                
            }
           catch (Exception ex)
            {
                return new GetGalleryImageResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
        }
        public async Task<GalleryImageCRUDResponse> DeleteGalleryImage(int id)
        {
            try
            {
                var existingImage = await _context.GalleryImages.FirstOrDefaultAsync(g => g.Id == id);
                if (existingImage == null)
                {
                    return new GalleryImageCRUDResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = "Gallery image not found."
                    };
                }
                _context.GalleryImages.Remove(existingImage);
                await _context.SaveChangesAsync();

                return new GalleryImageCRUDResponse
                {
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Gallery Delete successfully"
                };
            }
            catch(Exception ex)
            {
                return new GalleryImageCRUDResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
        }
        private string CleanBase64String(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
                return null;

            // Remove prefix data:image/png;base64,
            if (base64.Contains(","))
                base64 = base64.Substring(base64.IndexOf(",") + 1);

            // Remove whitespace/newlines
            base64 = base64.Trim()
                           .Replace("\r", "")
                           .Replace("\n", "")
                           .Replace(" ", "");

            return base64;
        }
        private async Task<string> SaveBase64File(string base64, string folderName)
        {
            base64 = CleanBase64String(base64);

            if (string.IsNullOrWhiteSpace(base64))
                throw new Exception("Base64 string is empty.");

            byte[] bytes;

            try
            {
                bytes = Convert.FromBase64String(base64);
            }
            catch
            {
                throw new Exception("Invalid Base64 string.");
            }

            var uploadBase = _configuration["FileStorage:UploadFolder"] ?? "";
            var uploadsFolder = Path.Combine(_environment.ContentRootPath, uploadBase, folderName);
            Directory.CreateDirectory(uploadsFolder);

            string fileName = $"{DateTime.Now:ddMMyyyy_HHmmssfff}.png";
            string filePath = Path.Combine(uploadsFolder, fileName);

            await File.WriteAllBytesAsync(filePath, bytes);

            return $"/images/{folderName}/{fileName}";
        }
        private async Task<string> SaveFile(IFormFile file)
        {
            var uploadBase = _configuration["FileStorage:UploadFolder"] ?? "";

            var folderName = "GalleryImage";
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
    }
}
