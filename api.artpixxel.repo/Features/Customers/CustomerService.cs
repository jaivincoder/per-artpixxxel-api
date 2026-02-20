

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Customers;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.artpixxel.repo.Extensions;
using api.artpixxel.data.Models;
using static api.artpixxel.repo.Utils.Customers.CustomerFilterUtils;

namespace api.artpixxel.repo.Features.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly ArtPixxelContext _context;
        public CustomerService(ArtPixxelContext context)
        {
            _context = context;
        }
        public async Task<CustomersResponse> CustomersPaginated(CustomerFilterData filter)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@filter", filter);

                bool emptyFilter = EmptyFilter(filter);
                var pred = ApplyFilter(@filter);

                List<Customer> query = emptyFilter ?

                    await _context.Customers
                        .OrderBy(n => n.User.FirstName)
                       .Skip(@filter.Skip)
                       .Take(@filter.PageSize)
                       .Include(c => c.Category)
                       .Include(u => u.User)
                       .Include(ct => ct.City).ThenInclude(s => s.State).ThenInclude(c => c.Country).ThenInclude(fl => fl.Flag)
                       .ToListAsync()

                       :

                    await _context.Customers
                       .Where(pred)
                       .OrderBy(n => n.User.FirstName)
                       .Skip(@filter.Skip)
                       .Take(@filter.PageSize)
                       .Include(c => c.Category)
                       .Include(u => u.User)
                       .Include(ct => ct.City).ThenInclude(s => s.State).ThenInclude(c => c.Country).ThenInclude(fl => fl.Flag)
                       .ToListAsync()
                       ;


                List<CustomerModel> customers = query.Any() ? query.Select(c => new CustomerModel
                {
                    Id = c.Id,
                    FirstName = c.User.FirstName,
                    FullName = string.Format("{0} {1}", c.User.FirstName, c.User.LastName),
                    LastName = c.User.LastName,
                    EmailAddress = c.User.Email,
                    MobileNumber = c.User.PhoneNumber,
                    HomeAddress = c.User.HomeAddress,
                    Username = c.User.UserName,
                    UserId = c.User.Id,
                    TotalOrder = decimal.Round(c.TotalOrder, 2, MidpointRounding.AwayFromZero),
                    LastOrder = decimal.Round(c.LastOrder, 2, MidpointRounding.AwayFromZero),
                    AverageOrder = decimal.Round(c.AverageOrder, 2, MidpointRounding.AwayFromZero),
                    AdditionalMobileNumber = c.AdditionalMobileNumber,
                    State = string.IsNullOrEmpty(c.CityId) ? new BaseOption { Id = "", Name = "" } : new BaseOption { Id = c.City.State.Id, Name = c.City.State.StateName },
                    Country = string.IsNullOrEmpty(c.CityId) ? new CountryOption { Id = "", Name = "", Flag = "" } :
                             new CountryOption { Id = c.City.State.Country.Id, Name = c.City.State.Country.Name, Flag = c.City.State.Country.Flag.Name },
                    City = string.IsNullOrEmpty(c.CityId) ? new BaseOption { Id = "", Name = "" } : new BaseOption { Id = c.City.Id, Name = c.City.CityName },
                    DOB = c.User.DOB == null ? "" : c.User.DOB.GetValueOrDefault().ToString(DefaultDateFormat.ddMMyyyy),
                    Gender = c.User.Gender.ToString(),
                    IsOnline = c.User.IsOnline,
                    DateRegistered = c.CreatedOn.ToString(DefaultDateFormat.ddMMyyyy),
                    LastLogin = c.User.IsOnline ? "Online" : c.User.LastLogin == null ? "" : c.User.LastLogin.GetValueOrDefault().GetElapsedTime(),
                    LastLoginDate = c.User.LastLogin == null ? null : c.User.LastLogin.GetValueOrDefault().ToString(DefaultDateFormat.ddMMyyyyhhmmsstt),
                    Category = string.IsNullOrEmpty(c.CategoryId) ? new CustomerCategoryOption { Id = "", Name = "", Count = 0, Color = "" } : new CustomerCategoryOption { Id = c.Category.Id, Name = c.Category.Name, Count = 0, Color = c.Category.ColorCode },
                    CategoryId = c.CategoryId,
                    Passport = string.IsNullOrEmpty(c.User.PassportAbsURL) ? AssetDefault.DefaultUserImage : c.User.PassportAbsURL


                }).ToList()
               : new List<CustomerModel>();


                customers = ApplySort(customers, @filter);

                return new CustomersResponse() 
                    { 
                    Customers = customers,
                      TotalCount = decimal.Round( emptyFilter ? await _context.Customers.CountAsync() : await _context.Customers.Where(pred).CountAsync(), 0, MidpointRounding.AwayFromZero)
                    };

            }
            catch (Exception)
            {

                throw;
            }
        }





        public async Task<CustomerData> Customers(Pagination pagination)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@pagination", pagination);


                CustomersResponse CustomerResponse = new()
                {
                    Customers = await _context.Customers
                        .Include(c => c.Category)
                        .Include(u => u.User)
                        .Include(ct => ct.City).ThenInclude(s => s.State).ThenInclude(c => c.Country).ThenInclude(fl => fl.Flag)
                        .OrderBy(n => n.User.FirstName)
                        .Skip(@pagination.Skip)
                        .Take(@pagination.PageSize).Select(c => new CustomerModel
                        {
                            Id = c.Id,
                            FirstName = c.User.FirstName,
                            FullName = string.Format("{0} {1}", c.User.FirstName, c.User.LastName),
                            LastName = c.User.LastName,
                            EmailAddress = c.User.Email,
                            MobileNumber = c.User.PhoneNumber,
                            HomeAddress = c.User.HomeAddress,
                            Username = c.User.UserName,
                            UserId = c.User.Id,
                            TotalOrder = decimal.Round(c.TotalOrder, 2, MidpointRounding.AwayFromZero),
                            LastOrder = decimal.Round(c.LastOrder, 2, MidpointRounding.AwayFromZero),
                            AverageOrder = decimal.Round(c.AverageOrder, 2, MidpointRounding.AwayFromZero),
                            AdditionalMobileNumber = c.AdditionalMobileNumber,
                            State = string.IsNullOrEmpty(c.CityId) ? new BaseOption { Id = "", Name = "" } : new BaseOption { Id = c.City.State.Id, Name = c.City.State.StateName },
                            Country = string.IsNullOrEmpty(c.CityId) ? new CountryOption { Id = "", Name = "", Flag = "" } :
                              new CountryOption { Id = c.City.State.Country.Id, Name = c.City.State.Country.Name, Flag = c.City.State.Country.Flag.Name },
                            City = string.IsNullOrEmpty(c.CityId) ? new BaseOption { Id = "", Name = "" } : new BaseOption { Id = c.City.Id, Name = c.City.CityName },
                            DOB = c.User.DOB == null ? "" : c.User.DOB.GetValueOrDefault().ToString(DefaultDateFormat.ddMMyyyy),
                            Gender = c.User.Gender.ToString(),
                            IsOnline = c.User.IsOnline,
                            DateRegistered = c.CreatedOn.ToString(DefaultDateFormat.ddMMyyyy),
                            LastLogin = c.User.IsOnline ? "Online" : c.User.LastLogin == null ? "" :  c.User.LastLogin.GetValueOrDefault().GetElapsedTime(),
                            Category = string.IsNullOrEmpty(c.CategoryId) ? new CustomerCategoryOption { Id = "", Name = "", Count = 0, Color =""} : new CustomerCategoryOption { Id = c.Category.Id, Name = c.Category.Name, Count = 0 , Color=c.Category.ColorCode},
                            CategoryId = c.CategoryId,
                            Passport = string.IsNullOrEmpty(c.User.PassportAbsURL) ? AssetDefault.DefaultUserImage : c.User.PassportAbsURL


                        }).ToListAsync(),

                    TotalCount = decimal.Round(await _context.Customers.CountAsync(), 0, MidpointRounding.AwayFromZero)
                };


                List<BaseOption> Cities = await _context.Cities.Select(s => new BaseOption
                {
                    Id = s.Id,
                    Name = s.CityName
                }).ToListAsync();

                List<BaseOption> States = await _context.States.Select(s => new BaseOption
                {
                    Id = s.Id,
                    Name = s.StateName
                }).ToListAsync();

                List<CountryOption> Countries = await _context.Countries.Include(fl => fl.Flag).Select(s => new CountryOption
                {
                    Id = s.Id,
                    Name = s.Name,
                    Flag = s.Flag.Name
                }).ToListAsync();




                List<CustomerCategoryOption> categories = await _context.CustomerCategories.Select(c => new CustomerCategoryOption
                {
                    Id = c.Id,
                    Name = c.Name,
                    Color = c.ColorCode,
                    Count = decimal.Round(c.Customers.Count(), 0, MidpointRounding.AwayFromZero)
                }).ToListAsync();



                return new CustomerData
                {
                    Categories = categories,
                    Cities = Cities,
                    Countries = Countries,
                    CustomerResponse = CustomerResponse,
                    States = States
                };



            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<CustomerCRUDResponse> BulkDelete(CustomerBulkDelete request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                if (@request.Ids.Any())
                {
                    List<Customer> customers = await _context.Customers.Where(b => @request.Ids.Contains(b.Id)).ToListAsync();
                    if (customers.Any())
                    {
                        _context.Customers.RemoveRange(customers);
                        await _context.SaveChangesAsync();

                        return new CustomerCRUDResponse
                        {
                            Customer = await CustomersPaginated(@request.FilterData),
                            Response = new BaseResponse
                            {
                                Message = customers.Count == 1 ? "Customer removed." : "Customers removed.",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            }
                        };
                    }
                }

                return new CustomerCRUDResponse
                {
                    Customer = await CustomersPaginated(request.FilterData),
                    Response = new BaseResponse
                    {
                        Message = "Couldn't delete empty list of customers",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Empty Customer List"
                    }
                };

            }
            catch (Exception ex)
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                return new CustomerCRUDResponse
                {
                    Customer = await CustomersPaginated(@request.FilterData),
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

        public async Task<CustomerCRUDResponse> BulkUpdate(CustomerBulkUpdate request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (request.Ids.Any())
                {
                    List<Customer> customers = await _context.Customers.Where(b => @request.Ids.Contains(b.Id)).ToListAsync();
                    if (customers.Any())
                    {
                        if(await _context.CustomerCategories.AnyAsync(e => e.Id == @request.CategoryId))
                        {
                            foreach(Customer customer in customers)
                            {
                                customer.CategoryId = @request.CategoryId;
                            }


                            _context.Customers.UpdateRange(customers);
                            int updateResult = await _context.SaveChangesAsync();
                            if(updateResult > 0)
                            {
                                return new CustomerCRUDResponse
                                {
                                    Customer = await CustomersPaginated(@request.FilterData),
                                    Response = new BaseResponse
                                    {
                                        Message = customers.Count == 1 ? "Customer category updated." : "Customers category updated.",
                                        Result = RequestResult.Success,
                                        Succeeded = true,
                                        Title = "Successful"
                                    }
                                };
                            }
                        }
                    };
                }

                return new CustomerCRUDResponse
                {
                    Customer = await CustomersPaginated(request.FilterData),
                    Response = new BaseResponse
                    {
                        Message = "Couldn't update empty list of customers",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Empty Customer List"
                    }
                };
            }
            catch (Exception ex)
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                return new CustomerCRUDResponse
                {
                    Customer = await CustomersPaginated(@request.FilterData),
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

        public async Task<CustomerCRUDResponse> Delete(CustomerDelete request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                if (!string.IsNullOrEmpty(request.Id))
                {
                    if(await _context.Customers.AnyAsync(e => e.Id == @request.Id))
                    {
                        Customer customer = await _context.Customers.FindAsync(@request.Id);
                        if(customer != null)
                        {
                            _context.Customers.Remove(customer);
                            await _context.SaveChangesAsync();

                            return new CustomerCRUDResponse
                            {
                                Customer = await CustomersPaginated(@request.FilterData),
                                Response = new BaseResponse
                                {
                                    Message = "Customer removal successful.",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }
                            };
                        }
                    }
                }

                return new CustomerCRUDResponse
                {
                    Customer = await CustomersPaginated(@request.FilterData),
                    Response = new BaseResponse
                    {
                        Message = "Reference to customer couldn't be resolved. This customer may have been deleted",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Null Customer Reference"
                    }
                };

            }
            catch (Exception ex)
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                return new CustomerCRUDResponse
                {
                    Customer = await CustomersPaginated(@request.FilterData),
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

        public async Task<CustomerCRUDResponse> Update(CustomerUpdate request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if(await _context.Customers.AnyAsync(e => e.Id == @request.Id))
                {
                    Customer customer = await _context.Customers.FindAsync(@request.Id);
                    if(customer != null)
                    {
                        if(await _context.CustomerCategories.AnyAsync(e => e.Id == @request.CategoryId))
                        {
                            if(customer.CategoryId == request.CategoryId)
                            {
                                return new CustomerCRUDResponse
                                {
                                    Customer = await CustomersPaginated(@request.FilterData),
                                    Response = new BaseResponse
                                    {
                                        Message = "Category same as customer's category",
                                        Result = RequestResult.Warn,
                                        Succeeded = false,
                                        Title = "Redundant Request"
                                    }
                                };
                            }

                            else
                            {
                                customer.CategoryId = request.CategoryId;
                                _context.Customers.Update(customer);
                                int saveResult = await _context.SaveChangesAsync();

                                if (saveResult > 0)
                                {
                                    return new CustomerCRUDResponse
                                    {
                                        Customer = await CustomersPaginated(@request.FilterData),
                                        Response = new BaseResponse
                                        {
                                            Message = "Customer's category changed",
                                            Result = RequestResult.Success,
                                            Succeeded = true,
                                            Title = "Successful"
                                        }
                                    };
                                }

                            }
                          
                        }
                    }
                }

                return new CustomerCRUDResponse
                {
                    Customer = await CustomersPaginated(@request.FilterData),
                    Response = new BaseResponse
                    {
                        Message = "Reference to customer couldn't be resolved. This customer may have been deleted",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Null Customer Reference"
                    }
                };

            }
            catch (Exception ex)
            {

                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                return new CustomerCRUDResponse
                {
                    Customer = await CustomersPaginated(@request.FilterData),
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

        public async Task<CustomerResponse> Customer(BaseId request)
        {
            try
            {
                if(await _context.Customers.AnyAsync(e => e.Id == request.Id))
                {

                    return new CustomerResponse
                    {
                        Customer = await _context.Customers.Where(e => e.Id == request.Id)
                        .Include(e => e.User)
                        .Include(ct => ct.Category)
                        .Include(ct => ct.City).ThenInclude(s => s.State).ThenInclude(c => c.Country).ThenInclude(fl => fl.Flag)
                        .Select(c => new CustomerModel
                        {
                            Id = c.Id,
                            FirstName = c.User.FirstName,
                            FullName = string.Format("{0} {1}", c.User.FirstName, c.User.LastName),
                            LastName = c.User.LastName,
                            EmailAddress = c.User.Email,
                            MobileNumber = c.User.PhoneNumber,
                            HomeAddress = c.User.HomeAddress,
                            Username = c.User.UserName,
                            UserId = c.User.Id,
                            TotalOrder = decimal.Round(c.TotalOrder, 2, MidpointRounding.AwayFromZero),
                            LastOrder = decimal.Round(c.LastOrder, 2, MidpointRounding.AwayFromZero),
                            AverageOrder = decimal.Round(c.AverageOrder, 2, MidpointRounding.AwayFromZero),
                            AdditionalMobileNumber = c.AdditionalMobileNumber,
                            State = string.IsNullOrEmpty(c.CityId) ? new BaseOption { Id = "", Name = "" } : new BaseOption { Id = c.City.State.Id, Name = c.City.State.StateName },
                            Country = string.IsNullOrEmpty(c.CityId) ? new CountryOption { Id = "", Name = "", Flag = "" } :
                             new CountryOption { Id = c.City.State.Country.Id, Name = c.City.State.Country.Name, Flag = c.City.State.Country.Flag.Name },
                            City = string.IsNullOrEmpty(c.CityId) ? new BaseOption { Id = "", Name = "" } : new BaseOption { Id = c.City.Id, Name = c.City.CityName },
                            DOB = c.User.DOB == null ? "" : c.User.DOB.GetValueOrDefault().ToString(DefaultDateFormat.ddMMyyyy),
                            Gender = c.User.Gender.ToString(),
                            IsOnline = c.User.IsOnline,
                            DateRegistered = c.CreatedOn.ToString(DefaultDateFormat.ddMMyyyy),
                            LastLogin = c.User.IsOnline ? "Online" : c.User.LastLogin == null ? "" : c.User.LastLogin.GetValueOrDefault().GetElapsedTime(),
                            LastLoginDate = c.User.LastLogin == null ? null : c.User.LastLogin.GetValueOrDefault().ToString(DefaultDateFormat.ddMMyyyyhhmmsstt),
                            Category = string.IsNullOrEmpty(c.CategoryId) ? new CustomerCategoryOption { Id = "", Name = "", Count = 0, Color = "" } : new CustomerCategoryOption { Id = c.Category.Id, Name = c.Category.Name, Count = 0, Color = c.Category.ColorCode },
                            CategoryId = c.CategoryId,
                            Passport = string.IsNullOrEmpty(c.User.PassportAbsURL) ? AssetDefault.DefaultUserImage : c.User.PassportAbsURL


                        }).FirstOrDefaultAsync(),
                        Response = new BaseResponse
                        {
                            Message = string.Empty,
                            Result = RequestResult.Success,
                            Succeeded = true,
                            Title = string.Empty
                        }
                    };
                }

                return new CustomerResponse
                {
                    Customer = new CustomerModel(),
                    Response = new BaseResponse
                    {
                        Message = "Reference to customer could not be resolved. This customer may have been deleted.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Null Customer Reference"
                    }
                };

            }
            catch (Exception ex)
            {

                return new CustomerResponse
                {
                    Customer = new CustomerModel(),
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
