

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.WallArts;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using api.artpixxel.repo.Extensions;
using api.artpixxel.data.Features.WallArtSizes;
using api.artpixxel.data.Features.WallartCategories;
using api.artpixxel.data.Models;
using System.Collections.Generic;
using api.artpixxel.data.Services;

namespace api.artpixxel.repo.Features.WallArts
{
    public class WallArtService : IWallArtService
    {
        private readonly ArtPixxelContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICurrentUserService _currentUserService;
        public WallArtService(ArtPixxelContext context, IWebHostEnvironment hostingEnvironment, ICurrentUserService currentUserService)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _currentUserService = currentUserService;
        }

        public async Task<WallArtCRUDResponse> BulkCreate(WallArtBulkCreateRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (request.WallArts.Any())
                {
                    List<WallArtImage> wallArtImages = new();
                    WallArtSize wallArtSize = null;
                    if(request.WallArts.Any(e => e.WallartFixedSize == true))
                    {
                        wallArtSize = await _context.WallArtSizes.Where(e => e.IsDefault == true).FirstOrDefaultAsync();
                    }
                    foreach (WallArtRequest wallart in request.WallArts)
                    {
                        if (!string.IsNullOrEmpty(wallart.WallartImage))
                        {
                            decimal amount = 0m;
                            string sizeId = string.Empty;

                            if (wallart.WallartFixedSize)
                            {
                               
                                if (wallArtSize != null)
                                {
                                    amount = wallArtSize.Amount;
                                    sizeId = wallArtSize.Id;
                                }

                                else
                                {
                                    return new WallArtCRUDResponse
                                    {
                                        WallArt = await WallArts(@request.PaginationFilter),
                                        Response = new BaseResponse
                                        {
                                            Message = "No default wallart size defined. Fixed wallart size setup assignment failed",
                                            Result = RequestResult.Error,
                                            Succeeded = false,
                                            Title = "Reference Error"
                                        }
                                    };
                                }

                            }

                            else
                            {
                                WallArtSize _wallArtSize = await _context.WallArtSizes.FindAsync(wallart.WallartSizeSizeId);
                                if (_wallArtSize != null)
                                {
                                    amount = _wallArtSize.Amount;
                                }

                                else
                                {
                                    return new WallArtCRUDResponse
                                    {
                                        WallArt = await WallArts(@request.PaginationFilter),
                                        Response = new BaseResponse
                                        {
                                            Message = "Reference to wall art size could not be resolved. Wallart setup  failed",
                                            Result = RequestResult.Error,
                                            Succeeded = false,
                                            Title = "Reference Error"
                                        }
                                    };
                                }
                            }



                            string outputPath = _hostingEnvironment.WebRootPath + "\\images\\WallArt\\" + wallart.WallartHeader;
                            FileMeta fileMeta = await wallart.WallartImage.SaveBase64AsImage(outputPath);

                            if((!string.IsNullOrEmpty(fileMeta.Path)) && (fileMeta.ImageByte != null) && (!string.IsNullOrEmpty(fileMeta.FileName)))
                            {
                                WallArt wallArt = new()
                                {
                                    Amount = amount,
                                    CategoryId = _context.WallArtCategories.Find(wallart.WallartCategoryCategoryId).Id,
                                    FixedSize = wallart.WallartFixedSize,
                                    Header = wallart.WallartHeader,
                                    Description = wallart.WallartDescription,
                                    ImageURL = fileMeta.Path,
                                    ImageAbsURL = _currentUserService.WebRoot() + "/images/WallArt/" + fileMeta.FileName,
                                    ImageRelURL = "/images/WallArt/" + fileMeta.FileName,
                                    Rating = 1,
                                    WallArtSizeId = wallart.WallartFixedSize ? _context.WallArtSizes.Find(sizeId).Id : _context.WallArtSizes.Find(wallart.WallartSizeSizeId).Id
                                };


                                _context.WallArts.Add(wallArt);
                                await _context.SaveChangesAsync();

                                foreach(WallArtImageBase wallimg in wallart.WallartImages)
                                {



                                    string wallArtImageOutputPath = _hostingEnvironment.WebRootPath + "\\images\\WallArt\\WallArtImages\\" + wallimg.WallartImageName;
                                    FileMeta wallArtImagFileMeta = await wallimg.WallartImageImage.SaveBase64AsImage(wallArtImageOutputPath);
                                    if ((!string.IsNullOrEmpty(wallArtImagFileMeta.Path)) && (wallArtImagFileMeta.ImageByte != null) && (!string.IsNullOrEmpty(wallArtImagFileMeta.FileName)))
                                    {
                                        WallArtImage wallArtImage = new()
                                        {
                                            WallArtId = _context.WallArts.Find(wallArt.Id).Id,
                                            Description = wallimg.WallartImageDescription,
                                            Name = wallimg.WallartImageName,
                                            ImageURL = wallArtImagFileMeta.Path,
                                            ImageRelURL = "/images/WallArt/WallArtImages/" + wallArtImagFileMeta.FileName,
                                            ImageAbsURL = _currentUserService.WebRoot() + "/images/WallArt/WallArtImages/" + wallArtImagFileMeta.FileName,
                                        };

                                        wallArtImages.Add(wallArtImage);
                                    }

                                    
                                }

                             
                            }

                        }

                    }



                    if(wallArtImages.Any())
                    {
                        _context.WallArtImages.AddRange(wallArtImages);
                        int saveRes = await _context.SaveChangesAsync();
                        if(saveRes > 0)
                        {
                            return new WallArtCRUDResponse
                            {
                                WallArt = await WallArts(@request.PaginationFilter),
                                Response = new BaseResponse
                                {
                                    Message = (@request.WallArts.Count == 1 ? "Wallart" : "Wallarts") +" created.",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }
                            };
                        }
                    }



                    return new WallArtCRUDResponse
                    {
                        WallArt = await WallArts(@request.PaginationFilter),
                        Response = new BaseResponse
                        {
                            Message = (@request.WallArts.Count == 1 ? "Wallart" : "Wallarts") + " created with no wall images",
                            Result = RequestResult.Success,
                            Succeeded = true,
                            Title = "Successful"
                        }
                    };



                }

                return new WallArtCRUDResponse
                {
                    WallArt = await WallArts(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = "Wallarts couldn't be created from an empty list. Request rejected.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Empty WallArt List"
                    }
                };

            }
            catch (Exception ex)
            {

                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                return new WallArtCRUDResponse
                {
                    WallArt = await WallArts(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    }
                };
            }
        }

        public async Task<WallArtCRUDResponse> BulkDelete(BulkWallArtDeleteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                if (request.WallartIds.Any())
                {
                    List<WallArt> wallArts = await _context.WallArts.Where(b => @request.WallartIds.Contains(b.Id)).Include(e => e.Images).ToListAsync();
                    if (wallArts.Any())
                    {
                        List<WallArtImage> wallArtImages = new();
                        foreach(WallArt wallArt in wallArts)
                        {
                            await wallArt.ImageURL.DeleteFileFromPathAsync();
                            if (wallArt.Images.Any())
                            {
                                foreach(var wallImage in wallArt.Images)
                                {
                                    await wallImage.ImageURL.DeleteFileFromPathAsync();
                                    wallArtImages.Add(wallImage);
                                }
                            }
                        }

                        if (wallArtImages.Any())
                        {
                            _context.WallArtImages.RemoveRange(wallArtImages);
                            await _context.SaveChangesAsync();
                        }

                        _context.WallArts.RemoveRange(wallArts);
                        int deleteResult = await _context.SaveChangesAsync();
                        if(deleteResult > 0)
                        {

                            return new WallArtCRUDResponse
                            {
                                WallArt = await WallArts(@request.PaginationFilter),
                                Response = new BaseResponse
                                {
                                    Message = (wallArts.Count == 1 ? "Wallart " : "Wallarts ") + "removed.",
                                    Result = RequestResult.Success,
                                    Succeeded = false,
                                    Title = "Successful"
                                }
                            };

                        }




                    }
                }


                return new WallArtCRUDResponse
                {
                    WallArt = await WallArts(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = "No wall art referenced.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Empty Object List"
                    }
                };


            }
            catch (Exception ex)
            {

                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                return new WallArtCRUDResponse
                {
                    WallArt = await WallArts(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    }
                };


            }
        }

        public async Task<WallArtCRUDResponse> BulkDeleteWallImages(BulkWallArtImageDeleteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (request.WallImageIds.Any())
                {
                    List<WallArtImage> wallArtImages = await _context.WallArtImages.Where(b => @request.WallImageIds.Contains(b.Id)).ToListAsync();
                    if (wallArtImages.Any())
                    {
                        foreach(WallArtImage wallArtImage in wallArtImages)
                        {
                            await wallArtImage.ImageURL.DeleteFileFromPathAsync();
                        }

                        _context.WallArtImages.RemoveRange(wallArtImages);
                        int deleteResult = await _context.SaveChangesAsync();

                        if(deleteResult > 0)
                        {
                            return new WallArtCRUDResponse
                            {
                                WallArt = await WallArts(@request.PaginationFilter),
                                Response = new BaseResponse
                                {
                                    Message = wallArtImages.Count == 1 ? "Wall image deleted ": "Wall images deleted.",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }
                            };
                        }
                    }


                }



                return new WallArtCRUDResponse
                {
                    WallArt = await WallArts(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = "No wall image(s) referenced.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Empty Object List"
                    }
                };


            }
            catch (Exception ex)
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                return new WallArtCRUDResponse
                {
                    WallArt = await WallArts(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    }
                };
            }
        }

        public async Task<WallArtCRUDResponse> BulkUpdate(BulkWallArtUpdateRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (request.WallartIds.Any())
                {
                    List<WallArt> wallArts = await _context.WallArts.Where(b => @request.WallartIds.Contains(b.Id)).ToListAsync();
                    if (wallArts.Any())
                    {
                        WallArtSize defaultSize = null;
                        WallArtSize chosenSize = null;
                        if (request.Value.FixedSize.GetValueOrDefault() == true)
                        {
                            defaultSize = await _context.WallArtSizes.Where(e => e.IsDefault == true).FirstOrDefaultAsync();
                        }

                        else
                        {
                            chosenSize = await _context.WallArtSizes.FindAsync(request.Value.SizeId);

                        }

                        foreach(WallArt wallart in wallArts)
                        {
                            wallart.Description = string.IsNullOrEmpty(@request.Value.Description) ? wallart.Description : @request.Value.Description;
                            wallart.WallArtSizeId =  defaultSize != null ? _context.WallArtSizes.Find(defaultSize.Id).Id : string.IsNullOrEmpty(@request.Value.SizeId) ? wallart.WallArtSizeId : _context.WallArtSizes.Find(@request.Value.SizeId).Id;
                            wallart.FixedSize = chosenSize != null ? chosenSize.IsDefault :  @request.Value.FixedSize.HasValue ? request.Value.FixedSize.GetValueOrDefault() : wallart.FixedSize;
                            wallart.CategoryId = string.IsNullOrEmpty(request.Value.CategoryId) ? wallart.CategoryId : _context.WallArtCategories.Find(request.Value.CategoryId).Id;
                        }

                        _context.WallArts.UpdateRange(wallArts);
                        int updateResult = await _context.SaveChangesAsync();
                        if(updateResult > 0)
                        {
                            return new WallArtCRUDResponse
                            {
                                WallArt = await WallArts(@request.PaginationFilter),
                                Response = new BaseResponse
                                {
                                    Message = (wallArts.Count == 1 ? "Wallart ": "Wallarts ") + "updated",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }
                            };
                        }
                    }
                }


                return new WallArtCRUDResponse
                {
                    WallArt = await WallArts(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = "Couldn't delete an empty list. Request was terminated",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Empty Wallart List"
                    }
                };
            }
            catch (Exception ex)
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                return new WallArtCRUDResponse
                {
                    WallArt = await WallArts(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    }
                };
            }
        }

        public async Task<WallArtCategorySize> CategorySize()
        {
            try
            {
                return new WallArtCategorySize
                {
                    Categories = await _context.WallArtCategories.OrderBy(n => n.Name).Include(w => w.WallArts).Select(c => new WallArtCategoryOption
                    {
                        Id = c.Id,
                        Name = c.Name,
                        WallArtCount = decimal.Round(c.WallArts.Count(), 0, MidpointRounding.AwayFromZero)

                    }).ToListAsync(),
                    Sizes = await _context.WallArtSizes.OrderBy(s => s.Name).Include(w => w.WallArts).Select(s => new WallArtSizeOption
                    {
                        Id = s.Id,
                        Name = s.Name,
                        IsDefault = s.IsDefault,
                        Amount = decimal.Round(s.Amount, 2, MidpointRounding.AwayFromZero),
                        WallArtCount = decimal.Round(s.WallArts.Count(), 0, MidpointRounding.AwayFromZero)
                    }).ToListAsync()
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<WallArtCRUDResponse> Create(WallArtCreateRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (!string.IsNullOrEmpty(@request.WallArt.WallartImage))
                {
                    decimal amount = 0m;
                    string sizeId = string.Empty;

                    if (request.WallArt.WallartFixedSize)
                    {
                        WallArtSize wallArtSize = await _context.WallArtSizes.Where(e => e.IsDefault == true).FirstOrDefaultAsync();
                        if (wallArtSize != null)
                        {
                            amount = wallArtSize.Amount;
                            sizeId = wallArtSize.Id;
                        }

                        else
                        {
                            return new WallArtCRUDResponse
                            {
                                WallArt = await WallArts(@request.PaginationFilter),
                                Response = new BaseResponse
                                {
                                    Message = "No default wallart size defined. Fixed wallart size setup assignment failed",
                                    Result = RequestResult.Error,
                                    Succeeded = false,
                                    Title = "Reference Error"
                                }
                            };
                        }

                    }

                    else
                    {
                        WallArtSize wallArtSize = await _context.WallArtSizes.FindAsync(@request.WallArt.WallartSizeSizeId);
                        if (wallArtSize != null)
                        {
                            amount = wallArtSize.Amount;
                        }

                        else
                        {
                            return new WallArtCRUDResponse
                            {
                                WallArt = await WallArts(@request.PaginationFilter),
                                Response = new BaseResponse
                                {
                                    Message = "Reference to wall art size could not be resolved. Wallart setup  failed",
                                    Result = RequestResult.Error,
                                    Succeeded = false,
                                    Title = "Reference Error"
                                }
                            };
                        }
                    }



                    string outputPath = _hostingEnvironment.WebRootPath + "\\images\\WallArt\\" + @request.WallArt.WallartHeader;
                    FileMeta fileMeta = await @request.WallArt.WallartImage.SaveBase64AsImage(outputPath);

                    if ((!string.IsNullOrEmpty(fileMeta.Path)) && (fileMeta.ImageByte != null) && (!string.IsNullOrEmpty(fileMeta.FileName)))

                    {


                    WallArt wallArt = new()
                    {
                        Amount = amount,
                        CategoryId = _context.WallArtCategories.Find(@request.WallArt.WallartCategoryCategoryId).Id,
                        FixedSize = @request.WallArt.WallartFixedSize,
                        Header = @request.WallArt.WallartHeader,
                        Description = @request.WallArt.WallartDescription,
                        ImageURL = fileMeta.Path,
                        ImageAbsURL = _currentUserService.WebRoot() + "/images/WallArt/" + fileMeta.FileName,
                        ImageRelURL = "/images/WallArt/" + fileMeta.FileName,
                        Rating = 1,
                        WallArtSizeId = @request.WallArt.WallartFixedSize ? _context.WallArtSizes.Find(sizeId).Id : _context.WallArtSizes.Find(@request.WallArt.WallartSizeSizeId).Id
                    };

                    _context.WallArts.Add(wallArt);
                    int saveResult = await _context.SaveChangesAsync();

                    if (saveResult > 0)
                    {
                        string wallArtId = wallArt.Id;
                        if (request.WallArt.WallartImages.Any())
                        {
                            List<WallArtImage> wallArtImages = new();
                            foreach (WallArtImageBase wallartImage in request.WallArt.WallartImages)
                            {
                                string wallArtImageOutputPath = _hostingEnvironment.WebRootPath + "\\images\\WallArt\\WallArtImages\\" + wallartImage.WallartImageName;
                                FileMeta wallArtImagFileMeta = await wallartImage.WallartImageImage.SaveBase64AsImage(wallArtImageOutputPath);
                                if((!string.IsNullOrEmpty(wallArtImagFileMeta.Path)) && (wallArtImagFileMeta.ImageByte != null) && (!string.IsNullOrEmpty(wallArtImagFileMeta.FileName)))
                                    {
                                        WallArtImage wallArtImage = new()
                                        {
                                            WallArtId = _context.WallArts.Find(wallArtId).Id,
                                            Description = wallartImage.WallartImageDescription,
                                            Name = wallartImage.WallartImageName,
                                            ImageURL = wallArtImagFileMeta.Path,
                                            ImageRelURL = "/images/WallArt/WallArtImages/" + wallArtImagFileMeta.FileName,
                                            ImageAbsURL = _currentUserService.WebRoot() + "/images/WallArt/WallArtImages/" + wallArtImagFileMeta.FileName,
                                        };

                                        wallArtImages.Add(wallArtImage);
                                    }
                               
                            }



                            if (wallArtImages.Any())
                            {
                                _context.WallArtImages.AddRange(wallArtImages);
                                int wallImagesSaveResult = await _context.SaveChangesAsync();

                                if (wallImagesSaveResult > 0)
                                {
                                    return new WallArtCRUDResponse
                                    {
                                        WallArt = await WallArts(@request.PaginationFilter),
                                        Response = new BaseResponse
                                        {
                                            Message = "Wall art created.",
                                            Result = RequestResult.Success,
                                            Succeeded = true,
                                            Title = "Successful"
                                        }
                                    };
                                }

                                else
                                {
                                    return new WallArtCRUDResponse
                                    {
                                        WallArt = await WallArts(@request.PaginationFilter),
                                        Response = new BaseResponse
                                        {
                                            Message = "Wall art created, but wall art images created failed",
                                            Result = RequestResult.Error,
                                            Succeeded = false,
                                            Title = "Images Creation Failure"
                                        }
                                    };
                                }
                            }


                        }


                        return new WallArtCRUDResponse
                        {
                            WallArt = await WallArts(@request.PaginationFilter),
                            Response = new BaseResponse
                            {
                                Message = "Wall art created.",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            }
                        };


                    }

                }



                }


                return new WallArtCRUDResponse
                {
                    WallArt = await WallArts(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = "Wall art without an image could not be created",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "File Error"
                    }
                };

            }
            catch (Exception ex)
            {

                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                return new WallArtCRUDResponse
                {
                    WallArt = await WallArts(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    }
                };
            }
        }

        public async Task<WallArtCRUDResponse> CreateWallImage(WallArtImageRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                string wallArtImageOutputPath = _hostingEnvironment.WebRootPath + "\\images\\WallArt\\WallArtImages\\" + @request.WallArtImage.WallartImageName;
                FileMeta wallArtImagFileMeta = await @request.WallArtImage.WallartImageImage.SaveBase64AsImage(wallArtImageOutputPath);

                if(wallArtImagFileMeta.ImageByte != null && (!string.IsNullOrEmpty(wallArtImagFileMeta.Path)) && (!string.IsNullOrEmpty(wallArtImagFileMeta.FileName)))
                {
                    WallArtImage wallArtImage = new()
                    {
                        WallArtId = _context.WallArts.Find(@request.WallArtImage.WallartImageWallArtId).Id,
                        Description = @request.WallArtImage.WallartImageDescription,
                        Name = @request.WallArtImage.WallartImageName,
                        ImageURL = wallArtImagFileMeta.Path,
                        ImageRelURL = "/images/WallArt/WallArtImages/" + wallArtImagFileMeta.FileName,
                        ImageAbsURL = _currentUserService.WebRoot() + "/images/WallArt/WallArtImages/" + wallArtImagFileMeta.FileName
                    };

                    _context.WallArtImages.Add(wallArtImage);
                    int saveResult = await _context.SaveChangesAsync();

                    if (saveResult > 0)
                    {
                        return new WallArtCRUDResponse
                        {
                            WallArt = await WallArts(@request.PaginationFilter),
                            Response = new BaseResponse
                            {
                                Message = "Wall image created.",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            }
                        };

                    }
                }
                

            




                return new WallArtCRUDResponse
                {
                    WallArt = await WallArts(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = "An Error Occurred",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Error"
                    }
                };


            }
            catch (Exception ex)
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                return new WallArtCRUDResponse
                {
                    WallArt = await WallArts(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    }
                };
            }
        }

        public  async Task<WallArtCRUDResponse> Delete(PaginatedDeleteRequest request)
        {
            try
            {

                if(await _context.WallArts.AnyAsync(e => e.Id == request.Id.Id))
                {

                    WallArt wallArt = await _context.WallArts.Where(e => e.Id == request.Id.Id).Include(e => e.Images).FirstOrDefaultAsync();
                    if(wallArt != null)
                    {
                        if (wallArt.Images.Any())
                        {
                            foreach(var wallImage in wallArt.Images)
                            {
                              await  wallImage.ImageURL.DeleteFileFromPathAsync();
                            }

                            _context.WallArtImages.RemoveRange(wallArt.Images);
                            await _context.SaveChangesAsync();

                        }

                        await wallArt.ImageURL.DeleteFileFromPathAsync();
                        _context.WallArts.Remove(wallArt);
                        int deleteResult =  await _context.SaveChangesAsync();
                       
                            return new WallArtCRUDResponse
                            {
                                WallArt = await WallArts(@request.PaginationFilter),
                                Response = new BaseResponse
                                {
                                    Message = "Wallart successfully removed",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }
                            };

                        
                    }


                }


                return new WallArtCRUDResponse
                {
                    WallArt = await WallArts(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = "Reference to wallart could not be resolved. This wallart may have been delted",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Null WallArt reference"
                    }
                };



            }
            catch (Exception ex)
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);


                return new WallArtCRUDResponse
                {
                    WallArt = await WallArts(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    }
                };
            }
        }

        public async Task<WallArtCRUDResponse> DeleteWallImage(PaginatedDeleteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.WallArtImages.AnyAsync(e => e.Id == @request.Id.Id))
                {


                    WallArtImage wallArtImage = await _context.WallArtImages.FindAsync(@request.Id.Id);
                    if(wallArtImage != null)
                    {
                       await wallArtImage.ImageURL.DeleteFileFromPathAsync();
                        _context.WallArtImages.Remove(wallArtImage);
                        int deleteResult = await _context.SaveChangesAsync();

                        if(deleteResult > 0)
                        {

                            return new WallArtCRUDResponse
                            {
                                WallArt = await WallArts(@request.PaginationFilter),
                                Response = new BaseResponse
                                {
                                    Message = "Wallart image removed",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }
                            };
                        }

                    }

                }



                return new WallArtCRUDResponse
                {
                    WallArt = await WallArts(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = "Reference to wallart image could not be resolved. This wallart image mya have been deleted",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Null Object Reference"
                    }
                };
            }
            catch (Exception ex)
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                return new WallArtCRUDResponse
                {
                    WallArt = await WallArts(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    }
                };
            }
        }

        public async Task<PublicWallArtBase> PublicLoadMore(PublicWallArtFilter request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                List<PublicWallArt> wallArts = new();

                if ((string.IsNullOrEmpty(request.Search)) && (string.IsNullOrEmpty(request.Category)) && (string.IsNullOrEmpty(request.Hashtag)))
                {
                    wallArts = await _context.WallArts
                   .OrderByDescending(e => e.CreatedOn)
                   .Skip(@request.Pagination.Skip).Take(@request.Pagination.PageSize)
                   .Include(e => e.Images)
                   .Include(c => c.Category)
                   .Include(s => s.WallArtSize).Select(w => new PublicWallArt
                   {
                       Id = w.Id,
                       Amount = w.WallArtSize.Amount,
                       TotalAmount = w.WallArtSize.Amount,
                       Category = w.Category.Name,
                       Quantity = 1,
                       Description = w.Description,
                       Favourite = false,
                       FixedSize = w.FixedSize,  // w.WallArtSize.IsDefault,
                       FrameClass = w.FixedSize ? string.Empty : FrameClass.ELEGANT,
                       Header = w.Header,
                       Image = w.ImageAbsURL,
                       Key = string.Empty,
                       Rating = 5,
                       Size = w.WallArtSize.Id,
                       Images = w.Images.Select(i => new ImageModel
                       {
                           Id = i.Id,
                           Name = i.Name,
                           Image = i.ImageAbsURL,
                           Price = 0m,
                           Description = i.Description,
                       }).ToList()



                   }).ToListAsync();
                }


                else if ((!string.IsNullOrEmpty(request.Search)) && (string.IsNullOrEmpty(request.Category)) && (string.IsNullOrEmpty(request.Hashtag)))
                {
                    wallArts = await _context.WallArts
                   .Where(e => e.Description.Contains(@request.Search) || (e.Header.Contains(@request.Search)) || (e.Category.Name.Contains(@request.Search)))
                   .OrderByDescending(e => e.CreatedOn)
                   .Skip(@request.Pagination.Skip).Take(@request.Pagination.PageSize)
                   .Include(e => e.Images)
                   .Include(c => c.Category)
                   .Include(s => s.WallArtSize).Select(w => new PublicWallArt
                   {
                       Id = w.Id,
                       Amount = w.WallArtSize.Amount,
                       TotalAmount = w.WallArtSize.Amount,
                       Category = w.Category.Name,
                       Quantity = 1,
                       Description = w.Description,
                       Favourite = false,
                       FixedSize = w.FixedSize, // w.WallArtSize.IsDefault,
                       FrameClass = w.FixedSize ? string.Empty : FrameClass.ELEGANT,
                       Header = w.Header,
                       Image = w.ImageAbsURL,
                       Key = string.Empty,
                       Rating = 5,
                       Size = w.WallArtSize.Id,
                       Images = w.Images.Select(i => new ImageModel
                       {
                           Id = i.Id,
                           Name = i.Name,
                           Image = i.ImageAbsURL,
                           Price = 0m,
                           Description = i.Description,
                       }).ToList()



                   }).ToListAsync();
                }

                else if ((string.IsNullOrEmpty(request.Search)) && (!string.IsNullOrEmpty(request.Category)) && (string.IsNullOrEmpty(request.Hashtag)))
                {
                    wallArts = await _context.WallArts
                   .Where(e => e.Category.Name == @request.Category)
                   .OrderByDescending(e => e.CreatedOn)
                   .Skip(@request.Pagination.Skip).Take(@request.Pagination.PageSize)
                   .Include(e => e.Images)
                   .Include(c => c.Category)
                   .Include(s => s.WallArtSize).Select(w => new PublicWallArt
                   {
                       Id = w.Id,
                       Amount = w.WallArtSize.Amount,
                       TotalAmount = w.WallArtSize.Amount,
                       Category = w.Category.Name,
                       Quantity = 1,
                       Description = w.Description,
                       Favourite = false,
                       FixedSize = w.FixedSize, // w.WallArtSize.IsDefault,
                       FrameClass = w.FixedSize ? string.Empty : FrameClass.ELEGANT,
                       Header = w.Header,
                       Image = w.ImageAbsURL,
                       Key = string.Empty,
                       Rating = 5,
                       Size = w.WallArtSize.Id,
                       Images = w.Images.Select(i => new ImageModel
                       {
                           Id = i.Id,
                           Name = i.Name,
                           Image = i.ImageAbsURL,
                           Price = 0m,
                           Description = i.Description,
                       }).ToList()



                   }).ToListAsync();

                }
                else if ((string.IsNullOrEmpty(request.Search)) && (string.IsNullOrEmpty(request.Category)) && (!string.IsNullOrEmpty(request.Hashtag)))
                {
                   wallArts = await _context.WallArts
                  .Where(e => e.Description.Contains(@request.Hashtag))
                  .OrderByDescending(e => e.CreatedOn)
                  .Skip(@request.Pagination.Skip).Take(@request.Pagination.PageSize)
                  .Include(e => e.Images)
                  .Include(c => c.Category)
                  .Include(s => s.WallArtSize).Select(w => new PublicWallArt
                  {
                      Id = w.Id,
                      Amount = w.WallArtSize.Amount,
                      TotalAmount = w.WallArtSize.Amount,
                      Category = w.Category.Name,
                      Quantity = 1,
                      Description = w.Description,
                      Favourite = false,
                      FixedSize = w.FixedSize, // w.WallArtSize.IsDefault,
                      FrameClass = w.FixedSize ? string.Empty : FrameClass.ELEGANT,
                      Header = w.Header,
                      Image = w.ImageAbsURL,
                      Key = string.Empty,
                      Rating = 5,
                      Size = w.WallArtSize.Id,
                      Images = w.Images.Select(i => new ImageModel
                      {
                          Id = i.Id,
                          Name = i.Name,
                          Image = i.ImageAbsURL,
                          Price = 0m,
                          Description = i.Description,
                      }).ToList()



                  }).ToListAsync();
                }

                else if ((!string.IsNullOrEmpty(request.Search)) && (!string.IsNullOrEmpty(request.Category)) && (string.IsNullOrEmpty(request.Hashtag)))
                {
                    wallArts = await _context.WallArts
                  .Where(e => e.Description.Contains(@request.Search) || (e.Header.Contains(@request.Search)) || (e.Category.Name.Contains(@request.Search)) || (e.Category.Name == @request.Category))
                  .OrderByDescending(e => e.CreatedOn)
                  .Skip(@request.Pagination.Skip).Take(@request.Pagination.PageSize)
                  .Include(e => e.Images)
                  .Include(c => c.Category)
                  .Include(s => s.WallArtSize).Select(w => new PublicWallArt
                  {
                      Id = w.Id,
                      Amount = w.WallArtSize.Amount,
                      TotalAmount = w.WallArtSize.Amount,
                      Category = w.Category.Name,
                      Quantity = 1,
                      Description = w.Description,
                      Favourite = false,
                      FixedSize = w.FixedSize,  // w.WallArtSize.IsDefault,
                      FrameClass = w.FixedSize ? string.Empty : FrameClass.ELEGANT,
                      Header = w.Header,
                      Image = w.ImageAbsURL,
                      Key = string.Empty,
                      Rating = 5,
                      Size = w.WallArtSize.Id,
                      Images = w.Images.Select(i => new ImageModel
                      {
                          Id = i.Id,
                          Name = i.Name,
                          Image = i.ImageAbsURL,
                          Price = 0m,
                          Description = i.Description,
                      }).ToList()



                  }).ToListAsync();
                }
                else if ((string.IsNullOrEmpty(request.Search)) && (!string.IsNullOrEmpty(request.Category)) && (!string.IsNullOrEmpty(request.Hashtag)))
                {
                    wallArts = await _context.WallArts
                 .Where(e => e.Description.Contains(@request.Hashtag) || (e.Category.Name == @request.Category))
                 .OrderByDescending(e => e.CreatedOn)
                 .Skip(@request.Pagination.Skip).Take(@request.Pagination.PageSize)
                 .Include(e => e.Images)
                 .Include(c => c.Category)
                 .Include(s => s.WallArtSize).Select(w => new PublicWallArt
                 {
                     Id = w.Id,
                     Amount = w.WallArtSize.Amount,
                     TotalAmount = w.WallArtSize.Amount,
                     Category = w.Category.Name,
                     Quantity = 1,
                     Description = w.Description,
                     Favourite = false,
                     FixedSize = w.FixedSize,  // w.WallArtSize.IsDefault,
                     FrameClass = w.FixedSize ? string.Empty : FrameClass.ELEGANT,
                     Header = w.Header,
                     Image = w.ImageAbsURL,
                     Key = string.Empty,
                     Rating = 5,
                     Size = w.WallArtSize.Id,
                     Images = w.Images.Select(i => new ImageModel
                     {
                         Id = i.Id,
                         Name = i.Name,
                         Image = i.ImageAbsURL,
                         Price = 0m,
                         Description = i.Description,
                     }).ToList()



                 }).ToListAsync();
                }

                else if ((!string.IsNullOrEmpty(request.Search)) && (string.IsNullOrEmpty(request.Category)) && (!string.IsNullOrEmpty(request.Hashtag)))
                {
                    wallArts = await _context.WallArts
                 .Where(e => e.Description.Contains(@request.Search) || (e.Header.Contains(@request.Search)) || (e.Category.Name.Contains(@request.Search)) || (e.Description.Contains(@request.Hashtag)))
                 .OrderByDescending(e => e.CreatedOn)
                 .Skip(@request.Pagination.Skip).Take(@request.Pagination.PageSize)
                 .Include(e => e.Images)
                 .Include(c => c.Category)
                 .Include(s => s.WallArtSize).Select(w => new PublicWallArt
                 {
                     Id = w.Id,
                     Amount = w.WallArtSize.Amount,
                     TotalAmount = w.WallArtSize.Amount,
                     Category = w.Category.Name,
                     Quantity = 1,
                     Description = w.Description,
                     Favourite = false,
                     FixedSize = w.FixedSize,  // w.WallArtSize.IsDefault,
                     FrameClass = w.FixedSize ? string.Empty : FrameClass.ELEGANT,
                     Header = w.Header,
                     Image = w.ImageAbsURL,
                     Key = string.Empty,
                     Rating = 5,
                     Size = w.WallArtSize.Id,
                     Images = w.Images.Select(i => new ImageModel
                     {
                         Id = i.Id,
                         Name = i.Name,
                         Image = i.ImageAbsURL,
                         Price = 0m,
                         Description = i.Description,
                     }).ToList()



                 }).ToListAsync();
                }

                else if ((!string.IsNullOrEmpty(request.Search)) && (!string.IsNullOrEmpty(request.Category)) && (!string.IsNullOrEmpty(request.Hashtag)))
                {
                    wallArts = await _context.WallArts
               .Where(e => e.Description.Contains(@request.Search) || (e.Header.Contains(@request.Search)) || (e.Category.Name.Contains(@request.Search)) || (e.Description.Contains(@request.Hashtag)) || (e.Category.Name == @request.Category))
               .OrderByDescending(e => e.CreatedOn)
               .Skip(@request.Pagination.Skip).Take(@request.Pagination.PageSize)
               .Include(e => e.Images)
               .Include(c => c.Category)
               .Include(s => s.WallArtSize).Select(w => new PublicWallArt
               {
                   Id = w.Id,
                   Amount = w.WallArtSize.Amount,
                   TotalAmount = w.WallArtSize.Amount,
                   Category = w.Category.Name,
                   Quantity = 1,
                   Description = w.Description,
                   Favourite = false,
                   FixedSize = w.FixedSize, // w.WallArtSize.IsDefault,
                   FrameClass = w.FixedSize ? string.Empty : FrameClass.ELEGANT,
                   Header = w.Header,
                   Image = w.ImageAbsURL,
                   Key = string.Empty,
                   Rating = 5,
                   Size = w.WallArtSize.Id,
                   Images = w.Images.Select(i => new ImageModel
                   {
                       Id = i.Id,
                       Name = i.Name,
                       Image = i.ImageAbsURL,
                       Price = 0m,
                       Description = i.Description,
                   }).ToList()



               }).ToListAsync();
                }


                return new PublicWallArtBase
                {
                    WallArts = wallArts
                };


            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<PublicWallartInit> PublicFilter(PublicWallArtFilter request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                List<PublicWallArt> wallArts = new List<PublicWallArt>();

                if (!string.IsNullOrEmpty(@request.Category))
                {
                   wallArts = await _context.WallArts
                   .Where(e => e.Category.Name == @request.Category)
                   .OrderByDescending(e => e.CreatedOn)
                   .Skip(@request.Pagination.Skip).Take(@request.Pagination.PageSize)
                   .Include(e => e.Images)
                   .Include(c => c.Category)
                   .Include(s => s.WallArtSize).Select(w => new PublicWallArt
                   {
                       Id = w.Id,
                       Amount = w.WallArtSize.Amount,
                       TotalAmount = w.WallArtSize.Amount,
                       Category = w.Category.Name,
                       Quantity = 1,
                       Description = w.Description,
                       Favourite = false,
                       FixedSize = w.FixedSize, // w.WallArtSize.IsDefault,
                       FrameClass = w.FixedSize ? string.Empty : FrameClass.ELEGANT,
                       Header = w.Header,
                       Image = w.ImageAbsURL,
                       Key = string.Empty,
                       Rating = 5,
                       Size = w.WallArtSize.Id,
                       Images = w.Images.Select(i => new ImageModel
                       {
                           Id = i.Id,
                           Name = i.Name,
                           Image = i.ImageAbsURL,
                           Price = 0m,
                           Description = i.Description,
                       }).ToList()



                   }).ToListAsync();
                }


               else if (!string.IsNullOrEmpty(@request.Hashtag))
                {
                    wallArts = await _context.WallArts
                   .Where(e => e.Description.Contains(@request.Hashtag))
                   .OrderByDescending(e => e.CreatedOn)
                   .Skip(@request.Pagination.Skip).Take(@request.Pagination.PageSize)
                   .Include(e => e.Images)
                   .Include(c => c.Category)
                   .Include(s => s.WallArtSize).Select(w => new PublicWallArt
                   {
                       Id = w.Id,
                       Amount = w.WallArtSize.Amount,
                       TotalAmount = w.WallArtSize.Amount,
                       Category = w.Category.Name,
                       Quantity = 1,
                       Description = w.Description,
                       Favourite = false,
                       FixedSize = w.FixedSize, // w.WallArtSize.IsDefault,
                       FrameClass = w.FixedSize ? string.Empty : FrameClass.ELEGANT,
                       Header = w.Header,
                       Image = w.ImageAbsURL,
                       Key = string.Empty,
                       Rating = 5,
                       Size = w.WallArtSize.Id,
                       Images = w.Images.Select(i => new ImageModel
                       {
                           Id = i.Id,
                           Name = i.Name,
                           Image = i.ImageAbsURL,
                           Price = 0m,
                           Description = i.Description,
                       }).ToList()



                   }).ToListAsync();
                }

               


              

                return new PublicWallartInit
                {
                    WallArts = wallArts,

                    Sizes = await _context.WallArtSizes.Select(s => new PublicWallArtSize
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Amount = decimal.Round(s.Amount, 2, MidpointRounding.AwayFromZero)


                    }).ToListAsync(),
                    Categories = await _context.WallArtCategories.Select(e => new PublicWallArtCategory
                    {
                        Image = string.IsNullOrEmpty(e.ImageURL) ? AssetDefault.DefaultImage : e.ImageAbsURL,
                        Label = e.Name

                    }).ToListAsync(),
                };


            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<PublicWallartInit> PublicInit(Pagination pagination)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@pagination", pagination);


                return new PublicWallartInit
                {
                    Sizes = await _context.WallArtSizes.Select(s => new PublicWallArtSize
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Amount = decimal.Round(s.Amount, 2 ,MidpointRounding.AwayFromZero)
                       

                    }).ToListAsync(),
                    Categories = await _context.WallArtCategories.Select(e => new PublicWallArtCategory
                    {
                        Image = string.IsNullOrEmpty(e.ImageURL)  ? AssetDefault.DefaultImage: e.ImageAbsURL,
                        Label = e.Name

                    }).ToListAsync(),

                    WallArts = await _context.WallArts
                    .OrderByDescending(e => e.CreatedOn)
                    .Skip(@pagination.Skip).Take(@pagination.PageSize)
                    .Include(e => e.Images)
                    .Include(c => c.Category)
                    .Include(s => s.WallArtSize).Select(w => new PublicWallArt
                    {
                        Id = w.Id,
                        Amount = w.WallArtSize.Amount,
                        TotalAmount = w.WallArtSize.Amount,
                        Category = w.Category.Name,
                        Quantity = 1,
                        Description = w.Description,
                        Favourite = false,
                        FixedSize = w.FixedSize, // w.WallArtSize.IsDefault,
                        FrameClass = w.FixedSize ? string.Empty : FrameClass.ELEGANT,
                        Header = w.Header,
                        Image = w.ImageAbsURL,
                        Key = string.Empty,
                        Rating = 5,
                        Size = w.WallArtSize.Id,
                        Images = w.Images.Select( i => new ImageModel
                        {
                            Id = i.Id,
                            Name = i.Name,
                            Image = i.ImageAbsURL,
                            Price = 0m,
                            Description = i.Description,
                        }).ToList()
                        
                     

                    }).ToListAsync()
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<PublicWallArtBase> PublicSearch(PublicWallArtFilter request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                //search

                return new PublicWallArtBase
                {
                    WallArts = string.IsNullOrEmpty(@request.Search) ?

                    await _context.WallArts
                    .OrderByDescending(e => e.CreatedOn)
                    .Skip(@request.Pagination.Skip).Take(@request.Pagination.PageSize)
                    .Include(e => e.Images)
                    .Include(c => c.Category)
                    .Include(s => s.WallArtSize).Select(w => new PublicWallArt
                    {
                        Id = w.Id,
                        Amount = w.WallArtSize.Amount,
                        TotalAmount = w.WallArtSize.Amount,
                        Category = w.Category.Name,
                        Quantity = 1,
                        Description = w.Description,
                        Favourite = false,
                        FixedSize = w.FixedSize, // w.WallArtSize.IsDefault,
                        FrameClass =  w.FixedSize ? string.Empty : FrameClass.ELEGANT,
                        Header = w.Header,
                        Image = w.ImageAbsURL,
                        Key = string.Empty,
                        Rating = 5,
                        Size = w.WallArtSize.Id,
                        Images = w.Images.Select(i => new ImageModel
                        {
                            Id = i.Id,
                            Name = i.Name,
                            Image = i.ImageAbsURL,
                            Price = 0m,
                            Description = i.Description,
                        }).ToList()



                    }).ToListAsync()

                    :

                    await _context.WallArts
                  .Where(e => e.Description.Contains(@request.Search) || (e.Header.Contains(@request.Search)) || (e.Category.Name.Contains(@request.Search)))
                  .OrderByDescending(e => e.CreatedOn)
                  .Skip(@request.Pagination.Skip).Take(@request.Pagination.PageSize)
                  .Include(e => e.Images)
                  .Include(c => c.Category)
                  .Include(s => s.WallArtSize).Select(w => new PublicWallArt
                  {
                      Id = w.Id,
                      Amount = w.WallArtSize.Amount,
                      TotalAmount = w.WallArtSize.Amount,
                      Category = w.Category.Name,
                      Quantity = 1,
                      Description = w.Description,
                      Favourite = false,
                      FixedSize = w.FixedSize, // w.WallArtSize.IsDefault,
                      FrameClass = w.FixedSize ? string.Empty : FrameClass.ELEGANT,
                      Header = w.Header,
                      Image = w.ImageAbsURL,
                      Key = string.Empty,
                      Rating = 5,
                      Size = w.WallArtSize.Id,
                      Images = w.Images.Select(i => new ImageModel
                      {
                          Id = i.Id,
                          Name = i.Name,
                          Image = i.ImageAbsURL,
                          Price = 0m,
                          Description = i.Description,
                      }).ToList()



                     }).ToListAsync()
                };
                   
                



                

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<WallArtSetup> Setup(PaginationFilter Filter)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@Filter", Filter);

                return new WallArtSetup
                {
                    WallArts = await WallArts(@Filter),
                    Categories = await _context.WallArtCategories.OrderBy(n => n.Name).Include(w => w.WallArts).Select(c => new WallArtCategoryOption
                    {
                        Id = c.Id,
                        Name = c.Name,
                        WallArtCount = decimal.Round(c.WallArts.Count(), 0, MidpointRounding.AwayFromZero)

                    }).ToListAsync(),
                    Sizes = await _context.WallArtSizes.OrderBy(s => s.Name).Include(w => w.WallArts).Select(s => new WallArtSizeOption 
                    { 
                      Id = s.Id,
                      Name = s.Name,
                      IsDefault = s.IsDefault,
                      Amount = decimal.Round(s.Amount,2, MidpointRounding.AwayFromZero),
                      WallArtCount = decimal.Round(s.WallArts.Count(), 0, MidpointRounding.AwayFromZero)
                    }).ToListAsync()
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<WallArtCRUDResponse> Update(WallArtCreateRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.WallArts.AnyAsync(e => e.Id == @request.WallArt.WallartId))
                {

                    WallArt wallArt = await _context.WallArts.Where(e => e.Id == request.WallArt.WallartId).Include(w => w.Images).FirstOrDefaultAsync();

                    if (wallArt != null)
                    {
                        List<WallArtImage> deletedWallImages = wallArt.Images.Where(e => !request.WallArt.WallartImages.Any(w => w.WallartImageId == e.Id)).ToList();
                        List<WallArtImage> oldWallImages = wallArt.Images.Where(e => request.WallArt.WallartImages.Any(w => w.WallartImageId == e.Id)).ToList();
                        List<WallArtImageBase> newWallImages = request.WallArt.WallartImages.Where(e => !wallArt.Images.Any(a => a.Id == e.WallartImageId)).ToList();

                        if (deletedWallImages.Any())
                        {
                            foreach (var deletedImage in deletedWallImages)
                            {
                               await deletedImage.ImageURL.DeleteFileFromPathAsync();

                            }

                            _context.WallArtImages.RemoveRange(deletedWallImages);
                            await _context.SaveChangesAsync();
                        }

                        if (oldWallImages.Any())
                        {

                            foreach (var oldImage in oldWallImages)
                            {
                                WallArtImageBase old = @request.WallArt.WallartImages.Where(e => e.WallartImageId == oldImage.Id).FirstOrDefault();
                                if (old != null)
                                {
                                    FileMeta fileMeta = new() { FileName = null, ImageByte = null, Path = null };

                                    if (old.WallartImageImage.IsBase64String())
                                    {
                                        string outPath = _hostingEnvironment.WebRootPath + "\\images\\WallArt\\WallArtImages\\" + old.WallartImageName;
                                        fileMeta = await oldImage.ImageURL.RenameFile(old.WallartImageImage, outPath);
                                    }


                                   
                                  
                                        oldImage.Description = old.WallartImageDescription;
                                        oldImage.ImageURL = string.IsNullOrEmpty(fileMeta.Path) ? oldImage.ImageURL :  fileMeta.Path;
                                        oldImage.ImageAbsURL = string.IsNullOrEmpty(fileMeta.Path) ? oldImage.ImageAbsURL  : _currentUserService.WebRoot() + "/images/WallArt/WallArtImages/" + fileMeta.FileName;
                                        oldImage.ImageRelURL = string.IsNullOrEmpty(fileMeta.Path) ? oldImage.ImageRelURL : "/images/WallArt/WallArtImages/" + fileMeta.FileName;
                                        oldImage.Name = old.WallartImageName;



                                    
                                }

                            }


                            _context.WallArtImages.UpdateRange(oldWallImages);
                            await _context.SaveChangesAsync();

                        }

                        if (newWallImages.Any())
                        {
                            List<WallArtImage> wArtImages = new();
                            foreach(var newWallImage in newWallImages)
                            {
                                string wallArtImageOutputPath = _hostingEnvironment.WebRootPath + "\\images\\WallArt\\WallArtImages\\" + newWallImage.WallartImageName;
                                FileMeta wallArtImagFileMeta = await newWallImage.WallartImageImage.SaveBase64AsImage(wallArtImageOutputPath);
                                if (!string.IsNullOrEmpty(wallArtImagFileMeta.Path) && (wallArtImagFileMeta.ImageByte != null))
                                {
                                    WallArtImage wallArtImage = new()
                                    {
                                        WallArtId = _context.WallArts.Find(wallArt.Id).Id,
                                        Description = newWallImage.WallartImageDescription,
                                        ImageAbsURL = _currentUserService.WebRoot() + "/images/WallArt/WallArtImages/"+ wallArtImagFileMeta.FileName,
                                        ImageRelURL = "/images/WallArt/WallArtImages/" + wallArtImagFileMeta.FileName,
                                        Name = newWallImage.WallartImageName,
                                        ImageURL = wallArtImagFileMeta.Path
                                    };

                                    wArtImages.Add(wallArtImage);
                                }
                               
                            }


                            if (wArtImages.Any())
                            {
                                await _context.WallArtImages.AddRangeAsync(wArtImages);
                                await _context.SaveChangesAsync();
                            }

                        }




                        //update the wallart

                        decimal amount = 0m;
                        string sizeId = string.Empty;

                        if (request.WallArt.WallartFixedSize)
                        {
                            WallArtSize wallArtSize = await _context.WallArtSizes.Where(e => e.IsDefault == true).FirstOrDefaultAsync();
                            if (wallArtSize != null)
                            {
                                amount = wallArtSize.Amount;
                                sizeId = wallArtSize.Id;
                            }

                            else
                            {
                                return new WallArtCRUDResponse
                                {
                                    WallArt = await WallArts(@request.PaginationFilter),
                                    Response = new BaseResponse
                                    {
                                        Message = "No default wallart size defined. Fixed wallart size setup assignment failed",
                                        Result = RequestResult.Error,
                                        Succeeded = false,
                                        Title = "Reference Error"
                                    }
                                };
                            }

                        }

                        else
                        {
                            WallArtSize wallArtSize = await _context.WallArtSizes.FindAsync(@request.WallArt.WallartSizeSizeId);
                            if (wallArtSize != null)
                            {
                                amount = wallArtSize.Amount;
                            }

                            else
                            {
                                return new WallArtCRUDResponse
                                {
                                    WallArt = await WallArts(@request.PaginationFilter),
                                    Response = new BaseResponse
                                    {
                                        Message = "Reference to wall art size could not be resolved. Wallart setup  failed",
                                        Result = RequestResult.Error,
                                        Succeeded = false,
                                        Title = "Reference Error"
                                    }
                                };
                            }
                        }





                        FileMeta wFileMeta = new() { FileName = null, Path = null, ImageByte = null };

                        if (@request.WallArt.WallartImage.IsBase64String())
                        {
                            string outputPath = _hostingEnvironment.WebRootPath + "\\images\\WallArt\\" + @request.WallArt.WallartHeader;
                            wFileMeta = string.IsNullOrEmpty(wallArt.ImageURL) ? await @request.WallArt.WallartImage.SaveBase64AsImage(outputPath) : await wallArt.ImageURL.RenameFile(request.WallArt.WallartImage, outputPath);
                        }


                       

                       
                            wallArt.Amount = amount;
                            wallArt.CategoryId = _context.WallArtCategories.Find(request.WallArt.WallartCategoryCategoryId).Id;
                            wallArt.Header = request.WallArt.WallartHeader;
                            wallArt.WallArtSizeId = request.WallArt.WallartFixedSize ? _context.WallArtSizes.Find(sizeId).Id : _context.WallArtSizes.Find(request.WallArt.WallartSizeSizeId).Id;
                            wallArt.ImageURL = string.IsNullOrEmpty(wFileMeta.Path) ? wallArt.ImageURL : wFileMeta.Path;
                            wallArt.ImageAbsURL = string.IsNullOrEmpty(wFileMeta.Path) ? wallArt.ImageAbsURL : _currentUserService.WebRoot() + "/images/WallArt/" + wFileMeta.FileName;
                            wallArt.ImageRelURL = string.IsNullOrEmpty(wFileMeta.Path) ? wallArt.ImageRelURL : "/images/WallArt/" + wFileMeta.FileName;
                            wallArt.Description = request.WallArt.WallartDescription;
                            wallArt.FixedSize = request.WallArt.WallartFixedSize;



                            _context.WallArts.Update(wallArt);
                            int updateResult = await _context.SaveChangesAsync();

                            if (updateResult > 0)
                            {
                                return new WallArtCRUDResponse
                                {
                                    WallArt = await WallArts(@request.PaginationFilter),
                                    Response = new BaseResponse
                                    {
                                        Message = "Wall art updated.",
                                        Result = RequestResult.Success,
                                        Succeeded = true,
                                        Title = "Successful"
                                    }
                                };
                            }
                        

                        

                      







                    }

                }

                return new WallArtCRUDResponse
                {
                    WallArt = await WallArts(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = "Reference to wallart could not be resolved. This wallart may have been deleted",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Null WallArt Reference"
                    }
                };


            }
            catch (Exception ex)
            {

                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                return new WallArtCRUDResponse
                {
                    WallArt = await WallArts(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    }
                };
            }
        }

        public async Task<WallArtCRUDResponse> UpdateWallImage(WallArtImageRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                if(await _context.WallArtImages.AnyAsync(e => e.Id == request.WallArtImage.WallartImageId))
                {

                    WallArtImage wallArtImage = await _context.WallArtImages.FindAsync(@request.WallArtImage.WallartImageId);
                    if(wallArtImage != null)
                    {
                        FileMeta fileMeta = new() { FileName = null, ImageByte = null, Path = null };

                        if (@request.WallArtImage.WallartImageImage.IsBase64String())
                        {
                            string outputPath = _hostingEnvironment.WebRootPath + "\\images\\WallArt\\WallArtImages\\" + @request.WallArtImage.WallartImageName;
                            fileMeta = await wallArtImage.ImageURL.RenameFile(@request.WallArtImage.WallartImageImage, outputPath);
                        }
                      

                       


                        
                            wallArtImage.Description = request.WallArtImage.WallartImageDescription;
                            wallArtImage.ImageURL = string.IsNullOrEmpty(fileMeta.Path) ? wallArtImage.ImageURL : fileMeta.Path;
                            wallArtImage.Name = request.WallArtImage.WallartImageName;
                            wallArtImage.ImageAbsURL = string.IsNullOrEmpty(fileMeta.Path) ? wallArtImage.ImageAbsURL :  _currentUserService.WebRoot() + "/images/WallArt/WallArtImages/" + fileMeta.FileName;
                            wallArtImage.ImageRelURL = string.IsNullOrEmpty(fileMeta.Path) ? wallArtImage.ImageRelURL : "/images/WallArt/WallArtImages/" + fileMeta.FileName;

                        _context.WallArtImages.Update(wallArtImage);

                            int updateResult = await _context.SaveChangesAsync();
                            if(updateResult > 0)
                            {
                                return new WallArtCRUDResponse
                                {
                                    WallArt = await WallArts(@request.PaginationFilter),
                                    Response = new BaseResponse
                                    {
                                        Message = "Wallart image updated.",
                                        Result = RequestResult.Success,
                                        Succeeded = true,
                                        Title = "Successfull"
                                    }
                                };
                            }
                        
                    }

                }


                return new WallArtCRUDResponse
                {
                    WallArt = await WallArts(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = "Reference to wallart image could not be resolved. This wallart image may have been deleted",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Null Object Reference"
                    }
                };
            }
            catch (Exception ex)
            {

                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                return new WallArtCRUDResponse
                {
                    WallArt = await WallArts(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    }
                };
            }
        }

        public async Task<WallArtResponse> WallArts(PaginationFilter Filter)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@Filter", Filter);

                return string.IsNullOrEmpty(Filter.Category.Id) ?

                    new WallArtResponse
                    {
                        WallArts = await _context.WallArts
                          .Include(e => e.Images)
                          .Include(s => s.WallArtSize)
                          .OrderBy(e => e.Header)
                          .Include(c => c.Category)
                          .Skip(@Filter.Pagination.Skip)
                          .Take(@Filter.Pagination.PageSize)
                          .Select( w => new WallArtModel
                          { 
                              WallartAmount = string.IsNullOrEmpty(w.WallArtSizeId) ? 0m : w.WallArtSize.Amount,
                              WallartCategoryCategoryId = w.CategoryId,
                              WallartDescription = w.Description,
                              WallartHeader = w.Header,
                              WallartCategoryCategoryName = w.Category.Name,
                              WallartFixedSize = w.FixedSize,
                              WallartId = w.Id,
                              WallartImage = w.ImageAbsURL,
                              WallartImagesCount = w.Images.Count(),
                              WallartImageURL = w.ImageURL,
                              WallartRating = w.Rating,
                              WallartSizeSizeId = w.WallArtSizeId,
                              WallartSizeSizeName = w.WallArtSize.Name,
                              WallartImages = w.Images.Select( i => new WallArtImageResponse
                              {
                                  WallartImageWallArtId = w.Id,
                                  WallartImageDescription = i.Description,
                                  WallartImageId = i.Id,
                                  WallartImageImage = i.ImageAbsURL,
                                  WallartImageImageURL = i.ImageAbsURL,
                                  WallartImageName = i.Name
                              }).ToList()

                          }).ToListAsync(),
                        TotalCount = decimal.Round(await _context.WallArts.CountAsync(), 0, MidpointRounding.AwayFromZero)
                    }

                    :

                     new WallArtResponse
                     {
                         WallArts = await _context.WallArts
                          .Where( ct => ct.CategoryId == @Filter.Category.Id)
                          .Include(e => e.Images)
                          .Include(s => s.WallArtSize)
                          .OrderBy(e => e.Header)
                          .Include(c => c.Category)
                          .Skip(@Filter.Pagination.Skip)
                          .Take(@Filter.Pagination.PageSize)
                          .Select(w => new WallArtModel
                          {
                              WallartAmount = string.IsNullOrEmpty(w.WallArtSizeId) ? 0m : w.WallArtSize.Amount,
                              WallartCategoryCategoryId = w.CategoryId,
                              WallartDescription = w.Description,
                              WallartHeader = w.Header,
                              WallartCategoryCategoryName = w.Category.Name,
                              WallartFixedSize = w.FixedSize,
                              WallartId = w.Id,
                              WallartImage = w.ImageAbsURL,
                              WallartImagesCount = w.Images.Count(),
                              WallartImageURL = w.ImageAbsURL,
                              WallartRating = w.Rating,
                              WallartSizeSizeId = w.WallArtSizeId,
                              WallartSizeSizeName = w.WallArtSize.Name,
                              WallartImages = w.Images.Select(i => new WallArtImageResponse
                              {
                                  WallartImageWallArtId = w.Id,
                                  WallartImageDescription = i.Description,
                                  WallartImageId = i.Id,
                                  WallartImageImage = i.ImageAbsURL,
                                  WallartImageImageURL = i.ImageAbsURL,
                                  WallartImageName = i.Name
                              }).ToList()

                          }).ToListAsync(),
                         TotalCount = decimal.Round(await _context.WallArts.Where(ct => ct.CategoryId == @Filter.Category.Id).CountAsync(), 0, MidpointRounding.AwayFromZero)

                     };
            }
            catch (Exception)
            {

                throw;
            }
           
        }
    }
}
