


using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.artpixxel.repo.Extensions;
using Microsoft.Data.SqlClient;
using api.artpixxel.data.Models;
using Microsoft.AspNetCore.Hosting;
using api.artpixxel.data.Features.Carousels;
using api.artpixxel.data.Features.Common;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using static api.artpixxel.repo.Utils.Carousels.CarouselFilterUtils;
using api.artpixxel.data.Services;


namespace api.artpixxel.repo.Features.HomeSliders
{
    public class CarouselService : ICarouselService
    {
        private readonly ArtPixxelContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICurrentUserService _currentUserService;
        public CarouselService(ArtPixxelContext context, IWebHostEnvironment hostingEnvironment, ICurrentUserService currentUserService)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _currentUserService = currentUserService;
        }

        public async Task<CarouselCRUDResponse> BatchCreate(BatchCarouselRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (@request.Request.Any())
                {
                    List<Carousel> carousels = new();

                    foreach (var carousel in request.Request)
                    {
                        string carouselOutputPath = _hostingEnvironment.WebRootPath + "\\images\\Carousel\\" + (string.IsNullOrEmpty(carousel.LinkLabel) ? DateTime.Today.Ticks.ToString() + "_" + Guid.NewGuid().ToString("N") : carousel.LinkLabel);
                        FileMeta fileMeta = await carousel.Image.SaveBase64AsImage(carouselOutputPath);
                        if (fileMeta.ImageByte != null && fileMeta.Path != null)
                        {
                            Carousel _carousel = new()
                            {
                                BodyText = carousel.BodyText,
                                Heading = carousel.Heading,
                                ImageURL = fileMeta.Path,
                                ImageAbsURL = _currentUserService.WebRoot() + "/images/Carousel/" + fileMeta.FileName,
                                ImageRelURL = "/images/Carousel/" + fileMeta.FileName,
                                HeadingColour = string.IsNullOrEmpty(carousel.HeadingColour) ? null : carousel.HeadingColour,
                                BackgroundColour = string.IsNullOrEmpty(carousel.BackgroundColour) ? null : carousel.BackgroundColour,
                                BodyTextColour = string.IsNullOrEmpty(carousel.BodyTextColour) ? null : carousel.BodyTextColour,
                                LinkLabelColour = string.IsNullOrEmpty(carousel.LinkLabelColour) ? null : carousel.LinkLabelColour,
                                Type = (CarouselType)Enum.Parse(typeof(CarouselType), carousel.Type),
                                Active = carousel.Active,
                                Link = carousel.Link,
                                LinkLabel = carousel.LinkLabel
                            };

                            carousels.Add(_carousel);
                        }
                    }



                    if (carousels.Any())
                    {
                        _context.Carousels.AddRange(carousels);
                        int saveResult = await _context.SaveChangesAsync();

                        if (saveResult > 0)
                        {
                            return new CarouselCRUDResponse
                            {
                                Carousel = await CarouselsPaginated(@request.Filter),
                                Response = new BaseResponse
                                {
                                    Title = "Successful",
                                    Message = carousels.Count == 1 ? "Carousel creation successful" : "Carousels creation successful",
                                    Result = RequestResult.Success,
                                    Succeeded = true
                                }

                            };
                        }



                    }

                }


                return new CarouselCRUDResponse
                {
                    Carousel = await CarouselsPaginated(@request.Filter),
                    Response = new BaseResponse
                    {
                        Title = "Empty Request",
                        Message = "Carousel couldn't be created from an empty list",
                        Result = RequestResult.Error,
                        Succeeded = false
                    }

                };
            }
            catch (Exception ex)
            {

                return new CarouselCRUDResponse
                {
                    Carousel = await CarouselsPaginated(@request.Filter),
                    Response = new BaseResponse
                    {
                        Title = ex.Source,
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false
                    }

                };
            }
        }

        public async Task<CarouselCRUDResponse> Create(CarouselRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

               
                string fileName = string.IsNullOrEmpty(@request.Request.LinkLabel) ? DateTime.Today.Ticks.ToString() + "_" + Guid.NewGuid().ToString("N") : @request.Request.LinkLabel;
                string carouselOutputPath = _hostingEnvironment.WebRootPath + "\\images\\Carousel\\" + fileName;
                FileMeta fileMeta = await @request.Request.Image.SaveBase64AsImage(carouselOutputPath);

               

                if (fileMeta.ImageByte != null && fileMeta.Path != null)
                {
                    Carousel carousel = new()
                    {
                        BodyText = @request.Request.BodyText,
                        Heading = @request.Request.Heading,
                        ImageURL = fileMeta.Path,
                        ImageAbsURL = _currentUserService.WebRoot() + "/images/Carousel/"+fileMeta.FileName,
                        ImageRelURL = "/images/Carousel/" + fileMeta.FileName,
                        HeadingColour = string.IsNullOrEmpty(request.Request.HeadingColour) ? null : request.Request.HeadingColour,
                        BackgroundColour = string.IsNullOrEmpty(request.Request.BackgroundColour) ? null : request.Request.BackgroundColour,
                        BodyTextColour = string.IsNullOrEmpty(request.Request.BodyTextColour) ? null : request.Request.BodyTextColour,
                        LinkLabelColour = string.IsNullOrEmpty(request.Request.LinkLabelColour) ? null : request.Request.LinkLabelColour,
                        Active = @request.Request.Active,
                        Type = (CarouselType)Enum.Parse(typeof(CarouselType), request.Request.Type),
                        Link = @request.Request.Link,
                        LinkLabel = @request.Request.LinkLabel
                    };

                    _context.Carousels.Add(carousel);
                  int  saveResult =  await _context.SaveChangesAsync();
                    if(saveResult > 0)
                    {
                        return new CarouselCRUDResponse
                        {
                            Carousel = await CarouselsPaginated(@request.Filter),
                            Response = new BaseResponse
                            {
                                Title = "Successful",
                                Message = "Carousel creation successful",
                                Result = RequestResult.Success,
                                Succeeded = true
                            }

                        };
                    }
                }

                return new CarouselCRUDResponse
                {
                    Carousel = await CarouselsPaginated(@request.Filter),
                    Response = new BaseResponse
                    {
                        Title = "Unknown Error",
                        Message = "An error occurred. Carousel creation failed. Please try again later.",
                        Result = RequestResult.Error,
                        Succeeded = false
                    }

                };

            }
            catch (Exception ex)
            {

                return new CarouselCRUDResponse
                {
                    Carousel = await CarouselsPaginated(@request.Filter),
                    Response = new BaseResponse
                    {
                        Title = ex.Source,
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false
                    }

                };
            }
        }

        public async Task<CarouselResponse> Carousels(Pagination pagination)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@pagination", pagination);

                if (await _context.Carousels.AnyAsync())
                {
                    return new CarouselResponse
                    {
                        Carousels = await RecentCarousels(@pagination),
                        TotalCarousels = decimal.Round(await _context.Carousels.CountAsync(), 0, MidpointRounding.AwayFromZero)
                    };
                }

                return new CarouselResponse
                {
                    Carousels = new List<CarouselModel>()
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<List<CarouselModel>> RecentCarousels(Pagination pagination)
        {
            try
            {
                
                return await _context.Carousels
                    .OrderBy(e => e.CreatedOn)
                    .Skip(pagination.Skip)
                    .Take(pagination.PageSize).Select(c => new CarouselModel 
                    { 
                      Id = c.Id,
                      BodyText = c.BodyText,
                      Heading = c.Heading,
                      Active = c.Active,
                      Type = c.Type.ToString(),
                      Image = c.ImageAbsURL,
                      BackgroundColour = c.BackgroundColour,
                      LinkLabelColour = c.LinkLabelColour,
                      BodyTextColour = c.BodyTextColour,
                      HeadingColour = c.HeadingColour,
                     // Image = c.ImageURL.Base64FromImage().Result,
                      Link = c.Link,
                      LinkLabel = c.LinkLabel
                    }).ToListAsync();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<CarouselResponse> PublicCarousels()
        {
            try
            {

                
                return new CarouselResponse
                {
                    Carousels =  await _context.Carousels
                    .Where(e => e.Active == true)
                    .OrderBy(e => e.CreatedOn)
                    .Select(c => new CarouselModel
                    {
                        Id = c.Id,
                        BodyText = c.BodyText,
                        Heading = c.Heading,
                        Active = c.Active,
                        Image = c.ImageAbsURL,
                        Type = c.Type.ToString(),
                        BackgroundColour = c.BackgroundColour,
                        LinkLabelColour = c.LinkLabelColour,
                        BodyTextColour = c.BodyTextColour,
                        HeadingColour = c.HeadingColour,
                        Link = string.IsNullOrEmpty(c.Link) ? "/" : c.Link,
                        LinkLabel = c.LinkLabel
                    }).ToListAsync()
                };
            }
            catch (Exception)
            {

                return new CarouselResponse
                {
                    Carousels = new List<CarouselModel>()
                };
            }
        }

        public async Task<CarouselCRUDResponse> Update(CarouselRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                if(await _context.Carousels.AnyAsync(e => e.Id == @request.Request.Id))
                {
                    Carousel carousel = await _context.Carousels.FindAsync(@request.Request.Id);
                    if(carousel != null)
                    {


                        FileMeta fileMeta = new() { FileName = null, ImageByte = null, Path = null };


                        if (@request.Request.Image.IsBase64String())
                        {
                            string carouselOutputPath = _hostingEnvironment.WebRootPath + "\\images\\Carousel\\" + (string.IsNullOrEmpty(carousel.LinkLabel) ? string.IsNullOrEmpty(@request.Request.LinkLabel) ? DateTime.Today.Ticks.ToString() + "_" + Guid.NewGuid().ToString("N") : @request.Request.LinkLabel : carousel.LinkLabel);
                            fileMeta = await carousel.ImageURL.RenameFile(@request.Request.Image, carouselOutputPath);
                        }



                        carousel.BodyText = @request.Request.BodyText;
                        carousel.Heading = @request.Request.Heading;
                        carousel.ImageURL = string.IsNullOrEmpty(fileMeta.Path) ? carousel.ImageURL : fileMeta.Path;
                        carousel.ImageAbsURL = string.IsNullOrEmpty(fileMeta.Path) ? carousel.ImageAbsURL : _currentUserService.WebRoot() + "/images/Carousel/" + fileMeta.FileName;
                        carousel.ImageRelURL = string.IsNullOrEmpty(fileMeta.Path) ? carousel.ImageRelURL : "/images/Carousel/" + fileMeta.FileName;
                        carousel.HeadingColour = string.IsNullOrEmpty(request.Request.HeadingColour) ? null : request.Request.HeadingColour;
                        carousel.BackgroundColour = string.IsNullOrEmpty(request.Request.BackgroundColour) ? null : request.Request.BackgroundColour;
                        carousel.BodyTextColour = string.IsNullOrEmpty(request.Request.BodyTextColour) ? null : request.Request.BodyTextColour;
                        carousel.LinkLabelColour = string.IsNullOrEmpty(request.Request.LinkLabelColour) ? null : request.Request.LinkLabelColour;
                        carousel.Type = (CarouselType) Enum.Parse(typeof(CarouselType), request.Request.Type);
                        carousel.Active = @request.Request.Active;
                        carousel.Link = @request.Request.Link;
                        carousel.LinkLabel = @request.Request.LinkLabel;


                        _context.Carousels.Update(carousel);
                        int updateResult = await _context.SaveChangesAsync();
                        if(updateResult > 0)
                        {
                            return new CarouselCRUDResponse
                            {
                                Carousel = await CarouselsPaginated(@request.Filter),
                                Response = new BaseResponse
                                {
                                    Title = "Successful",
                                    Message = "Update was Successful",
                                    Result = RequestResult.Success,
                                    Succeeded = true
                                }

                            };

                        }


                    }


                }


                return new CarouselCRUDResponse
                {
                    Carousel = await CarouselsPaginated(@request.Filter),
                    Response = new BaseResponse
                    {
                        Title = "Null Carousel Item Reference",
                        Message = "Referenced carousel item was not found. This item may have been deleted",
                        Result = RequestResult.Error,
                        Succeeded = false
                    }

                };



            }
            catch (Exception ex)
            {

                return new CarouselCRUDResponse
                {
                    Carousel = await CarouselsPaginated(@request.Filter),
                    Response = new BaseResponse
                    {
                        Title = ex.Source,
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false
                    }

                };
            }
        }

        public async Task<CarouselCRUDResponse> Delete(CarouselDeleteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if(await _context.Carousels.AnyAsync(e => e.Id == @request.Id))
                {

                    Carousel carousel = await _context.Carousels.FindAsync(@request.Id);
                    if(carousel != null)
                    {

                        await carousel.ImageURL.DeleteFileFromPathAsync();

                        _context.Carousels.Remove(carousel);
                        await _context.SaveChangesAsync();

                        return new CarouselCRUDResponse
                        {
                            Carousel = await CarouselsPaginated(@request.Filter),
                            Response = new BaseResponse
                            {
                                Title = "Successful",
                                Message = "Carousel deleted.",
                                Result = RequestResult.Success,
                                Succeeded = true
                            }
                        };


                    }
                }


                return new CarouselCRUDResponse
                {
                    Carousel = await CarouselsPaginated(@request.Filter),
                    Response = new BaseResponse
                    {
                        Title = "Null Carousel Reference",
                        Message = "Reference to carousel couldn't be resolved. This carousel may have been deleted.",
                        Result = RequestResult.Error,
                        Succeeded = false
                    }
                };
            }
            catch (Exception ex)
            {

                return new CarouselCRUDResponse
                {
                    Carousel = await CarouselsPaginated(@request.Filter),
                    Response = new BaseResponse
                    {
                        Title = ex.Source,
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false
                    }

                };
            }
        }

        public async Task<CarouselResponse> CarouselsPaginated(Filter filter)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@filter", filter);

                bool emptyFilter = EmptyFilter(filter);
                var pred = ApplyFilter(@filter);

                List<Carousel> carousels = emptyFilter ? await _context.Carousels
                    .OrderBy(e => e.CreatedOn)
                    .Skip(filter.Skip)
                    .Take(filter.PageSize).ToListAsync()

                    :

                    await _context.Carousels
                    .Where(pred)
                    .Skip(filter.Skip)
                    .OrderBy(e => e.CreatedOn)
                    .Take(filter.PageSize).ToListAsync()
                    ;

                List<CarouselModel> carouselModels =  carousels.Any() ? carousels.Select(c => new CarouselModel
                {
                    Id = c.Id,
                    BodyText = c.BodyText,
                    Active = c.Active,
                    Heading = c.Heading,
                    Image = c.ImageAbsURL,
                    Type = c.Type.ToString(),
                    BackgroundColour = c.BackgroundColour,
                    LinkLabelColour = c.LinkLabelColour,
                    BodyTextColour = c.BodyTextColour,
                    HeadingColour = c.HeadingColour,
                    Link = c.Link,
                    LinkLabel = c.LinkLabel
                }).ToList() : new List<CarouselModel>()
                ;

                carouselModels = ApplySort(carouselModels, filter);

                return new CarouselResponse
                {
                    Carousels = carouselModels,

                    TotalCarousels = decimal.Round( emptyFilter ? await _context.Carousels.CountAsync() : await _context.Carousels.Where(pred).CountAsync(), 0, MidpointRounding.AwayFromZero)
                };

               


            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
