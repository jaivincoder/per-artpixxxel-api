

using api.artpixxel.data.Features.CustomerCategories;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Models;

namespace api.artpixxel.repo.Features.CustomerCategories
{
    public class CustomerCategoryService : ICustomerCategoryService
    {
        private readonly ArtPixxelContext _context;
        public CustomerCategoryService(ArtPixxelContext context)
        {
            _context = context;
        }

        public async Task<CustomerCategoryCRUDResponse> BatchCreate(List<CustomerCategoryRequest> request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                if (request.Any())
                {
                    List<CustomerCategory> customerCategories = request.Select(c => new CustomerCategory
                    {
                        Name = c.CustomerCategoryName,
                        ColorCode = c.CustomerCategoryColorCode,
                        IsDefault = c.CustomerCategoryDefault,
                        Description = c.CustomerCategoryDescription

                    }).ToList();

                    if (customerCategories.Any())
                    {
                        _context.CustomerCategories.AddRange(customerCategories);
                        int saveResult = await _context.SaveChangesAsync();

                        if(saveResult > 0)
                        {
                            return new CustomerCategoryCRUDResponse
                            {
                                Categories = await Categories(),
                                Response = new BaseResponse
                                {
                                    Message = "Customer"+ (customerCategories.Count() == 1 ? " category ": " categories ")+  "created.",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }
                            };

                        }
                    }

                }


                return new CustomerCategoryCRUDResponse
                {
                    Categories = await Categories(),
                    Response = new BaseResponse
                    {
                        Message = "Couldn't create customer categories from an empty list. Request terminated.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Empty Category List"
                    }
                };

            }
            catch (Exception ex)
            {
                return new CustomerCategoryCRUDResponse
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

        public async Task<CustomerCategoryCRUDResponse> BatchDelete(CustomerCategoryBatchDelete request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (@request.Ids.Any())
                {
                    List<CustomerCategory> customerCategories = await _context.CustomerCategories.Where(b => @request.Ids.Contains(b.Id)).Include(e => e.Customers).ToListAsync();
                    if (customerCategories.Any())
                    {
                        if(!customerCategories.Any(e => e.Customers.Any()))
                        {
                            _context.CustomerCategories.RemoveRange(customerCategories);
                            await _context.SaveChangesAsync();

                            return new CustomerCategoryCRUDResponse
                            {
                                Categories = await Categories(),
                                Response = new BaseResponse
                                {
                                    Message = customerCategories.Count == 1 ? "Customer category '"+ customerCategories.First().Name +"' deleted" : "Customer categories deleted",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }
                            };

                        }

                        return new CustomerCategoryCRUDResponse
                        {
                            Categories = await Categories(),
                            Response = new BaseResponse
                            {
                                Message = "Couldn't delete a customer that has customers. Request rejected.",
                                Result = RequestResult.Error,
                                Succeeded = false,
                                Title = "Invalid Request"
                            }
                        };

                    }
                }


                return new CustomerCategoryCRUDResponse
                {
                    Categories = await Categories(),
                    Response = new BaseResponse
                    {
                        Message = "Couldn't delete an empty list. Request rejected.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Empty Customer Category"
                    }
                };

            }
            catch (Exception ex)
            {

                return new CustomerCategoryCRUDResponse
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

        public async Task<List<CustomerCategoryResponse>> Categories()
        {
            try
            {
                return await _context.CustomerCategories.OrderBy(e => e.Name).Select(c => new 
                CustomerCategoryResponse
                { 
                  CustomerCategoryId = c.Id,
                  CustomerCategoryColorCode = c.ColorCode,
                  CustomerCategoryName = c.Name,
                  CustomerCategoryDefault = c.IsDefault,
                  CustomerCategoryDescription = c.Description,
                  CustomerCategoryCustomerCount = decimal.Round(c.Customers.Count(), 0, MidpointRounding.AwayFromZero)
                }).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<CustomerCategoryCRUDResponse> Create(CustomerCategoryRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                if(!await _context.CustomerCategories.AnyAsync(e => e.Name == @request.CustomerCategoryName))
                {
                    CustomerCategory category = new()
                    {
                        Name = @request.CustomerCategoryName,
                        ColorCode = @request.CustomerCategoryColorCode,
                        IsDefault = @request.CustomerCategoryDefault,
                        Description = @request.CustomerCategoryDescription
                    };

                    _context.CustomerCategories.Add(category);
                    int createResult = await _context.SaveChangesAsync();

                    if(createResult > 0)
                    {
                        return new CustomerCategoryCRUDResponse
                        {
                            Categories = await Categories(),
                            Response = new BaseResponse
                            {
                                Message = string.Format("Category '{0}' created.", @request.CustomerCategoryName),
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            }

                        };
                    }

                }


                return new CustomerCategoryCRUDResponse
                {
                    Categories = await Categories(),
                    Response = new BaseResponse
                    {
                        Message = string.Format("Category '{0}' already exists.", @request.CustomerCategoryName),
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Duplicate Category"
                    }
                };
            }
            catch (Exception ex)
            {

                return new CustomerCategoryCRUDResponse
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

        public async Task<CustomerCategoryCRUDResponse> Delete(BaseId request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if(await _context.CustomerCategories.AnyAsync(e => e.Id == @request.Id))
                {
                    CustomerCategory customerCategory = await _context.CustomerCategories.FindAsync(request.Id);
                    if(customerCategory != null)
                    {
                        _context.CustomerCategories.Remove(customerCategory);
                        await _context.SaveChangesAsync();

                        return new CustomerCategoryCRUDResponse
                        {
                            Categories = await Categories(),
                            Response = new BaseResponse
                            {
                                Message = string.Format("Category '{0}' removed", customerCategory.Name),
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            }
                        };
                    }
                }


                return new CustomerCategoryCRUDResponse
                {
                    Categories = await Categories(),
                    Response = new BaseResponse
                    {
                        Message = "Reference to customer category couldn't be resolved. This category may have been deleted.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Null Customer Category Reference"
                    }
                };



            }
            catch (Exception ex)
            {

                return new CustomerCategoryCRUDResponse
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

        public async Task<BaseBoolResponse> Duplicate(BaseOption request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.CustomerCategories.AnyAsync(e => e.Name == @request.Name && e.Id != @request.Id))
                {
                    return new BaseBoolResponse
                    {
                        Exist = true,
                        Message = "Customer Category: <i>" + @request.Name + " </i> already exists "
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

                if (await _context.CustomerCategories.AnyAsync(e => e.Name == @request.Name))
                {
                    return new BaseBoolResponse
                    {
                        Exist = true,
                        Message = "Customer Category: <i>" + @request.Name + " </i> already exists "
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

        public async Task<CustomerCategoryCRUDResponse> Update(CustomerCategoryRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.CustomerCategories.AnyAsync(e => e.Id == request.CustomerCategoryId))
                {
                    CustomerCategory customerCategory = await _context.CustomerCategories.FindAsync(request.CustomerCategoryId);
                    if(customerCategory != null)
                    {
                        string oldName = customerCategory.Name;

                        customerCategory.Name = request.CustomerCategoryName;
                        customerCategory.IsDefault = request.CustomerCategoryDefault;
                        customerCategory.ColorCode = request.CustomerCategoryColorCode;
                        customerCategory.Description = request.CustomerCategoryDescription;


                        _context.CustomerCategories.Update(customerCategory);
                        int updateResult = await _context.SaveChangesAsync();

                        if(updateResult > 0)
                        {
                            return new CustomerCategoryCRUDResponse
                            {
                                Categories = await Categories(),
                                Response = new BaseResponse
                                {
                                    Message = string.Format("Customer category '{0}' updated", oldName),
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }
                            };
                        }
                    }
                }


                return new CustomerCategoryCRUDResponse
                {
                    Categories = await Categories(),
                    Response = new BaseResponse
                    {
                        Message = string.Format("Reference to Category with reference number '{0}' couldn't be resolved. This category may have been deleted.", @request.CustomerCategoryId),
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Null Category Reference"
                    }
                };

            }
            catch (Exception ex)
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                return new CustomerCategoryCRUDResponse
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
    }
}
