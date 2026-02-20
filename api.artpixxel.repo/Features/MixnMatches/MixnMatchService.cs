
using api.artpixxel.data.Features.MixnMatches;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using api.artpixxel.data.Features.Common;
using Microsoft.Data.SqlClient;
using api.artpixxel.data.Models;
using Microsoft.AspNetCore.Hosting;
using api.artpixxel.repo.Extensions;
using System.Collections.Generic;
using api.artpixxel.data.Services;

namespace api.artpixxel.repo.Features.MixnMatches
{
    public class MixnMatchService : IMixnMatchService
    {
        private readonly ArtPixxelContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICurrentUserService _currentUserService;
        public MixnMatchService(ArtPixxelContext context, IWebHostEnvironment hostingEnvironment, ICurrentUserService currentUserService)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _currentUserService = currentUserService;
        }

        public async Task<MixMatchCRUDResponse> BatchDelete(MixnMatchBatchDelete request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (@request.MixnMatchIds.Any())
                {
                    List<MixnMatch> mixnMatches = await _context.MixnMatches.Where(b => @request.MixnMatchIds.Contains(b.Id)).ToListAsync();
                    if (mixnMatches.Any())
                    {
                        foreach(var mixnmatch in mixnMatches)
                        {
                            await mixnmatch.ImageURL.DeleteFileFromPathAsync();
                           
                        }

                        _context.MixnMatches.RemoveRange(mixnMatches);
                        int deleteResult =  await _context.SaveChangesAsync();
                        if(deleteResult > 0)
                        {
                            return new MixMatchCRUDResponse
                            {
                                MixMatches = await MixnMatches(@request.PaginationFilter),
                                Response = new BaseResponse
                                {
                                    Message = @request.MixnMatchIds.Count == 1 ? "mixNMatch removed" : "mixNMatches removed",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }

                            };
                        }

                    }


                }


                return new MixMatchCRUDResponse
                {
                    MixMatches = await MixnMatches(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = "No mixnmatch referenced. Delete operation failed",
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

                return new MixMatchCRUDResponse
                {
                    MixMatches = await MixnMatches(@request.PaginationFilter),
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

        public async Task<MixMatchCRUDResponse> BatchUpdate(MixnMatchBatchUpdate request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                if (request.MixnMatchIds.Any())
                {
                    List<MixnMatch> mixnMatches = await _context.MixnMatches.Where(b => @request.MixnMatchIds.Contains(b.Id)).ToListAsync();
                    if (mixnMatches.Any())
                    {
                        foreach (var mixnmatch in mixnMatches)
                        {
                            mixnmatch.CategoryId = _context.MixnMatchCategories.Find(@request.CategoryId).Id;
                            mixnmatch.Description = string.IsNullOrEmpty(@request.Description) ? mixnmatch.Description : @request.Description;

                        }

                        _context.MixnMatches.UpdateRange(mixnMatches);
                        int updateResult = await _context.SaveChangesAsync();

                        if(updateResult > 0)
                        {
                            return new MixMatchCRUDResponse
                            {
                                MixMatches = await MixnMatches(@request.PaginationFilter),
                                Response = new BaseResponse
                                {
                                    Message = @request.MixnMatchIds.Count == 1 ? "mixNMatch updated." : "mixNMatches updated",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }

                            };
                        }
                    }
                }


                return new MixMatchCRUDResponse
                {
                    MixMatches = await MixnMatches(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = "No mixnmatch referenced. Update operation failed",
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

                return new MixMatchCRUDResponse
                {
                    MixMatches = await MixnMatches(@request.PaginationFilter),
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

        public async Task<MixMatchCRUDResponse> Create(MixnMatchRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                string outputPath = string.Empty;
                decimal price = 0m;

                if (!string.IsNullOrEmpty(@request.MixNMatch.MixnmatchImage))
                {

                    Size size = await _context.Sizes.Where(e => e.Default == true).FirstOrDefaultAsync();
                    if (size != null)
                    {
                        price = size.Amount;
                    }

                    else
                    {
                        if (await _context.Metas.AnyAsync(e => e.MetaType == MetaType.MixnMatch))
                        {
                            Meta _price = await _context.Metas.Where(e => e.MetaType == MetaType.MixnMatch).FirstOrDefaultAsync();
                            if (_price != null)
                            {
                                price = decimal.Parse(_price.Value);
                            }
                        }

                    }


                   
                    outputPath = _hostingEnvironment.WebRootPath + "\\images\\MixnMatch\\" + @request.MixNMatch.MixnmatchName;
                    FileMeta fileMeta = await @request.MixNMatch.MixnmatchImage.SaveBase64AsImage(outputPath);

                    MixnMatch mixnMatch = new()
                    {
                        Name = request.MixNMatch.MixnmatchName,
                        CategoryId = _context.MixnMatchCategories.Find(@request.MixNMatch.MixnmatchCategoryCategoryId).Id,
                        ImageURL = fileMeta.Path,
                        ImageAbsURL = _currentUserService.WebRoot() + "/images/MixnMatch/" + fileMeta.FileName,
                        Price = price,
                        ImageRelURL = "/images/MixnMatch/" + fileMeta.FileName,
                        Description = @request.MixNMatch.MixnmatchDescription,

                    };

                     _context.MixnMatches.Add(mixnMatch);
                    int saveResult = await _context.SaveChangesAsync();
                    if(saveResult > 0)
                    {
                        return new MixMatchCRUDResponse
                        {
                            MixMatches = await MixnMatches(request.PaginationFilter),
                            Response = new BaseResponse
                            {
                                Message = "mixNmatch created.",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            }
                        };
                    }

                   
                }




                return new MixMatchCRUDResponse
                {
                    MixMatches = await MixnMatches(request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = "Empty Image File",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Invalid Request"
                    }
                };


               
            }
            catch (Exception ex)
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                return new MixMatchCRUDResponse
                {
                    MixMatches = await MixnMatches(@request.PaginationFilter),
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

        public async Task<MixMatchCRUDResponse> CreateMultiple(MultipleMixnMatchRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (request.MixNMatches.Any())
                {
                    List<MixnMatch> mixnmatches = new();
                    string outputPath = string.Empty;
                    decimal price = 0m;

                    Size size = await _context.Sizes.Where(e => e.Default == true).FirstOrDefaultAsync();
                    if (size != null)
                    {
                        price = size.Amount;
                    }

                    else
                    {
                        if (await _context.Metas.AnyAsync(e => e.MetaType == MetaType.MixnMatch))
                        {
                            Meta _price = await _context.Metas.Where(e => e.MetaType == MetaType.MixnMatch).FirstOrDefaultAsync();
                            if (_price != null)
                            {
                                price = decimal.Parse(_price.Value);
                            }
                        }

                    }

                    foreach (MixnMatchBase mix in @request.MixNMatches)
                    {


                        if (!string.IsNullOrEmpty(mix.MixnmatchImage))
                        {
                            outputPath = _hostingEnvironment.WebRootPath + "\\images\\MixnMatch\\" + mix.MixnmatchName;
                            FileMeta fileMeta = await mix.MixnmatchImage.SaveBase64AsImage(outputPath);

                            MixnMatch mixnMatch = new()
                            {
                                Name = mix.MixnmatchName,
                                CategoryId = _context.MixnMatchCategories.Find(mix.MixnmatchCategoryCategoryId).Id,
                                ImageAbsURL = _currentUserService.WebRoot() + "/images/MixnMatch/" + fileMeta.FileName,
                                ImageRelURL = "/images/MixnMatch/" + fileMeta.FileName,
                                Price = price,
                                ImageURL = fileMeta.Path,
                                Description = mix.MixnmatchDescription,

                            };

                            mixnmatches.Add(mixnMatch);

                          


                        }

                    }




                    if (mixnmatches.Any())
                    {
                        _context.MixnMatches.AddRange(mixnmatches);
                        int saveResult = await _context.SaveChangesAsync();
                        if (saveResult > 0)
                        {
                            return new MixMatchCRUDResponse
                            {
                                MixMatches = await MixnMatches(@request.PaginationFilter),
                                Response = new BaseResponse
                                {
                                    Message = "mixNmatches created.",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }
                            };
                        }


                    }



                }



                return new MixMatchCRUDResponse
                {
                    MixMatches = await MixnMatches(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = "Couldn't create mixnmatches from empty list",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Empty MixnMatch List"
                    }

                };
            }
            catch (Exception ex)
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                return new MixMatchCRUDResponse
                {
                    MixMatches = await MixnMatches(@request.PaginationFilter),
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

        public async Task<MixMatchCRUDResponse> Delete(MixMatchDeleteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.MixnMatches.AnyAsync(e => e.Id == @request.Id.Id))
                {
                    MixnMatch mixnMatch = await _context.MixnMatches.FindAsync(request.Id.Id);
                    if(mixnMatch != null)
                    {
                        await mixnMatch.ImageURL.DeleteFileFromPathAsync();
                        _context.MixnMatches.Remove(mixnMatch);
                        int deleteResult = await _context.SaveChangesAsync();
                        if(deleteResult > 0)
                        {
                            return new MixMatchCRUDResponse
                            {
                                MixMatches = await MixnMatches(@request.PaginationFilter),
                                Response = new BaseResponse
                                {
                                    Message = "mixNMatch removed",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }

                            };
                        }


                    }
                }

                return new MixMatchCRUDResponse
                {
                    MixMatches = await MixnMatches(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = "Reference to mixNMatch could not be resolved. Item may heve been deleted",
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

                return new MixMatchCRUDResponse
                {
                    MixMatches = await MixnMatches(@request.PaginationFilter),
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

        public async Task<MixnMatchResponse> MixnMatches(PaginationFilter Filter)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@Filter", Filter);

                return   string.IsNullOrEmpty(@Filter.Category.Id) ?
                    
                    new MixnMatchResponse
                {
                    MixNMatchData = await _context.MixnMatches.Include(c => c.Category)
                    .OrderBy(n => n.Name)
                    .Skip(@Filter.Pagination.Skip)
                    .Take(@Filter.Pagination.PageSize)
                    .Select(m => new MixnMatchData
                    {
                        MixnmatchId = m.Id,
                        MixnmatchCategoryCategoryId = m.CategoryId,
                        MixnmatchCategoryCategoryName = m.Category.Name,
                        MixnmatchDescription = m.Description,
                        MixnmatchName = m.Name,
                        MixnmatchImageURL = m.ImageURL,
                        MixnmatchImage =   m.ImageAbsURL
                        // Image = m.Image == null ? "" : "data:image/png;base64," + Convert.ToBase64String(m.Image, 0, m.Image.Length)

                    }).ToListAsync(),
                    TotalCount = decimal.Round(await _context.MixnMatches.CountAsync(), 0 ,MidpointRounding.AwayFromZero)

                }
                    :


                new MixnMatchResponse
                {
                    MixNMatchData = await _context.MixnMatches.Where(c => c.CategoryId == @Filter.Category.Id).Include(c => c.Category)
                    .OrderBy(n => n.Name)
                    .Skip(@Filter.Pagination.Skip)
                    .Take(@Filter.Pagination.PageSize)
                    .Select(m => new MixnMatchData
                    {
                        MixnmatchId = m.Id,
                        MixnmatchCategoryCategoryId = m.CategoryId,
                        MixnmatchCategoryCategoryName = m.Category.Name,
                        MixnmatchDescription = m.Description,
                        MixnmatchImageURL = m.ImageAbsURL,
                        MixnmatchName = m.Name,
                        MixnmatchImage = m.ImageURL.Base64FromImage().Result

                    }).ToListAsync(),
                    TotalCount = decimal.Round( await _context.MixnMatches.Where(c => c.CategoryId == @Filter.Category.Id).CountAsync(), 0, MidpointRounding.AwayFromZero)

                }


                ;
                
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<MixnMatchModel> Public()
        {
            try
            {
                MixnMatchModel mixnMatchModel = new() { MixnMatches = new List<MixnMatchContent>() };
                if(await _context.MixnMatchCategories.AnyAsync())
                {
                    List<MixnMatchContent> contents = await _context.MixnMatchCategories.Include(e => e.MixnMatches).Select(e => 
                    new MixnMatchContent 
                    { 
                      Category = e.Name,
                      Images = e.MixnMatches.Select( i => new ImageModel
                      {
                          Id = i.Id,
                          Name = i.Name,
                          Price = i.Price,
                          Description = i.Description,
                          Image = i.ImageAbsURL
                      }).ToList()
                    
                    }).ToListAsync();

                    MixnMatchContent all = new MixnMatchContent
                    {
                        Category = "All",
                        Images = contents.SelectMany(e => e.Images).ToList()
                     };

                    mixnMatchModel.MixnMatches.Add(all);
                    mixnMatchModel.MixnMatches.AddRange(contents);

                }


                return mixnMatchModel;
               


            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<MixMatchCRUDResponse> Update(MixnMatchRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.MixnMatches.AnyAsync(e => e.Id == @request.MixNMatch.MixnmatchId))
                {
                    MixnMatch mixnMatch = await _context.MixnMatches.FindAsync(@request.MixNMatch.MixnmatchId);
                    if(mixnMatch != null)
                    {
                        FileMeta fileMeta = new() { FileName = null, Path = null, ImageByte = null };

                       


                        if (request.MixNMatch.MixnmatchImage.IsBase64String())
                        {
                            string outputPath = _hostingEnvironment.WebRootPath + "\\images\\MixnMatch\\" + @request.MixNMatch.MixnmatchName;
                            fileMeta = await mixnMatch.ImageURL.RenameFile(request.MixNMatch.MixnmatchImage, outputPath);
                        }


                            mixnMatch.Name = request.MixNMatch.MixnmatchName;
                            mixnMatch.CategoryId = _context.MixnMatchCategories.Find(request.MixNMatch.MixnmatchCategoryCategoryId).Id;
                            mixnMatch.Description = request.MixNMatch.MixnmatchDescription;
                            mixnMatch.ImageAbsURL = string.IsNullOrEmpty(fileMeta.Path) ? mixnMatch.ImageAbsURL : _currentUserService.WebRoot() + "/images/MixnMatch/" + fileMeta.FileName;
                            mixnMatch.ImageRelURL = string.IsNullOrEmpty(fileMeta.Path) ? mixnMatch.ImageRelURL : "/images/MixnMatch/" + fileMeta.FileName;
                            mixnMatch.ImageURL = string.IsNullOrEmpty(fileMeta.Path) ? mixnMatch.ImageURL : fileMeta.Path;

                            _context.MixnMatches.Update(mixnMatch);
                            int updateResult = await _context.SaveChangesAsync();
                            if (updateResult > 0)
                            {
                                return new MixMatchCRUDResponse
                                {
                                    MixMatches = await MixnMatches(@request.PaginationFilter),
                                    Response = new BaseResponse
                                    {
                                        Message = "mixNmatch updated",
                                        Result = RequestResult.Success,
                                        Succeeded = true,
                                        Title = "Successful"
                                    }

                                };
                            }
                        
                      
                    }
                }


               

                return new MixMatchCRUDResponse
                {
                    MixMatches = await MixnMatches(@request.PaginationFilter),
                    Response = new BaseResponse
                    {
                        Message = "Reference to mixNMatch could not be resolved. Item may heve been deleted",
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

                return new MixMatchCRUDResponse
                {
                    MixMatches = await MixnMatches(@request.PaginationFilter),
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
