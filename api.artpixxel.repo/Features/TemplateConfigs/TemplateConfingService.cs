using api.artpixxel.data.Features.FrameCategories;
using api.artpixxel.data.Features.TemplateConfing;
using api.artpixxel.data.Models;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static api.artpixxel.data.Models.Permissions;

namespace api.artpixxel.repo.Features.TemplateConfigs
{
    public class TemplateConfingService : ITemplateCondigsService
    {
        private readonly ArtPixxelContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public TemplateConfingService(ArtPixxelContext context, IWebHostEnvironment environment, IConfiguration configuration)
        {
            _context = context;
            _environment = environment;
            _configuration = configuration;
        }

        public async Task<GetTemplateConfingListResponse> GetTemplatesAsync()
        {
            try
            {
                var query = from tc in _context.TemplateConfigs
                            join fc in _context.FrameCategories
                                on tc.CategoryId equals fc.Id
                            orderby tc.TemplateKey, tc.CategoryId
                            select new
                            {
                                tc.Id,
                                tc.TemplateKey,
                                tc.CategoryId,
                                tc.PreviewImage,
                                tc.Price,
                                fc.CategoryType,
                                fc.Label,
                                tc.IsActive
                            };

                var templates = await query.ToListAsync();

                var templateIds = templates.Select(t => t.Id).ToList();

                var mappings = await _context.TemplateFramesMapping.Where(m => templateIds.Contains(m.TemplateConfigId))
                    .Select(m => new TemplateFrameMappingRequestDto
                    {
                        TemplateConfigId = m.TemplateConfigId,
                        OrderNo = m.OrderNo,
                        FrameId = m.FrameId,
                        IsActive=m.IsActive,
                        SvgPath = m.Frames != null ? m.Frames.SvgPath : string.Empty,
                        Thumbnail = m.Frames != null ? m.Frames.Thumbnail : string.Empty
                    }).ToListAsync();

                var result = templates.Select(t => new TemplateWithCategoryDto
                {
                    Id = t.Id,
                    TemplateKey = t.TemplateKey,
                    CategoryId = t.CategoryId,
                    PreviewImage = t.PreviewImage,
                    Price = t.Price,
                    CategoryType = t.CategoryType,
                    CategoryLabel = t.Label,
                    IsActive = t.IsActive,
                    TemplateFramesMapping = mappings.Where(m => m.TemplateConfigId == t.Id).OrderBy(m => m.OrderNo).ToList()
                }).ToList();

                return new GetTemplateConfingListResponse
                {
                    Data = result,
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Retrived successfully"
                };
            }
            catch(Exception ex)
            {
                return new GetTemplateConfingListResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
        }


        public async Task<GetTemplateConfingResponse> GetTemplatesById(int id)
        {
            try
            {
                var template = await (from tc in _context.TemplateConfigs
                                      join fc in _context.FrameCategories
                                          on tc.CategoryId equals fc.Id
                                      where tc.Id == id
                                      select new
                                      {
                                          tc.Id,
                                          tc.TemplateKey,
                                          tc.CategoryId,
                                          tc.PreviewImage,
                                          tc.Price,
                                          fc.CategoryType,
                                          fc.Label,
                                          tc.IsActive
                                      }).FirstOrDefaultAsync();

                if (template == null)
                {
                    return new GetTemplateConfingResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = $"Template with ID {id} not found."
                    };
                }

                var mappings = await _context.TemplateFramesMapping.Where(m => m.TemplateConfigId == id)
                    .Select(m => new TemplateFrameMappingRequestDto
                    {
                        TemplateConfigId = m.TemplateConfigId,
                        OrderNo = m.OrderNo,
                        FrameId = m.FrameId,
                        IsActive = m.IsActive,
                        SvgPath = m.Frames != null ? m.Frames.SvgPath : string.Empty,
                        Thumbnail = m.Frames != null ? m.Frames.Thumbnail : string.Empty
                    }).ToListAsync();

                var result = new TemplateWithCategoryDto
                {
                    Id = template.Id,
                    TemplateKey = template.TemplateKey,
                    CategoryId = template.CategoryId,
                    PreviewImage = template.PreviewImage,
                    Price = template.Price,
                    CategoryType = template.CategoryType,
                    CategoryLabel = template.Label,
                    IsActive= template.IsActive,
                    TemplateFramesMapping = mappings
                };

                return new GetTemplateConfingResponse
                {
                    Data = result,
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Retrived successfully"
                };
            }
            catch(Exception ex)
            {
                return new GetTemplateConfingResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
        }

        public async Task<GetTemplateConfingResponse> GetTemplatesByTemplateKey(string TemplateKey)
        {
            try
            {
                var template = await (from tc in _context.TemplateConfigs
                                      join fc in _context.FrameCategories
                                          on tc.CategoryId equals fc.Id
                                      where tc.TemplateKey == TemplateKey
                                      select new
                                      {
                                          tc.Id,
                                          tc.TemplateKey,
                                          tc.CategoryId,
                                          tc.PreviewImage,
                                          tc.Price,
                                          fc.CategoryType,
                                          fc.Label,
                                          tc.IsActive
                                      }).FirstOrDefaultAsync();

                if (template == null)
                {
                    return new GetTemplateConfingResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = $"Template with TemplateKey '{TemplateKey}' not found."
                    };
                  
                }

                var mappings = await _context.TemplateFramesMapping.Where(m => m.TemplateConfigId == template.Id)
                    .Select(m => new TemplateFrameMappingRequestDto
                    {
                        TemplateConfigId = m.TemplateConfigId,
                        OrderNo = m.OrderNo,
                        FrameId = m.FrameId,
                        IsActive=m.IsActive,
                        SvgPath = m.Frames != null ? m.Frames.SvgPath : string.Empty,
                        Thumbnail = m.Frames != null ? m.Frames.Thumbnail : string.Empty
                    }).ToListAsync();

                var result = new TemplateWithCategoryDto
                {
                    Id = template.Id,
                    TemplateKey = template.TemplateKey,
                    CategoryId = template.CategoryId,
                    PreviewImage = template.PreviewImage,
                    Price = template.Price,
                    CategoryType = template.CategoryType,
                    CategoryLabel = template.Label,
                    IsActive= template.IsActive,
                    TemplateFramesMapping = mappings
                };

                return new GetTemplateConfingResponse
                {
                    Data = result,
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Retrived successfully"
                };
            }
            catch (Exception ex)
            {
                return new GetTemplateConfingResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
        }

        public async Task<TemplateConfligCRUDResponse> AddTemplateConfigs(TemplateConfingModel request)
        {
            try
            {
                var type = request.TemplateKey;

                var allowedTypes = TemplateConfig.template_key_type1
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (!allowedTypes.Contains(type))
                {
                    return new TemplateConfligCRUDResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = $"Invalid TemplateKey '{type}'. It must be one of: {TemplateConfig.template_key_type1}"
                    };
                }
                var TemKey = _context.TemplateConfigs.Where(x => x.TemplateKey == request.TemplateKey && x.CategoryId == request.CategoryId).Count();
                if (TemKey > 0)
                {
                    return new TemplateConfligCRUDResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = $"Template Key already exists for current category. Please update the record instead of creating a new one."
                    };
                }

                var CatogryId = await _context.FrameCategories.Where(f => f.Id == request.CategoryId).FirstOrDefaultAsync();

                if (CatogryId == null)
                {
                    return new TemplateConfligCRUDResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = $"Frame Category Not Exist"
                    };
                }
                var framesData = request.TemplateFramesMapping ?? new List<TemplateFramesData>();
                if (framesData.Any())
                {
                    var reqIds = framesData.Select(x => x.FrameId).ToList();

                    var existingIds = await _context.Frames
                        .Where(f => reqIds.Contains(f.Id))
                        .Select(f => f.Id)
                        .ToListAsync();

                    var missingIds = reqIds.Except(existingIds).ToList();

                    if (missingIds.Any())
                    {
                        return new TemplateConfligCRUDResponse
                        {
                            Succeeded = false,
                            Result = RequestResult.Error,
                            Message = $"FrameIds do not exist: {string.Join(", ", missingIds)}"
                        };
                    }
                }
                var match = Regex.Match(type, @"setof(\d+)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    int required = int.Parse(match.Groups[1].Value);

                    if (framesData.Count != required)
                    {
                        return new TemplateConfligCRUDResponse
                        {
                            Succeeded = false,
                            Result = RequestResult.Error,
                            Message = $"TemplateKey '{type}' requires exactly {required} frames."
                        };
                    }
                }

