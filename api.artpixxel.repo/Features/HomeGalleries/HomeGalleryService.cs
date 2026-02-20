
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.HomeGalleries;
using api.artpixxel.data.Models;
using api.artpixxel.data.Services;
using api.artpixxel.Data;
using api.artpixxel.repo.Extensions;
using api.artpixxel.service.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace api.artpixxel.repo.Features.HomeGalleries
{
    public class HomeGalleryService : IHomeGalleryService
    {
        private readonly ArtPixxelContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICurrentUserService _currentUserService;
        public HomeGalleryService(ArtPixxelContext context, IWebHostEnvironment hostingEnvironment, ICurrentUserService currentUserService)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _currentUserService = currentUserService;

        }
        public async Task<HomeGalleryCRUDResponse> BatchCreate(List<BaseHomeGalleryImageModel> requests)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@requests", requests);


                if (requests.Any())
                {

                    string outputPath = _hostingEnvironment.WebRootPath + "\\images\\homeGallery";
                    if (!(await outputPath.DirectoryExistAsync()))
                    {
                        await Task.Run(() => Directory.CreateDirectory(outputPath));
                    }

                    List<HomeGallery> galleries = new List<HomeGallery>();

                    foreach (BaseHomeGalleryImageModel request in requests)
                    {
                        if (Enum.TryParse(request.Type, out HomeGalleryImageType type))
                        {
                            string filePath = outputPath + "\\" + request.Name;
                            FileMeta fileMeta = await @request.Image.SaveBase64AsImage(filePath);

                            if (fileMeta.Path != null)
                            {
                                HomeGallery galleryImage = new()
                                {
                                    Name = request.Name,
                                    Active = request.Active,
                                    Type = type,
                                    ImageURL = fileMeta.Path,
                                    ImageAbsURL = _currentUserService.WebRoot() + "/images/homeGallery/" + fileMeta.FileName,
                                    ImageRelURL = "/images/homeGallery/" + fileMeta.FileName,
                                    Description = request.Description,

                                };

                                galleries.Add(galleryImage);

                            }
                        }
                    }






                    if (galleries.Any())
                    {
                        _context.HomeGalleries.AddRange(galleries);
                        int savResult = await _context.SaveChangesAsync();

                        if (savResult > 0)
                        {
                            return new HomeGalleryCRUDResponse
                            {
                                Images = galleries.Select(e => new HomeGalleryImageModel
                                {
                                    Name = e.Name,
                                    Description = e.Description,
                                    Type = e.Type.ToString(),
                                    Active = e.Active,
                                    Id = e.Id,
                                    Image = e.ImageAbsURL,
                                    Selected = false
                                }).ToList(),
                                Response = new BaseResponse
                                {
                                    Message = $"Gallery {(galleries.Count == 1 ? "image" : "images")} successfully created.",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }
                            };
                        }


                        else
                        {

                            foreach (HomeGallery homeGalleryImage in galleries)
                            {
                                await homeGalleryImage.ImageURL.DeleteFileFromPathAsync();
                            }

                            return new HomeGalleryCRUDResponse
                            {
                                Images = new List<HomeGalleryImageModel>(),
                                Response = new BaseResponse
                                {
                                    Message = $"Gallery {(galleries.Count == 1 ? "image" : "images")} could not be created created. Please try again later",
                                    Result = RequestResult.Error,
                                    Succeeded = false,
                                    Title = "Unknown Error"
                                }
                            };
                        }



                    }




                }



                return new HomeGalleryCRUDResponse
                {
                    Images = new List<HomeGalleryImageModel>(),
                    Response = new BaseResponse
                    {
                        Message = "Couldn\'t create gallery images from an empty list.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Empty Request"
                    }
                };

             



            }
            catch (Exception ex)
            {

                return new HomeGalleryCRUDResponse
                {
                    Images = new List<HomeGalleryImageModel>(),
                    Response= new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    }
                };
            }
        }

        public async Task<BaseResponse> BatchDelete(BaseId request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);


                if (!string.IsNullOrWhiteSpace(request.Id))
                {
                    List<string> ids = request.Id.Split(',').ToList();

                    if (ids.Any())
                    {
                        List<HomeGallery> homeGalleryImages = await _context.HomeGalleries.Where(e => ids.Contains(e.Id)).ToListAsync();
                        if (homeGalleryImages.Any())
                        {
                            foreach (HomeGallery homeGalleryImage in homeGalleryImages)
                            {
                                await homeGalleryImage.ImageURL.DeleteFileFromPathAsync();
                            }


                            _context.HomeGalleries.RemoveRange(homeGalleryImages);
                            int rs = await _context.SaveChangesAsync();


                            if (rs > 0)
                            {
                                return new BaseResponse
                                {
                                    Message = $"Home gallery image{(homeGalleryImages.Count == 1 ? "" : "s")} deleted successfully.",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                };
                            }


                            return new BaseResponse
                            {
                                Message = "Home gallery images deletion did not succeed. Please try again later.",
                                Result = RequestResult.Error,
                                Succeeded = false,
                                Title = "Unknown Error"
                            };


                        }

                    }

                    return new BaseResponse
                    {
                        Message = $"Gallery {(ids.Count == 1 ? "image" : "images")} not found.  {(ids.Count == 1 ? "This gallery image" : "These gallery images")} may have been deleted.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Empty Result"

                    };
                }

                


                return new BaseResponse
                {
                    Message = "Request misformatted and not understood",
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = "Invalid Request"

                };

            }
            catch (Exception ex)
            {

                return new BaseResponse
                {
                    Message = ex.Message,
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = ex.Source
                };
            }
        }

        public async Task<HomeGalleryCRUDResponse> Create(BaseHomeGalleryImageModel request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (Enum.TryParse(request.Type, out HomeGalleryImageType type))
                {
                    string outputPath = _hostingEnvironment.WebRootPath + "\\images\\homeGallery";
                    if (!(await outputPath.DirectoryExistAsync()))
                    {
                        await Task.Run(() => Directory.CreateDirectory(outputPath));
                    }

                    string filePath = outputPath + "\\" + request.Name;
                    FileMeta fileMeta = await @request.Image.SaveBase64AsImage(filePath);


                    if (fileMeta.Path != null)
                    {
                        HomeGallery galleryImage = new()
                        {
                            Name = request.Name,
                            Active = request.Active,
                            Type = type,
                            ImageURL = fileMeta.Path,
                            ImageAbsURL = _currentUserService.WebRoot() + "/images/homeGallery/" + fileMeta.FileName,
                            ImageRelURL = "/images/homeGallery/" + fileMeta.FileName,
                            Description = request.Description,

                        };



                        _context.HomeGalleries.Add(galleryImage);
                        int svResult = await _context.SaveChangesAsync();

                        if (svResult > 0)
                        {
                            return new HomeGalleryCRUDResponse
                            {
                                Images = new List<HomeGalleryImageModel>
                            {
                                new HomeGalleryImageModel
                                {
                                    Id = galleryImage.Id,
                                    Description = galleryImage.Description,
                                    Image = galleryImage.ImageAbsURL,
                                    Name = galleryImage.Name,
                                    Active = galleryImage.Active, 
                                    Type = galleryImage.Type.ToString(),
                                    Selected = false,
                                }
                            },

                                Response = new BaseResponse
                                {
                                    Message = "Gallery image successfully created.",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }
                            };
                        }

                        else
                        {
                            await galleryImage.ImageURL.DeleteFileFromPathAsync();

                            return new HomeGalleryCRUDResponse
                            {
                                Images = new List<HomeGalleryImageModel>(),
                                Response = new BaseResponse
                                {
                                    Message = "Gallery image creation failed. Please try again later.",
                                    Result = RequestResult.Error,
                                    Succeeded = false,
                                    Title = "Error"
                                }
                            };



                        }




                    }






                }

                return new HomeGalleryCRUDResponse
                {
                    Images = new List<HomeGalleryImageModel>(),
                    Response = new BaseResponse
                    {
                        Message = "Type of home gallery not understood.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Invalid Type"
                    }
                };


            }
            catch (Exception ex)
            {

                return new HomeGalleryCRUDResponse
                {
                    Images = new List<HomeGalleryImageModel>(),
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

        public async Task<BaseResponse> Delete(BaseId request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.HomeGalleries.AnyAsync(e => e.Id == @request.Id))
                {
                    HomeGallery homeGalleryImage = await _context.HomeGalleries.FindAsync(@request.Id);
                    if (homeGalleryImage != null)
                    {
                        await homeGalleryImage.ImageURL.DeleteFileFromPathAsync();

                        _context.HomeGalleries.Remove(homeGalleryImage);
                        int deleteResult = await _context.SaveChangesAsync();

                        if (deleteResult > 0)
                        {
                            return new BaseResponse
                            {
                                Message = "Home gallery image deleted successfully.",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            };
                        };

                    }



                    return new BaseResponse
                    {
                        Message = "Home gallery image deletion did not succeed. Please try again later.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Unknown Error"
                    };


                }


                return new BaseResponse
                {
                    Message = "Home gallery image not found. This gallery image may have been deleted.",
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = "Image Not Found"
                };



            }
            catch (Exception ex)
            {

                return new BaseResponse
                {
                    Message = ex.Message,
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = ex.Source
                };
            }
        }

        public async Task<HomeGalleryCRUDResponse> Images()
        {
            try
            {
                if(await _context.HomeGalleries.AnyAsync())
                {
                    List<HomeGalleryImageModel> images = await _context.HomeGalleries.Select(x => new HomeGalleryImageModel
                    {
                        Name = x.Name,
                        Description = x.Description,
                        Id = x.Id,
                        Active = x.Active,
                        Type  = x.Type.ToString(),
                        Image = x.ImageAbsURL,
                        Selected = false

                    })
                   .OrderBy(e => e.Name)
                   .ToListAsync();

                    if (images.Any())
                    {
                        return new HomeGalleryCRUDResponse
                        {
                            Images = images,
                            Response = new BaseResponse
                            {
                                Message = string.Empty,
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = string.Empty
                            }
                        };
                    }

                }

                return new HomeGalleryCRUDResponse
                {
                    Images = new List<HomeGalleryImageModel>(),
                    Response = new BaseResponse
                    {
                        Message = string.Empty,
                        Result = RequestResult.Success,
                        Succeeded = true,
                        Title = string.Empty
                    }

                };
            }
            catch (Exception ex)
            {
                return new HomeGalleryCRUDResponse
                {
                    Images = new List<HomeGalleryImageModel>(),
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

        public async Task<HomeGalleryCRUDResponse> Update(BaseHomeGalleryImageModel request)
        {
            try
            {

                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);


                if (Enum.TryParse(@request.Type, out HomeGalleryImageType type))
                {

                    if (await _context.HomeGalleries.AnyAsync(e => e.Id == request.Id))
                    {

                        HomeGallery homeGalleryImage = await _context.HomeGalleries.FindAsync(@request.Id);
                        if (homeGalleryImage != null)
                        {

                            FileMeta fileMeta = new() { FileName = null, Path = null, ImageByte = null };

                            if (request.Image.IsBase64String())
                            {

                                string outputPath = _hostingEnvironment.WebRootPath + "\\images\\homeGallery\\" + @request.Name;
                                fileMeta = await homeGalleryImage.ImageURL.RenameFile(request.Image, outputPath);
                            }

                            homeGalleryImage.Name = request.Name;
                            homeGalleryImage.Description = request.Description;
                            homeGalleryImage.ImageAbsURL = string.IsNullOrEmpty(fileMeta.Path) ? homeGalleryImage.ImageAbsURL : _currentUserService.WebRoot() + "/images/homeGallery/" + fileMeta.FileName;
                            homeGalleryImage.ImageRelURL = string.IsNullOrEmpty(fileMeta.Path) ? homeGalleryImage.ImageRelURL : "/images/homeGallery/" + fileMeta.FileName;
                            homeGalleryImage.ImageURL = string.IsNullOrEmpty(fileMeta.Path) ? homeGalleryImage.ImageURL : fileMeta.Path;
                            homeGalleryImage.Active = request.Active;
                            homeGalleryImage.Type = type;


                            _context.HomeGalleries.Update(homeGalleryImage);
                            int upResult = await _context.SaveChangesAsync();

                            if (upResult > 0)
                            {
                                return new HomeGalleryCRUDResponse
                                {
                                    Images = new List<HomeGalleryImageModel>()
                            {
                                new HomeGalleryImageModel
                                {
                                    Id = homeGalleryImage.Id,
                                    Description = homeGalleryImage.Description,
                                    Image = homeGalleryImage.ImageAbsURL,
                                    Name = homeGalleryImage.Name,
                                    Active = homeGalleryImage.Active,
                                    Type = homeGalleryImage.Type.ToString(),
                                    Selected = false
                                }
                            },

                                    Response = new BaseResponse
                                    {
                                        Message = "Home gallery image successfully updated.",
                                        Result = RequestResult.Success,
                                        Succeeded = true,
                                        Title = "Successful"
                                    }
                                };
                            }


                            return new HomeGalleryCRUDResponse
                            {
                                Images = new List<HomeGalleryImageModel>(),
                                Response = new BaseResponse
                                {
                                    Message = "Home gallery update failed. Please try again later.",
                                    Result = RequestResult.Error,
                                    Succeeded = false,
                                    Title = "Unknown Eror"
                                }
                            };




                        }

                    }


                    return new HomeGalleryCRUDResponse
                    {
                        Images = new List<HomeGalleryImageModel>(),
                        Response = new BaseResponse
                        {
                            Message = "Home gallery image not found. This gallery image may have been deleted.",
                            Result = RequestResult.Error,
                            Succeeded = false,
                            Title = "Image Not Found"
                        }
                    };


                }


                return new HomeGalleryCRUDResponse
                {
                    Images = new List<HomeGalleryImageModel>(),
                    Response = new BaseResponse
                    {
                        Message = "Type of home gallery not understood.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Invalid Type"
                    }
                };




            }
            catch (Exception ex)
            {

                return new HomeGalleryCRUDResponse
                {
                    Images = new List<HomeGalleryImageModel>(),
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
    }
}
