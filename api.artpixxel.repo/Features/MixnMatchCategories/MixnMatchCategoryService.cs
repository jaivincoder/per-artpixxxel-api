

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.MixnMatchCategories;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using api.artpixxel.data.Models;
using api.artpixxel.data.Features.MixnMatches;
using api.artpixxel.repo.Extensions;

namespace api.artpixxel.repo.Features.MixnMatchCategories
{
  public  class MixnMatchCategoryService : IMixnMatchCategoryService
    {

        private readonly ArtPixxelContext _context;
        public MixnMatchCategoryService(ArtPixxelContext context)
        {
            _context = context;
        }

        public async Task<List<BaseOption>> Categories()
        {
            try
            {
                return await _context.MixnMatchCategories.OrderBy(n => n.Name).Select(e => new BaseOption 
                { 
                    Id = e.Id,
                    Name = e.Name

                }).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<MixnMatchResponse> CategoryMixnMatches(PaginationFilter paginationFilter)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@paginationFilter", paginationFilter);

                return new MixnMatchResponse
                {
                    MixNMatchData = await _context.MixnMatches.Where(c => c.CategoryId == @paginationFilter.Category.Id).Include(c => c.Category)
                    .OrderBy(n => n.Name)
                    .Skip(@paginationFilter.Pagination.Skip)
                    .Take(@paginationFilter.Pagination.PageSize)
                    .Select(m => new MixnMatchData
                    {
                        MixnmatchId = m.Id,
                        MixnmatchCategoryCategoryId = m.CategoryId,
                        MixnmatchCategoryCategoryName = m.Category.Name,
                        MixnmatchDescription = m.Description,
                        MixnmatchImageURL = m.ImageURL,
                        MixnmatchName = m.Name,
                        MixnmatchImage = m.ImageURL.Base64FromImage().Result

                    }).ToListAsync(),
                    TotalCount = decimal.Round(await _context.MixnMatches.Where(c => c.CategoryId == @paginationFilter.Category.Id).CountAsync(), 0, MidpointRounding.AwayFromZero)

                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<MixNMatchCategoryCRUDResponse> Create(MixnMatchCategoryRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if(!await _context.MixnMatchCategories.AnyAsync(mc => mc.Name == @request.MixnmatchCategoryName))
                {
                   await _context.MixnMatchCategories.AddAsync(new MixnMatchCategory
                    {
                      Name = @request.MixnmatchCategoryName,
                      Description = @request.MixnmatchCategoryDescription
                   });

                    int createResult = await _context.SaveChangesAsync();

                    if(createResult > 0)
                    {
                        return new MixNMatchCategoryCRUDResponse
                        {
                            Categories = await Categories(),
                            Response = new BaseResponse
                            {
                                Message = string.Format("Category '{0}' created", @request.MixnmatchCategoryName),
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            }
                        };
                    }
                }


                return new MixNMatchCategoryCRUDResponse
                {
                    Categories = await Categories(),
                    Response = new BaseResponse
                    {
                        Message = string.Format("Category '{0}' already exist", @request.MixnmatchCategoryName),
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Duplicate Category"
                    }
                };

            }
            catch (Exception ex)
            {

                return new MixNMatchCategoryCRUDResponse
                {
                    Categories = await Categories(),
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

        public async Task<MixnMatchCategoryCRUD> CreateCategory(MixnMatchCategoryRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if(!await _context.MixnMatchCategories.AnyAsync(e => e.Name  == request.MixnmatchCategoryName))
                {

                    MixnMatchCategory matchCategory = new()
                    {
                        Name = @request.MixnmatchCategoryName,
                        Description = @request.MixnmatchCategoryDescription

                    };


                    await _context.MixnMatchCategories.AddAsync(matchCategory);
                    int result = await _context.SaveChangesAsync();

                    if(result > 0)
                    {

                        return new MixnMatchCategoryCRUD
                        {
                            Categories = await MixnMatchCategories(),
                            Response = new BaseResponse
                            {
                                Message = string.Format("Category '{0}' created", request.MixnmatchCategoryName),
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            }
                        };
                    }



                }


                return new MixnMatchCategoryCRUD
                {
                    Categories = await MixnMatchCategories(),
                    Response = new BaseResponse
                    {
                        Message = string.Format("Category '{0}' already exist", request.MixnmatchCategoryName),
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Duplicate Entry"
                    }
                };

            }
            catch (Exception ex)
            {

                return new MixnMatchCategoryCRUD
                {
                    Categories = await MixnMatchCategories(),
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

        public async Task<MixNMatchCategoryCRUDResponse> CreateMultiple(List<MixnMatchCategoryRequest> requests)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", requests);

                if (@requests.Any())
                {


                    List<MixnMatchCategory> mixnMatchCategories = @requests.Select(cc => new MixnMatchCategory
                    {
                        Name = cc.MixnmatchCategoryName,
                        Description = cc.MixnmatchCategoryDescription
                    }).ToList();



                    await _context.MixnMatchCategories.AddRangeAsync(mixnMatchCategories);
                    int createResult = await _context.SaveChangesAsync();

                    if(createResult > 0)
                    {
                        return new MixNMatchCategoryCRUDResponse
                        {
                            Categories = await Categories(),
                            Response = new BaseResponse
                            {
                                Message = "Categories successfully created",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            }
                        };
                    }
                }



                return new MixNMatchCategoryCRUDResponse
                {
                    Categories = await Categories(),
                    Response = new BaseResponse
                    {
                        Message = "Couldn't create mixNmatch category from empty list",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Empty Request"
                    }
                };

            }
            catch (Exception ex)
            {

                return new MixNMatchCategoryCRUDResponse
                {
                    Categories = await Categories(),
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

        public async Task<MixnMatchCategoryCRUD> CreateMultipleCategories(List<MixnMatchCategoryRequest> requests)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", requests);

                if (requests.Any())
                {


                    List<MixnMatchCategory> mixnMatchCategories = @requests.Select(c => new MixnMatchCategory
                    {
                        Name = c.MixnmatchCategoryName,
                        Description = c.MixnmatchCategoryDescription

                    }).ToList();


                    await _context.MixnMatchCategories.AddRangeAsync(mixnMatchCategories);
                    int saveResult = await _context.SaveChangesAsync();

                    if(saveResult > 0)
                    {


                        return new MixnMatchCategoryCRUD
                        {
                            Categories = await MixnMatchCategories(),
                            Response = new BaseResponse
                            {
                                Message = "MixnMatch categories created.",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            }
                        };
                    }
                }


                return new MixnMatchCategoryCRUD
                {
                    Categories = await MixnMatchCategories(),
                    Response = new BaseResponse
                    {
                        Message = "Couldn't create mixnMatch categories from an empty list",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Empty Object List"
                    }
                };


            }
            catch (Exception ex)
            {
                return new MixnMatchCategoryCRUD
                {
                    Categories = await MixnMatchCategories(),
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

        public async Task<MixnMatchCategoryCRUD> Delete(BaseId request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.MixnMatchCategories.AnyAsync(e => e.Id == @request.Id))
                {

                    MixnMatchCategory mixnMatchCategory = await _context.MixnMatchCategories.FindAsync(@request.Id);
                    if(mixnMatchCategory != null)
                    {
                        if(!await _context.MixnMatches.AnyAsync(e => e.CategoryId == mixnMatchCategory.Id)) //no attached categories yet
                        {

                            _context.MixnMatchCategories.Remove(mixnMatchCategory);
                            int deleteResult = await _context.SaveChangesAsync();

                            if(deleteResult > 0)
                            {
                                return new MixnMatchCategoryCRUD
                                {
                                    Response = new BaseResponse
                                    {
                                        Message = string.Format("MixnMatch category '{0}' removed.", mixnMatchCategory.Name),
                                        Result = RequestResult.Success,
                                        Succeeded = false,
                                        Title = "Successful"
                                    },

                                    Categories = await MixnMatchCategories()
                                };
                            }


                        }



                        return new MixnMatchCategoryCRUD
                        {
                            Response = new BaseResponse
                            {
                                Message = "MixnMatch category has mixnmatches. Explicit deletion of mixnmatches in this category required to carry this task.",
                                Result = RequestResult.Error,
                                Succeeded = false,
                                Title = "Invalid Object State"
                            },

                            Categories = await MixnMatchCategories()
                        };
                    }

                }


                return new MixnMatchCategoryCRUD
                {
                    Response = new BaseResponse
                    {
                        Message = "Reference to mixNmatch category could not be resolved. This category may have been deleted.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Null Object Reference"
                    },

                    Categories = await MixnMatchCategories()
                };

            }
            catch (Exception ex)
            {

                return new MixnMatchCategoryCRUD
                {
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    },

                    Categories = await MixnMatchCategories()
                };
            }
        }

        public async Task<BaseBoolResponse> Duplicate(BaseOption request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.MixnMatchCategories.AnyAsync(e => e.Name == @request.Name && e.Id != @request.Id))
                {
                    return new BaseBoolResponse
                    {
                        Exist = true,
                        Message = "MixNMatch Category: <i>" + @request.Name + " </i> already exists "
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

                if (await _context.MixnMatchCategories.AnyAsync(e => e.Name == @request.Name))
                {
                    return new BaseBoolResponse
                    {
                        Exist = true,
                        Message = "MixNMatch Category: <i>" + @request.Name + " </i> already exists "
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

        public async Task<List<MixnMatchCategoryBase>> MixnMatchCategories()
        {
            try
            {
                return await _context.MixnMatchCategories.Include(mx => mx.MixnMatches).OrderBy(n => n.Name).Select(c => 
                new MixnMatchCategoryBase 
                {
                    MixnmatchCategoryId = c.Id,
                    MixnmatchCategoryName = c.Name,
                    MixnmatchCategoryDescription = c.Description,
                    MixnmatchCategoryImageCount = decimal.Round(c.MixnMatches.Count(), 0, MidpointRounding.AwayFromZero)
                }
                ).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<MixnMatchCategoryCRUD> Update(MixnMatchCategoryRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                string oldName = string.Empty;

                if(await _context.MixnMatchCategories.AnyAsync(e => e.Id == @request.MixnmatchCategoryId))
                {


                    MixnMatchCategory mixnMatchCategory = await _context.MixnMatchCategories.FindAsync(@request.MixnmatchCategoryId);
                    if(mixnMatchCategory != null)
                    {
                        oldName = mixnMatchCategory.Name;

                        mixnMatchCategory.Name = @request.MixnmatchCategoryName;
                        mixnMatchCategory.Description = @request.MixnmatchCategoryDescription;


                        _context.MixnMatchCategories.Update(mixnMatchCategory);
                        int updateResult = await _context.SaveChangesAsync();

                        if(updateResult > 0)
                        {
                            return new MixnMatchCategoryCRUD
                            {
                                Categories = await MixnMatchCategories(),
                                Response = new BaseResponse
                                {
                                    Message = string.Format("MixnMatch Category '{0}' updated", oldName),
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }
                            };

                        }
                        



                    }


                }


                return new MixnMatchCategoryCRUD
                {
                    Categories = await MixnMatchCategories(),
                    Response = new BaseResponse
                    {
                        Message = string.Format("Reference to Category '{0}' couldn't be resolved. This mixNmacth category may have been deleted.", request.MixnmatchCategoryName),
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Null Object Reference"
                    }
                };




            }
            catch (Exception ex)
            {

                return new MixnMatchCategoryCRUD
                {
                    Categories = await MixnMatchCategories(),
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