                var PreviewImage = "";
                if (request.PreviewImage != null)
                {
                    PreviewImage = await SaveFile(request.PreviewImage);
                }

                var newTemplate = new Template_Configs
                {
                    TemplateKey = request.TemplateKey,
                    CategoryId = request.CategoryId,
                    PreviewImage = PreviewImage,
                    IsActive= request.IsActive,
                    Price = request.Price

                };

                await _context.TemplateConfigs.AddAsync(newTemplate);
                await _context.SaveChangesAsync();

                if (framesData.Any())
                {
                    foreach (var item in framesData)
                    {
                        _context.TemplateFramesMapping.Add(new Template_Frames_Mapping
                        {
                            TemplateConfigId = newTemplate.Id,
                            FrameId = item.FrameId,
                            IsActive=item.IsActive,
                            OrderNo = item.OrderNo
                        });
                    }

                    await _context.SaveChangesAsync();
                }

                List<TemplateFrameMappingRequestDto> _framesResult = new List<TemplateFrameMappingRequestDto>();
                foreach (var item in framesData)
                {
                    var frm = _context.Frames.Where(x => x.Id == item.FrameId).FirstOrDefault();

                    TemplateFrameMappingRequestDto _frameitem = new TemplateFrameMappingRequestDto();
                    _frameitem.FrameId = item.FrameId;
                    _frameitem.OrderNo = item.OrderNo;
                    _frameitem.TemplateConfigId = newTemplate.Id;
                    if (frm != null)
                    {
                        _frameitem.SvgPath = frm.SvgPath;
                        _frameitem.Thumbnail = frm.Thumbnail;
                    }
                    _framesResult.Add(_frameitem);
                }

