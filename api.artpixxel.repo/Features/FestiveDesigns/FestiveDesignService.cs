using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.FestiveDesigns;
using api.artpixxel.data.Models;
using api.artpixxel.data.Models.Base;
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

namespace api.artpixxel.repo.Features.FestiveDesigns
{
    public class FestiveDesignService : IFestiveDesignService
    {
        private readonly ArtPixxelContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICurrentUserService _currentUserService;

        private const string folderName = "Images";
        private const string subFolderName = "festiveDesigns";


        public FestiveDesignService(ArtPixxelContext context, IWebHostEnvironment hostingEnvironment, ICurrentUserService currentUserService)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _currentUserService = currentUserService;
        }


        public async Task<FestiveDesignCRUDResponse> BatchCreate(List<BaseFestiveDesignModel> requests)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@requests", requests);


                if (requests.Any())
                {

                    string outputPath = $"{_hostingEnvironment.WebRootPath}\\{folderName}\\{subFolderName}";
                    if (!(await outputPath.DirectoryExistAsync()))
                    {
                        await Task.Run(() => Directory.CreateDirectory(outputPath));
                    }

                    List<FestiveDesign> designs = new List<FestiveDesign>();

                    foreach (BaseFestiveDesignModel request in requests)
                    {
                        if (Enum.TryParse(request.Category, out FestiveDesignCategory category))
                        {
                            string filePath = outputPath + "\\" + request.Name;
                            FileMeta fileMeta = await @request.Image.SaveBase64AsImage(filePath);

                            if (fileMeta.Path != null)
                            {
                                FestiveDesign design = new()
                                {
                                    Name = request.Name,
                                    Active = request.Active,
                                    Category = category,
                                    ImageURL = fileMeta.Path,
                                    ImageAbsURL = $"{_currentUserService.WebRoot()}/{folderName}/{subFolderName}/{fileMeta.FileName}",
                                    ImageRelURL = $"/{folderName}/{subFolderName}/{fileMeta.FileName}",
                                    Description = request.Description,

                                };

                                designs.Add(design);

                            }
                        }
                    }






                    if (designs.Any())
                    {
                        _context.FestiveDesigns.AddRange(designs);
                        int savResult = await _context.SaveChangesAsync();

                        if (savResult > 0)
                        {
                            return new FestiveDesignCRUDResponse
                            {
                                Designs = designs.Select(e => new FestiveDesignModel
                                {
                                    Name = e.Name,
                                    Description = e.Description,
                                    Category = e.Category.ToString(),
                                    Active = e.Active,
                                    Id = e.Id,
                                    Image = e.ImageAbsURL,
                                    Selected = false
                                }).ToList(),
                                Response = new BaseResponse
                                {
                                    Message = $"Festive {(designs.Count == 1 ? "design" : "designs")} successfully created.",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }
                            };
                        }


                        else
                        {

                            foreach (FestiveDesign festiveDesign in designs)
                            {
                                await festiveDesign.ImageURL.DeleteFileFromPathAsync();
                            }

                            return new FestiveDesignCRUDResponse
                            {
                                Designs = new List<FestiveDesignModel>(),
                                Response = new BaseResponse
                                {
                                    Message = $"Festive {(designs.Count == 1 ? "design" : "designs")} could not be created.",
                                    Result = RequestResult.Error,
                                    Succeeded = false,
                                    Title = "Unknown Error"
                                }
                            };
                        }



                    }




                }



