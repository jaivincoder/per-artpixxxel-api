
using System;

using api.artpixxel.Data;
using api.artpixxel.service.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using api.artpixxel.data.Features.Common;
using Microsoft.AspNetCore.Hosting;
using System.Data.SqlClient;
using System.IO;
using api.artpixxel.repo.Extensions;
using api.artpixxel.data.Models;
using api.artpixxel.data.Features.KidsImageGalleries;
using api.artpixxel.data.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace api.artpixxel.repo.Features.KidsGalleries
{
    public class KidsGalleryImageService : IKidsGalleryImageService
    {
        private readonly ArtPixxelContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICurrentUserService _currentUserService;
        public KidsGalleryImageService(ArtPixxelContext context,
            ICurrentUserService currentUserService,
            IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _currentUserService = currentUserService;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<KidsImageGalleryCRUDResponse> BatchCreate(List<BaseKidseGalleryImageModel> requests)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@requests", requests);

                if (@requests.Any())
                {
                    string outputPath = _hostingEnvironment.WebRootPath + "\\images\\kidsGallery";
                    if (!(await outputPath.DirectoryExistAsync()))
                    {
                        await Task.Run(() => Directory.CreateDirectory(outputPath));
                    }

                    List<KidsGalleryImage> kidsGalleryImages = new List<KidsGalleryImage>();

                    foreach (var request in @requests)
                    {
                        string filePath = outputPath + "\\" + request.Name;
                        FileMeta fileMeta = await @request.Image.SaveBase64AsImage(filePath);
                        if (fileMeta.Path != null)
                        {
                            KidsGalleryImage galleryImage = new()
                            {
                                Name = request.Name,
                                ImageURL = fileMeta.Path,
                                ImageAbsURL = _currentUserService.WebRoot() + "/images/kidsGallery/" + fileMeta.FileName,
                                ImageRelURL = "/images/kidsGallery/" + fileMeta.FileName,
                                Description = request.Description,

                            };

                            kidsGalleryImages.Add(galleryImage);

                        }

                    }

                    if (kidsGalleryImages.Any())
                    {
                        _context.KidsGalleryImages.AddRange(kidsGalleryImages);
                        int savResult = await _context.SaveChangesAsync();

                        if (savResult > 0)
                        {
                            return new KidsImageGalleryCRUDResponse
                            {
                                Images = kidsGalleryImages.Select(e => new KidsGalleryImageModel
                                {
                                    Name = e.Name,
                                    Description = e.Description,
                                    Id = e.Id,
                                    Image = e.ImageAbsURL,
                                    Selected = false
                                }).ToList(), 
                                Response = new BaseResponse
                                {
                                    Message = $"Gallery {(kidsGalleryImages.Count == 1 ? "image": "images")} successfully created.",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }
                            };
                        }


                        else
                        {

                            foreach (KidsGalleryImage kidsGalleryImage in kidsGalleryImages)
                            {
                                await kidsGalleryImage.ImageURL.DeleteFileFromPathAsync();
                            }

                            return new KidsImageGalleryCRUDResponse
                            {
                                Images = new List<KidsGalleryImageModel>(),
                                Response = new BaseResponse
                                {
                                    Message = $"Gallery {(kidsGalleryImages.Count == 1 ? "image" : "images")} couldn't be created. Please try again later.",
                                    Result = RequestResult.Error,
                                    Succeeded = false,
                                    Title = "Unknown Error"
                                }
                            };
                        }

                      

                    }





                }

                return new KidsImageGalleryCRUDResponse
                {
                    Images = new List<KidsGalleryImageModel>(),
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


                return new KidsImageGalleryCRUDResponse
                {
                    Images = new List<KidsGalleryImageModel>(),
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

        public async Task<BaseResponse> BatchDelete(BaseId request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                List<string> ids = request.Id.Split(',').ToList();
                if (ids.Any())
                {
                    List<KidsGalleryImage> kidsGalleryImages = await _context.KidsGalleryImages.Where(e => ids.Contains(e.Id)).ToListAsync();
                    if (kidsGalleryImages.Any())
                    {
                        foreach (KidsGalleryImage kidsGalleryImage in kidsGalleryImages)
                        {
                            await kidsGalleryImage.ImageURL.DeleteFileFromPathAsync();
                        }

                        _context.KidsGalleryImages.RemoveRange(kidsGalleryImages);
                        int rs = await _context.SaveChangesAsync();

                        if(rs > 0)
                        {
                            return new BaseResponse
                            {
                                Message = $"Kids gallery image{(kidsGalleryImages.Count == 1 ? "" : "s")} deleted successfully.",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            };
                        }


                        return new BaseResponse
                        {
                            Message = "Kids gallery images deletion did not succeed. Please try again later.",
                            Result = RequestResult.Error,
                            Succeeded = false,
                            Title = "Unknown Error"
                        };

                    }
                }

                return new BaseResponse
                {
                    Message = "Request misformatted and not understood",
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = "INvalid Request"

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

        public async Task<PublicKidsCharactersResponse> Characters()
        {
            try
            {
                if(await _context.KidsGalleryImages.AnyAsync())
                {
                    List<PublicKidsGalleryImage> characters = await _context.KidsGalleryImages.Select( e => new PublicKidsGalleryImage
                    {
                        Image = e.ImageAbsURL,
                        Id = e.Id,
                        Name = e.Name,
                        ImageString = e.ImageURL
                        

                    }).ToListAsync();

                    if (characters.Any())
                    {
                        foreach (PublicKidsGalleryImage character in characters)
                        {
                            character.ImageString = await character.ImageString.GetImageFileBase64();
                        }


                        return new PublicKidsCharactersResponse
                        {
                            Characters = characters,
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

                return new PublicKidsCharactersResponse
                {
                    Characters = new List<PublicKidsGalleryImage>(),
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

                return new PublicKidsCharactersResponse
                {
                    Characters = new List<PublicKidsGalleryImage>(),
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

        public async Task<KidsImageGalleryCRUDResponse> Create(BaseKidseGalleryImageModel request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                string outputPath = _hostingEnvironment.WebRootPath + "\\images\\kidsGallery";
                if(! (await outputPath.DirectoryExistAsync()))
                {
                    await Task.Run(() => Directory.CreateDirectory(outputPath));
                }

                outputPath += "\\" + @request.Name;
                FileMeta fileMeta = await @request.Image.SaveBase64AsImage(outputPath);
                if(fileMeta.Path != null)
                {
                    KidsGalleryImage galleryImage = new()
                    {
                        Name = @request.Name,
                        ImageURL = fileMeta.Path,
                        ImageAbsURL = _currentUserService.WebRoot() + "/images/kidsGallery/" + fileMeta.FileName,
                        ImageRelURL = "/images/kidsGallery/" + fileMeta.FileName,
                        Description = @request.Description,

                    };


                    _context.KidsGalleryImages.Add(galleryImage);
                    int saveResult = await _context.SaveChangesAsync();

                    if(saveResult > 0)
                    {
                        return new KidsImageGalleryCRUDResponse
                        {
                            Images = new List<KidsGalleryImageModel>
                            {
                                new KidsGalleryImageModel
                                {
                                    Id = galleryImage.Id,
                                    Description = galleryImage.Description,
                                    Image = galleryImage.ImageAbsURL,
                                    Name = galleryImage.Name,
                                    Selected = false,
                                }
                            },

                            Response = new BaseResponse
                            {
                                Message = "Gallery image successfully created.",
                                Result = RequestResult.Success,
                                Succeeded  = true,
                                Title = "Successful"
                            }
                        };
                    }

                    else
                    {
                        await galleryImage.ImageURL.DeleteFileFromPathAsync();
                    }

                }


                return new KidsImageGalleryCRUDResponse
                {
                    Images = new List<KidsGalleryImageModel>(),
                    Response = new BaseResponse
                    {
                        Message = "Gallery image coudldn't be saved. Please try again later.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "An error occurred"
                    }
                };
                




            }
            catch (Exception ex)
            {

                return new KidsImageGalleryCRUDResponse
                {
                    Images = new List<KidsGalleryImageModel>(),
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title= ex.Source
                    }
                };
            }
        }

        public async Task<BaseResponse> Delete(BaseId request)
        {
            SqlParameter[] myparm = new SqlParameter[1];
            myparm[0] = new SqlParameter("@request", request);
            try
            {
                if (await _context.KidsGalleryImages.AnyAsync(e => e.Id == @request.Id))
                {
                    KidsGalleryImage kidsGalleryImage = await _context.KidsGalleryImages.FindAsync(@request.Id);
                    if (kidsGalleryImage != null)
                    {
                        await kidsGalleryImage.ImageURL.DeleteFileFromPathAsync();

                        _context.KidsGalleryImages.Remove(kidsGalleryImage);
                        int deleteResult = await _context.SaveChangesAsync();

                        if (deleteResult > 0)
                        {
                            return new BaseResponse
                            {
                                Message = "Kids gallery image deleted successfully.",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            };
                        };

                    }



                    return new BaseResponse
                    {
                        Message = "Kids gallery image deletion did not succeed. Please try again later.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Unknown Error"
                    };


                }


                return new BaseResponse
                {
                    Message = "Kids gallery image not found. This gallery image may have been deleted.",
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
        

        public async Task<KidsImageGalleryCRUDResponse> Images()
        {
            try
            {
                if(await _context.KidsGalleryImages.AnyAsync())
                {
                    List<KidsGalleryImageModel> images = await _context.KidsGalleryImages.Select(x => new KidsGalleryImageModel 
                    { 
                        Name = x.Name,
                        Description = x.Description,
                        Id = x.Id,
                        Image = x.ImageAbsURL,
                        Selected = false
                    
                    })
                    .OrderBy(e => e.Name)
                    .ToListAsync();

                    if (images.Any())
                    {
                        return new KidsImageGalleryCRUDResponse
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

                return new KidsImageGalleryCRUDResponse
                {
                    Images = new List<KidsGalleryImageModel>(),
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

                return new KidsImageGalleryCRUDResponse
                {
                    Images = new List<KidsGalleryImageModel>(),
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

        public async Task<KidsImageGalleryCRUDResponse> Update(BaseKidseGalleryImageModel request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.KidsGalleryImages.AnyAsync(e => e.Id == @request.Id))
                {
                    KidsGalleryImage kidsGalleryImage = await _context.KidsGalleryImages.FindAsync(@request.Id);
                    if(kidsGalleryImage != null) 
                    {

                        FileMeta fileMeta = new() { FileName = null, Path = null, ImageByte = null };

                        if (request.Image.IsBase64String())
                        {
                            string outputPath = _hostingEnvironment.WebRootPath + "\\images\\kidsGallery\\" + @request.Name;
                            fileMeta = await kidsGalleryImage.ImageURL.RenameFile(request.Image, outputPath);
                        }

                        kidsGalleryImage.Name = request.Name;
                        kidsGalleryImage.Description = request.Description;
                        kidsGalleryImage.ImageAbsURL = string.IsNullOrEmpty(fileMeta.Path) ? kidsGalleryImage.ImageAbsURL : _currentUserService.WebRoot() + "/images/kidsGallery/" + fileMeta.FileName;
                        kidsGalleryImage.ImageRelURL = string.IsNullOrEmpty(fileMeta.Path) ? kidsGalleryImage.ImageRelURL : "/images/kidsGallery/" + fileMeta.FileName;
                        kidsGalleryImage.ImageURL = string.IsNullOrEmpty(fileMeta.Path) ? kidsGalleryImage.ImageURL : fileMeta.Path;


                        _context.KidsGalleryImages.Update(kidsGalleryImage);
                        int upResult = await _context.SaveChangesAsync();

                        if(upResult > 0)
                        {
                            return new KidsImageGalleryCRUDResponse
                            {
                                Images = new List<KidsGalleryImageModel>
                            {
                                new KidsGalleryImageModel
                                {
                                    Id = kidsGalleryImage.Id,
                                    Description = kidsGalleryImage.Description,
                                    Image = kidsGalleryImage.ImageAbsURL,
                                    Name = kidsGalleryImage.Name,
                                    Selected = false
                                }
                            },

                                Response = new BaseResponse
                                {
                                    Message = "Gallery image successfully updated.",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }
                            };
                        }


                        return new KidsImageGalleryCRUDResponse
                        {
                            Images = new List<KidsGalleryImageModel>(),
                            Response = new BaseResponse
                            {
                                Message = "Kids gallery update failed. Please try again later.",
                                Result = RequestResult.Error,
                                Succeeded = false,
                                Title = "Unknown Eror"
                            }
                        };




                    }
                }

                return new KidsImageGalleryCRUDResponse
                {
                    Images = new List<KidsGalleryImageModel>(),
                    Response = new BaseResponse
                    {
                        Message = "Kids gallery image not found. This gallery image may have been deleted.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Image Not Found"
                    }
                };
            }
            catch (Exception ex)
            {

                return new KidsImageGalleryCRUDResponse
                {
                    Images = new List<KidsGalleryImageModel>(),
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