                var result = new TemplateConfligResponse
                {
                    Id = newTemplate.Id,
                    TemplateKey = newTemplate.TemplateKey,
                    CategoryId = newTemplate.CategoryId,
                    Price = newTemplate.Price,
                    PreviewImage = newTemplate.PreviewImage,
                    IsActive = newTemplate.IsActive,
                    TemplateFramesMapping = _framesResult
                };
                return new TemplateConfligCRUDResponse
                {
                    Data = result,
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Template Added successfully"
                };
            }
            catch (Exception ex)
            {
                return new TemplateConfligCRUDResponse
                {
                      Succeeded = false,
                      Result = RequestResult.Error,
                      Message = ex.Message
               };
            }
        }
        public async Task<TemplateConfligCRUDResponse> UpdateTemplateConfigs(int id, TemplateConfingModel request)
        {
            try
            {
                var template = await _context.TemplateConfigs.FirstOrDefaultAsync(t => t.Id == id );

                if (template == null)
                {
                    return new TemplateConfligCRUDResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = "TemplateConfig not found."
                    };
                }

                var type = request.TemplateKey;
                var allowedTypes = TemplateConfig.template_key_type1.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (!allowedTypes.Contains(type))
                {
                    return new TemplateConfligCRUDResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = $"Invalid TemplateKey '{type}'"
                    };
                }

                var category = await _context.FrameCategories.FirstOrDefaultAsync(c => c.Id == request.CategoryId);

                if (category == null)
                {
                    return new TemplateConfligCRUDResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = "Category not found."
                    };
                }

                var framesData = request.TemplateFramesMapping ?? new List<TemplateFramesData>();

                if (framesData.Any())
                {
                    var reqIds = framesData.Select(x => x.FrameId).ToList();
                    var existingIds = await _context.Frames.Where(f => reqIds.Contains(f.Id)).Select(f => f.Id).ToListAsync();

                    var missingIds = reqIds.Except(existingIds).ToList();
                    if (missingIds.Any())
                    {
                        return new TemplateConfligCRUDResponse
                        {
                            Succeeded = false,
                            Result = RequestResult.Error,
                            Message = $"These FrameIds do not exist: {string.Join(", ", missingIds)}"
                        };
                    }
                }

                var match = Regex.Match(type, @"setof(\d+)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    int required = int.Parse(match.Groups[1].Value);
                    if (framesData.Count != required)
                    {
                        return new TemplateConfligCRUDResponse
                        {
                            Succeeded = false,
                            Result = RequestResult.Error,
                            Message = $"TemplateKey '{type}' requires {required} frames."
                        };
                    }
                }

                string previewImage = template.PreviewImage;
                if (!string.IsNullOrEmpty(request.PreviewImage))
                {
                    if (!request.PreviewImage.Contains(template.PreviewImage))
                    {
                        previewImage = await SaveFile(request.PreviewImage);
                        template.PreviewImage = previewImage;
                    }
                }

                template.TemplateKey = request.TemplateKey;
                template.CategoryId = request.CategoryId;
                template.Price = request.Price;
                template.IsActive = request.IsActive;
                await _context.SaveChangesAsync();

                var oldMappings = await _context.TemplateFramesMapping.Where(m => m.TemplateConfigId == template.Id).ToListAsync();

                if (oldMappings.Any())
                    _context.TemplateFramesMapping.RemoveRange(oldMappings);

                foreach (var item in framesData)
                {
                    _context.TemplateFramesMapping.Add(new Template_Frames_Mapping
                    {
                        TemplateConfigId = template.Id,
                        FrameId = item.FrameId,
                        IsActive = item.IsActive,
                        OrderNo = item.OrderNo
                    });
                }

                await _context.SaveChangesAsync();


                List<TemplateFrameMappingRequestDto> _framesResult = new List<TemplateFrameMappingRequestDto>();
                foreach (var item in framesData)
                {
                    var frm = _context.Frames.Where(x => x.Id == item.FrameId).FirstOrDefault();

                    TemplateFrameMappingRequestDto _frameitem = new TemplateFrameMappingRequestDto();
                    _frameitem.FrameId = item.FrameId;
                    _frameitem.OrderNo = item.OrderNo;
                    _frameitem.TemplateConfigId = template.Id;
                    if (frm != null)
                    {
                        _frameitem.SvgPath = frm.SvgPath;
                        _frameitem.Thumbnail = frm.Thumbnail;
                    }
                    _framesResult.Add(_frameitem);
                }


                var result = new TemplateConfligResponse
                {
                    Id=template.Id,
                    TemplateKey = template.TemplateKey,
                    CategoryId = template.CategoryId,
                    Price = template.Price,
                    PreviewImage = template.PreviewImage,
                    IsActive = template.IsActive,
                    TemplateFramesMapping = _framesResult
                };
                return new TemplateConfligCRUDResponse
                {
                    Data = result,
                    Result = RequestResult.Success,
                    Succeeded = true,
                    Message = "Template updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new TemplateConfligCRUDResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
        }



        public async Task<GetTemplateConfingResponse> DeleteTemplateConfigs(int Id)
        {
            try
            {
                var template = await _context.TemplateConfigs.FirstOrDefaultAsync(tc => tc.Id == Id);

                if (template == null)
                {
                    return new GetTemplateConfingResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = $"Template with ID {Id} not found."
                    };
                }
                var templateFramesMapping = await _context.TemplateFramesMapping.Where(tf => tf.TemplateConfigId == Id).ToListAsync();

                if (templateFramesMapping.Any())
                {
                    _context.TemplateFramesMapping.RemoveRange(templateFramesMapping);
                }

                _context.TemplateConfigs.Remove(template);
                await _context.SaveChangesAsync();
                return new GetTemplateConfingResponse
                {
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Template Deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new GetTemplateConfingResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
        }
        //private async Task<string> SaveFile(string base64String)
        //{
        //    if (string.IsNullOrWhiteSpace(base64String))
        //        return null;

        //    if (base64String.Contains(","))
        //        base64String = base64String.Split(',')[1];

        //    byte[] fileBytes = Convert.FromBase64String(base64String);

        //    var uploadBase = _configuration["FileStorage:UploadFolder"] ?? "";
        //    var folderName = "TemplateConfigs";

        //    string extension = ".png"; 
        //    string originalFileName = $"image{extension}";

        //    var uploadsFolder = Path.Combine(_environment.ContentRootPath, uploadBase, folderName);
        //    Directory.CreateDirectory(uploadsFolder);

        //    var dateTimePrefix = DateTime.Now.ToString("ddMMyyyy_HHmmss");
        //    var fileName = $"{dateTimePrefix}_{originalFileName}";
        //    var filePath = Path.Combine(uploadsFolder, fileName);

        //    await File.WriteAllBytesAsync(filePath, fileBytes);

        //    return $"/images/{folderName}/{fileName}";
        //}

        private async Task<string> SaveFile(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                return null;

            string extension = "";
            string pureBase64 = base64String;

            if (base64String.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                var header = base64String.Split(',')[0];
                var mime = header.Split(':')[1].Split(';')[0];

                if (!mime.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Unsupported file type. Only images allowed.");

                extension = GetExtensionFromMime(mime);
                pureBase64 = base64String.Split(',')[1];
            }

            pureBase64 = pureBase64.Trim();

            byte[] fileBytes;
            try
            {
                fileBytes = Convert.FromBase64String(pureBase64);
            }
            catch
            {
                throw new Exception("Invalid Base64 string.");
            }

            if (string.IsNullOrEmpty(extension))
            {
                extension = DetectImageExtension(fileBytes);

                if (string.IsNullOrEmpty(extension))
                    throw new Exception("Unsupported or invalid image.");
            }

            var uploadBase = _configuration["FileStorage:UploadFolder"] ?? "";
            var folderName = "TemplateConfigs";

            var uploadsFolder = Path.Combine(_environment.ContentRootPath, uploadBase, folderName);
            Directory.CreateDirectory(uploadsFolder);

            string fileName = $"{DateTime.Now:ddMMyyyy_HHmmssfff}{extension}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            await File.WriteAllBytesAsync(filePath, fileBytes);

            return $"/images/{folderName}/{fileName}";
        }

        private string GetExtensionFromMime(string mime)
        {
            return mime.ToLower() switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                "image/gif" => ".gif",
                "image/bmp" => ".bmp",
                "image/svg+xml" => ".svg",
                _ => throw new Exception("Unsupported image type.")
            };
        }

        private string DetectImageExtension(byte[] bytes)
        {
            // JPEG FF D8 FF
            if (bytes.Length > 3 &&
                bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
                return ".jpg";

            // PNG 89 50 4E 47
            if (bytes.Length > 4 &&
                bytes[0] == 0x89 && bytes[1] == 0x50 &&
                bytes[2] == 0x4E && bytes[3] == 0x47)
                return ".png";

            // GIF 47 49 46
            if (bytes.Length > 3 &&
                bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46)
                return ".gif";

            // WEBP "RIFF" + "WEBP"
            if (bytes.Length > 12 &&
                Encoding.ASCII.GetString(bytes, 0, 4) == "RIFF" &&
                Encoding.ASCII.GetString(bytes, 8, 4) == "WEBP")
                return ".webp";

            // SVG (text-based) — check beginning for XML or <svg tag
            try
            {
                int len = Math.Min(bytes.Length, 1024); // inspect up to first 1KB
                string start = Encoding.UTF8.GetString(bytes, 0, len).TrimStart();

                // common patterns: <?xml ... ?> or <svg ...>
                if (start.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase) ||
                    start.IndexOf("<svg", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return ".svg";
                }
            }
            catch
            {
                // ignore decoding errors and fall through
            }

            return null;
        }


        public async Task<GetTemplateConfingListResponse> GetTemplatesByCategoryId(int CategoryId)
        {
            try
            {
                var query = from tc in _context.TemplateConfigs
                            join fc in _context.FrameCategories
                                on tc.CategoryId equals fc.Id
                            where tc.CategoryId == CategoryId 
                            orderby tc.TemplateKey, tc.CategoryId
                            select new
                            {
                                tc.Id,
                                tc.TemplateKey,
                                tc.CategoryId,
                                tc.PreviewImage,
                                tc.Price,
                                fc.CategoryType,
                                fc.Label,
                                fc.IsActive
                            };
                var templates = await query.ToListAsync();
                var templateIds = templates.Select(t => t.Id).ToList();
                if (templateIds.Count() <= 0)
                {
                    return new GetTemplateConfingListResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = $"Template with CategoryId {CategoryId} not found."
                    };
                }

                var mappings = await _context.TemplateFramesMapping.Where(m => templateIds.Contains(m.TemplateConfigId))
                   .Select(m => new TemplateFrameMappingRequestDto
                   {
                       TemplateConfigId = m.TemplateConfigId,
                       OrderNo = m.OrderNo,
                       FrameId = m.FrameId,
                       SvgPath = m.Frames != null ? m.Frames.SvgPath : string.Empty,
                       Thumbnail = m.Frames != null ? m.Frames.Thumbnail : string.Empty
                   }).ToListAsync();

                var result = templates.Select(t => new TemplateWithCategoryDto
                {
                    Id = t.Id,
                    TemplateKey = t.TemplateKey,
                    CategoryId = t.CategoryId,
                    PreviewImage = t.PreviewImage,
                    Price = t.Price,
                    CategoryType = t.CategoryType,
                    CategoryLabel = t.Label,
                    IsActive = t.IsActive,
                    TemplateFramesMapping = mappings.Where(m => m.TemplateConfigId == t.Id).OrderBy(m => m.OrderNo).ToList()
                }).ToList();

                return new GetTemplateConfingListResponse
                {
                    Data = result,
                    Succeeded = true,
                    Result = RequestResult.Success,
                    Message = "Retrived successfully"
                };

            }
            catch (Exception ex)
            {
                return new GetTemplateConfingListResponse
                {
                    Succeeded = false,
                    Result = RequestResult.Error,
                    Message = ex.Message
                };
            }
        }
    }
}
