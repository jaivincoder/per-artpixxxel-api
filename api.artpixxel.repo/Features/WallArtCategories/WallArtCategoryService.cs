

using api.artpixxel.data.Features.WallartCategories;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.artpixxel.repo.Extensions;
using api.artpixxel.data.Features.Common;
using Microsoft.Data.SqlClient;
using api.artpixxel.data.Models;
using Microsoft.AspNetCore.Hosting;
using api.artpixxel.data.Services;

namespace api.artpixxel.repo.Features.WallArtCategories
{
    public class WallArtCategoryService : IWallArtCategoryService
    {
        private readonly ArtPixxelContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICurrentUserService _currentUserService;
        public WallArtCategoryService(ArtPixxelContext context, IWebHostEnvironment hostingEnvironment, ICurrentUserService currentUserService)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _currentUserService = currentUserService;
        }

        public async Task<WallArtCategoryCRUDResponse> BatchCreate(List<WallArtCategoryBase> requests)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", requests);
                if (@requests.Any())
                {
                   

                        List<WallArtCategory> wallArtCategories = new();

                        foreach(WallArtCategoryBase request in @requests)
                        {


                        if(!await _context.WallArtCategories.AnyAsync(e => e.Name == request.WallartCategoryName))
                        {
                            FileMeta fileMeta = new() { FileName = null, Path = null, ImageByte = null};

                            if (!string.IsNullOrEmpty(request.WallartCategoryImage))
                            {
                                string outputPath = _hostingEnvironment.WebRootPath + "\\images\\WallArtCategory\\" + request.WallartCategoryName;
                                fileMeta = await request.WallartCategoryImage.SaveBase64AsImage(outputPath);
                                
                            }

                           

                           
                                WallArtCategory wallArtCategory = new()
                                {
                                    Name = request.WallartCategoryName,
                                    Description = request.WallartCategoryDescription,
                                    ImageURL =  string.IsNullOrEmpty(fileMeta.Path) ? null : fileMeta.Path,
                                    ImageAbsURL = string.IsNullOrEmpty(fileMeta.Path) ? null :  _currentUserService.WebRoot() + "/images/WallArtCategory/" + fileMeta.FileName,
                                    ImageRelURL = string.IsNullOrEmpty(fileMeta.Path) ? null : "/images/WallArtCategory/" + fileMeta.FileName
                                };

                                wallArtCategories.Add(wallArtCategory);
                            

                          
                        }
                           
                        }

                        if (wallArtCategories.Any())
                        {
                            _context.WallArtCategories.AddRange(wallArtCategories);
                            int saveResult = await _context.SaveChangesAsync();

                            if(saveResult > 0)
                            {
                                return new WallArtCategoryCRUDResponse
                                {
                                    WallArtCategories = await WallArtCategories(),
                                    Response = new BaseResponse
                                    {
                                        Message = wallArtCategories.Count == 1 ? "Wallart category created" : "Wallart categories created",
                                        Result = RequestResult.Success,
                                        Succeeded = true,
                                        Title = "Successful"
                                    }
                                };
                            }
                        }


                    



                   
                }