                return new FestiveDesignCRUDResponse
                {
                    Designs = new List<FestiveDesignModel>(),
                    Response = new BaseResponse
                    {
                        Message = "Couldn\'t create festive designs from an empty list.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Empty Request"
                    }
                };





            }
            catch (Exception ex)
            {

                return new FestiveDesignCRUDResponse
                {
                    Designs = new List<FestiveDesignModel>(),
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


                if (!string.IsNullOrWhiteSpace(request.Id))
                {
                    List<string> ids = request.Id.Split(',').ToList();

                    if (ids.Any())
                    {
                        List<FestiveDesign> designs = await _context.FestiveDesigns.Where(e => ids.Contains(e.Id)).ToListAsync();
                        if (designs.Any())
                        {
                            foreach (FestiveDesign festiveDesign in designs)
                            {
                                await festiveDesign.ImageURL.DeleteFileFromPathAsync();
                            }


                            _context.FestiveDesigns.RemoveRange(designs);
                            int rs = await _context.SaveChangesAsync();


                            if (rs > 0)
                            {
                                return new BaseResponse
                                {
                                    Message = $"Festive design{(designs.Count == 1 ? "" : "s")} deleted successfully.",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                };
                            }


                            return new BaseResponse
                            {
                                Message = $"Festive design{(designs.Count == 1 ? "" : "s")} deletion failed. Please try again later.",
                                Result = RequestResult.Error,
                                Succeeded = false,
                                Title = "Unknown Error"
                            };


                        }


                        return new BaseResponse
                        {
                            Message = $"Festive {(ids.Count == 1 ? "Design" : "Designs")} not found. {(ids.Count == 1 ? "This design" : "These designs")} may have been deleted.",
                            Result = RequestResult.Error,
                            Succeeded = false,
                            Title = "Empty Result"

                        };

                    }
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

        public async Task<FestiveDesignCRUDResponse> Create(BaseFestiveDesignModel request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (Enum.TryParse(request.Category, out FestiveDesignCategory category))
                {
                    string outputPath = $"{_hostingEnvironment.WebRootPath}\\{folderName}\\{subFolderName}";
                    if (!(await outputPath.DirectoryExistAsync()))
                    {
                        await Task.Run(() => Directory.CreateDirectory(outputPath));
                    }

                    string filePath = outputPath + "\\" + request.Name;
                    FileMeta fileMeta = await @request.Image.SaveBase64AsImage(filePath);


                    if (fileMeta.Path != null)
                    {
                        FestiveDesign design = new()
                        {
                            Name = request.Name,
                            Active = request.Active,
                            Category = category,
                            ImageURL = fileMeta.Path,
                            ImageAbsURL = $"{_currentUserService.WebRoot()}/{folderName}/{subFolderName}/{fileMeta.FileName}",
                            ImageRelURL = $"/{folderName}/{subFolderName}/{fileMeta.FileName}",
                            Description = request.Description,

                        };



                        _context.FestiveDesigns.Add(design);
                        int svResult = await _context.SaveChangesAsync();

                        if (svResult > 0)
                        {
                            return new FestiveDesignCRUDResponse
                            {
                               Designs = new List<FestiveDesignModel>
                            {
                                new FestiveDesignModel
                                {
                                    Id = design.Id,
                                    Description = design.Description,
                                    Image = design.ImageAbsURL,
                                    Name = design.Name,
                                    Active = design.Active,
                                    Category = design.Category.ToString(),
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
                            await design.ImageURL.DeleteFileFromPathAsync();

                            return new FestiveDesignCRUDResponse
                            {
                                Designs = new List<FestiveDesignModel>(),
                                Response = new BaseResponse
                                {
                                    Message = "Festive design creation failed. Please try again later.",
                                    Result = RequestResult.Error,
                                    Succeeded = false,
                                    Title = "Error"
                                }
                            };



                        }




                    }






                }

                return new FestiveDesignCRUDResponse
                {
                    Designs = new List<FestiveDesignModel>(),
                    Response = new BaseResponse
                    {
                        Message = "Category of festive design not understood.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Invalid Category"
                    }
                };


            }
            catch (Exception ex)
            {

                return new FestiveDesignCRUDResponse
                {
                    Designs = new List<FestiveDesignModel>(),
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

                if (await _context.FestiveDesigns.AnyAsync(e => e.Id == @request.Id))
                {
                    FestiveDesign design = await _context.FestiveDesigns.FindAsync(@request.Id);
                    if (design != null)
                    {
                        await design.ImageURL.DeleteFileFromPathAsync();

                        _context.FestiveDesigns.Remove(design);
                        int deleteResult = await _context.SaveChangesAsync();

                        if (deleteResult > 0)
                        {
                            return new BaseResponse
                            {
                                Message = "Festive design deleted successfully.",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            };
                        };

                    }



                    return new BaseResponse
                    {
                        Message = "Festive design deletion did not succeed. Please try again later.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Unknown Error"
                    };


                }


                return new BaseResponse
                {
                    Message = "Festive design not found. This design may have been deleted.",
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = "Design Not Found"
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

        public async Task<FestiveDesignCRUDResponse> Designs()
        {
            try
            {
                if (await _context.FestiveDesigns.AnyAsync())
                {
                    List<FestiveDesignModel> designs = await _context.FestiveDesigns.Select(x => new FestiveDesignModel
                    {
                        Name = x.Name,
                        Description = x.Description,
                        Id = x.Id,
                        Active = x.Active,
                        Category = x.Category.ToString(),
                        Image = x.ImageAbsURL,
                        Selected = false

                    })
                   .OrderBy(e => e.Name)
                   .ToListAsync();

                    if (designs.Any())
                    {
                        return new FestiveDesignCRUDResponse
                        {
                            Designs = designs,
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

                return new FestiveDesignCRUDResponse
                {
                    Designs = new List<FestiveDesignModel>(),
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
                return new FestiveDesignCRUDResponse
                {
                    Designs = new List<FestiveDesignModel>(),
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

        public async Task<FestiveDesignCRUDResponse> Update(BaseFestiveDesignModel request)
        {
            try
            {

                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);


                if (Enum.TryParse(@request.Category, out FestiveDesignCategory category))
                {

                    if (await _context.FestiveDesigns.AnyAsync(e => e.Id == request.Id))
                    {

                        FestiveDesign design = await _context.FestiveDesigns.FindAsync(@request.Id);
                        if (design != null)
                        {

                            FileMeta fileMeta = new() { FileName = null, Path = null, ImageByte = null };

                            if (request.Image.IsBase64String())
                            {

                                string outputPath = $"{_hostingEnvironment.WebRootPath}\\{folderName}\\{subFolderName}\\{@request.Name}";
                                fileMeta = await design.ImageURL.RenameFile(request.Image, outputPath);
                            }

                            design.Name = request.Name;
                            design.Description = request.Description;
                            design.ImageAbsURL = string.IsNullOrEmpty(fileMeta.Path) ? design.ImageAbsURL : $"{_currentUserService.WebRoot()}/{folderName}/{subFolderName}/{fileMeta.FileName}";
                            design.ImageRelURL = string.IsNullOrEmpty(fileMeta.Path) ? design.ImageRelURL : $"/{folderName}/{subFolderName}/{fileMeta.FileName}";
                            design.ImageURL = string.IsNullOrEmpty(fileMeta.Path) ? design.ImageURL : fileMeta.Path;
                            design.Active = request.Active;
                            design.Category = category;


                            _context.FestiveDesigns.Update(design);
                            int upResult = await _context.SaveChangesAsync();

                            if (upResult > 0)
                            {
                                return new FestiveDesignCRUDResponse()
                                {
                                    Designs = new List<FestiveDesignModel>()
                            {
                                new FestiveDesignModel
                                {
                                    Id = design.Id,
                                    Description = design.Description,
                                    Image = design.ImageAbsURL,
                                    Name = design.Name,
                                    Active = design.Active,
                                    Category = design.Category.ToString(),
                                    Selected = false
                                }
                            },

                                    Response = new BaseResponse
                                    {
                                        Message = "Festive design successfully updated.",
                                        Result = RequestResult.Success,
                                        Succeeded = true,
                                        Title = "Successful"
                                    }
                                };
                            }


                            return new FestiveDesignCRUDResponse
                            {
                                Designs = new List<FestiveDesignModel>(),
                                Response = new BaseResponse
                                {
                                    Message = "Festive design update failed. Please try again later.",
                                    Result = RequestResult.Error,
                                    Succeeded = false,
                                    Title = "Unknown Eror"
                                }
                            };




                        }

                    }


                    return new FestiveDesignCRUDResponse
                    {
                        Designs = new List<FestiveDesignModel>(),
                        Response = new BaseResponse
                        {
                            Message = "Festive design not found. This design may have been deleted.",
                            Result = RequestResult.Error,
                            Succeeded = false,
                            Title = "Design Not Found"
                        }
                    };


                }


                return new FestiveDesignCRUDResponse()
                {
                    Designs = new List<FestiveDesignModel>(),
                    Response = new BaseResponse
                    {
                        Message = "Category of festive design not understood.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Invalid Category"
                    }
                };




            }
            catch (Exception ex)
            {

                return new FestiveDesignCRUDResponse()
                {
                    Designs = new List<FestiveDesignModel>(),
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

        public async Task<PublicFeststiveDesignResponse> Public()
        {
            try
            {
                if (await _context.FestiveDesigns.AnyAsync())
                {
                    List<PublicFestiveDesignModel> designs = await _context.FestiveDesigns.Where(e => e.Active == true).Select(e => new PublicFestiveDesignModel
                    {

                        Id = e.Id,
                        Image = e.ImageAbsURL,
                        Category = e.Category.ToString(),
                        ImageString = e.ImageURL,
                        Name = e.Name,

                    }).ToListAsync();


                    if (designs.Any())
                    {
                        foreach (PublicFestiveDesignModel design in designs)
                        {
                            design.ImageString = await design.ImageString.GetImageFileBase64();
                        }


                        return new PublicFeststiveDesignResponse
                        {
                            Designs = designs,
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


                return new PublicFeststiveDesignResponse
                {
                    Designs = new List<PublicFestiveDesignModel>(),
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

                return new PublicFeststiveDesignResponse
                {
                    Designs = new List<PublicFestiveDesignModel>(),
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