                return new WallArtCategoryCRUDResponse
                {
                    WallArtCategories = await WallArtCategories(),
                    Response = new BaseResponse
                    {
                        Message = "Couldn't create wall categories from empty list",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Empty Wallart Category List"
                    }
                };



            }
            catch (Exception ex)
            {
                return new WallArtCategoryCRUDResponse
                {
                    WallArtCategories = await WallArtCategories(),
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

        public async Task<WallArtCategoryCRUDResponse> Create(WallArtCategoryBase request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (!await _context.WallArtCategories.AnyAsync(e => e.Name == @request.WallartCategoryName))
                {



                    FileMeta fileMeta = new() { FileName = null, Path = null, ImageByte = null };

                    if (!string.IsNullOrEmpty(@request.WallartCategoryImage))
                    {
                        string outputPath = _hostingEnvironment.WebRootPath + "\\images\\WallArtCategory\\" + @request.WallartCategoryName;
                        fileMeta = await request.WallartCategoryImage.SaveBase64AsImage(outputPath);
                       
                    }

                 

                     WallArtCategory wallArtCategory = new()
                    {
                        Name = request.WallartCategoryName,
                        Description = request.WallartCategoryDescription,
                        ImageURL = string.IsNullOrEmpty(fileMeta.Path) ? null : fileMeta.Path,
                        ImageAbsURL = string.IsNullOrEmpty(fileMeta.Path) ? null : _currentUserService.WebRoot() + "/images/WallArtCategory/" + fileMeta.FileName,
                        ImageRelURL = string.IsNullOrEmpty(fileMeta.Path) ? null : "/images/WallArtCategory/" + fileMeta.FileName,
                     };

                     _context.WallArtCategories.Add(wallArtCategory);
                     int saveResult = await _context.SaveChangesAsync();

                    if(saveResult > 0)
                    {
                        return new WallArtCategoryCRUDResponse
                        {
                            WallArtCategories = await WallArtCategories(),
                            Response = new BaseResponse
                            {
                                Message = string.Format("Wallart category '{0}' created", request.WallartCategoryName),
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            }
                        };
                    }


                }

                return new WallArtCategoryCRUDResponse
                {
                    WallArtCategories = await WallArtCategories(),
                    Response = new BaseResponse
                    {
                        Message = string.Format("WallArt category '{0}' already exists", @request.WallartCategoryName),
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Duplicate Entry"
                    }
                };
            }
            catch (Exception ex)
            {

                return new WallArtCategoryCRUDResponse
                {
                    WallArtCategories = await WallArtCategories(),
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

        public async Task<WallArtCategoryCRUDResponse> Delete(BaseId request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if(await _context.WallArtCategories.AnyAsync(e => e.Id == @request.Id))
                {
                    WallArtCategory wallArtCategory = await _context.WallArtCategories.FindAsync(@request.Id);
                    if(wallArtCategory != null)
                    {


                        if(!await _context.WallArts.AnyAsync(e => e.CategoryId == wallArtCategory.Id))
                        {
                            if (!string.IsNullOrEmpty(wallArtCategory.ImageURL))
                            {
                                await wallArtCategory.ImageURL.DeleteFileFromPathAsync();
                            }

                            _context.WallArtCategories.Remove(wallArtCategory);
                            int deleteResult = await _context.SaveChangesAsync();
                            if (deleteResult > 0)
                            {
                                return new WallArtCategoryCRUDResponse
                                {
                                    WallArtCategories = await WallArtCategories(),
                                    Response = new BaseResponse
                                    {
                                        Message = "Wallart category removed.",
                                        Result = RequestResult.Success,
                                        Succeeded = true,
                                        Title = "Successful"
                                    }
                                };
                            }

                        }

                        return new WallArtCategoryCRUDResponse
                        {
                            WallArtCategories = await WallArtCategories(),
                            Response = new BaseResponse
                            {
                                Message = string.Format("Category '{0}' has wallarts and could not removed.", wallArtCategory.Name),
                                Result = RequestResult.Error,
                                Succeeded = false,
                                Title = "Error"
                            }
                        };

                    }
                }

                return new WallArtCategoryCRUDResponse
                {
                    WallArtCategories = await WallArtCategories(),
                    Response = new BaseResponse
                    {
                        Message = "Reference to wall art category could not be resolved. This category may have been deleted",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Null Wallart Category Reference"
                    }
                };



            }
            catch (Exception ex)
            {

                return new WallArtCategoryCRUDResponse
                {
                    WallArtCategories = await WallArtCategories(),
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

        public async Task<BaseBoolResponse> Duplicate(BaseOption request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.WallArtCategories.AnyAsync(e => e.Name == @request.Name && e.Id != @request.Id))
                {
                    return new BaseBoolResponse
                    {
                        Exist = true,
                        Message = "WallArt category: <i>" + @request.Name + " </i> already exists "
                    };
                }


                return new BaseBoolResponse
                {
                    Exist = false,
                    Message = string.Empty
                };
            }
            catch (Exception ex)
            {

                return new BaseBoolResponse
                {
                    Exist = true,
                    Message = ex.Message
                };
            }
        }

        public async Task<BaseBoolResponse> Exists(BaseStringRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.WallArtCategories.AnyAsync(e => e.Name == @request.Name))
                {
                    return new BaseBoolResponse
                    {
                        Exist = true,
                        Message = "WallArt category: <i>" + @request.Name + " </i> already exists"
                    };
                }


                return new BaseBoolResponse
                {
                    Exist = false,
                    Message = string.Empty
                };
            }
            catch (Exception ex)
            {

                return new BaseBoolResponse
                {
                    Exist = true,
                    Message = ex.Message
                };
            }
        }

        public async Task<WallArtCategoryCRUDResponse> Update(WallArtCategoryBase request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.WallArtCategories.AnyAsync(e  => e.Id == @request.WallartCategoryId))
                {
                    WallArtCategory wallArtCategory = await _context.WallArtCategories.FindAsync(@request.WallartCategoryId);

                    if(wallArtCategory != null)
                    {
                        FileMeta fileMeta = new() { FileName = null, Path = null, ImageByte = null };
                        string oldName = wallArtCategory.Name;

                        if (!string.IsNullOrEmpty(@request.WallartCategoryImage))
                        {
                            if (string.IsNullOrEmpty(wallArtCategory.ImageURL))
                            {
                                if (@request.WallartCategoryImage.IsBase64String())
                                {
                                    string outputPath = _hostingEnvironment.WebRootPath + "\\images\\WallArtCategory\\" + @request.WallartCategoryName;
                                    fileMeta = await request.WallartCategoryImage.SaveBase64AsImage(outputPath);
                                }
                                  
                                
                            }

                            else
                            {
                                if (@request.WallartCategoryImage.IsBase64String())
                                {
                                    string outputPath = _hostingEnvironment.WebRootPath + "\\images\\WallArtCategory\\" + @request.WallartCategoryName;
                                    fileMeta = await wallArtCategory.ImageURL.RenameFile(@request.WallartCategoryImage, outputPath);
                                }
                               
                                
                            }
                          
                        }


                        else
                        {
                            if (!string.IsNullOrEmpty(wallArtCategory.ImageURL))
                            {
                                await wallArtCategory.ImageURL.DeleteFileFromPathAsync();
                                wallArtCategory.ImageURL = null;
                                wallArtCategory.ImageAbsURL = null;
                                wallArtCategory.ImageRelURL = null;
                            }

                        }



                        wallArtCategory.Description = @request.WallartCategoryDescription;
                        wallArtCategory.Name = request.WallartCategoryName;
                        wallArtCategory.ImageURL = string.IsNullOrEmpty(fileMeta.Path) ? wallArtCategory.ImageURL : fileMeta.Path;
                        wallArtCategory.ImageAbsURL = string.IsNullOrEmpty(fileMeta.Path) ? wallArtCategory.ImageAbsURL : _currentUserService.WebRoot() + "/images/WallArtCategory/" + fileMeta.FileName;
                        wallArtCategory.ImageRelURL = string.IsNullOrEmpty(fileMeta.Path) ? wallArtCategory.ImageRelURL : "/images/WallArtCategory/" + fileMeta.FileName;



                        _context.WallArtCategories.Update(wallArtCategory);
                        int saveResult = await _context.SaveChangesAsync();

                        if(saveResult > 0)
                        {
                            return new WallArtCategoryCRUDResponse
                            {
                                WallArtCategories = await WallArtCategories(),
                                Response = new BaseResponse
                                {
                                    Message = string.Format("Wallart category '{0}' updated", oldName),
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }
                            };
                        }
                       
                    }
                }

                return new WallArtCategoryCRUDResponse
                {
                    WallArtCategories = await WallArtCategories(),
                    Response = new BaseResponse
                    {
                        Message = "Refefrenced wallart category could not be found. This category may have been deleted.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Null WallArt Category Reference"
                    }
                };


            }
            catch (Exception ex)
            {

                return new WallArtCategoryCRUDResponse
                {
                    WallArtCategories = await WallArtCategories(),
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

        public async Task<List<WallArtCategoryResponse>> WallArtCategories()
        {
            try
            {
                return await _context.WallArtCategories.OrderBy(cn => cn.Name).Select(e => new WallArtCategoryResponse
                {
                    WallartCategoryId = e.Id,
                    WallartCategoryName = e.Name,
                    WallartCategoryImage = string.IsNullOrEmpty(e.ImageURL) ? AssetDefault.DefaultImage :  e.ImageAbsURL,
                    WallartCategoryDescription = e.Description,
                    WallartCategoryWallartCount = decimal.Round(e.WallArts.Count(), 0, MidpointRounding.AwayFromZero)

                }).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
