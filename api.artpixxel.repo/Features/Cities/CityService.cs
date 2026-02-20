

using api.artpixxel.data.Features.Cities;
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Models;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static api.artpixxel.repo.Utils.Cities.CityFilterUtils;

namespace api.artpixxel.repo.Features.Cities
{
    public class CityService : ICityService
    {
        private readonly ArtPixxelContext _context;
        public CityService(ArtPixxelContext context)
        {
            _context = context;
        }

        public async Task<CityCRUDResponse> Create(CityWriteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
              
                    if(await _context.States.AnyAsync(e => e.Id == @request.City.State))
                    {
                        City city = new()
                        {
                            CityName = @request.City.Name,
                            StateId = _context.States.Find(@request.City.State).Id,
                            DeliveryFee = @request.City.DeliveryFee,
                            Description = @request.City.Description
                        };

                          _context.Cities.Add(city);
                          int saveResult = await _context.SaveChangesAsync();
                    if (saveResult > 0)
                    {
                        return new CityCRUDResponse
                        {
                            CityInit = await Cities(@request.Filter),
                            Response = new BaseResponse
                            {
                                Message = "City created.",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful",
                            }

                        };


                    }

                        return new CityCRUDResponse
                        {
                            CityInit = await Cities(@request.Filter),
                            Response = new BaseResponse
                            {
                                Message = "City could not be created.",
                                Result = RequestResult.Error,
                                Succeeded = false,
                                Title = "Unknown Error"
                            }

                        };


                    }


                    return new CityCRUDResponse
                    {
                        CityInit = await Cities(@request.Filter),
                        Response = new BaseResponse
                        {
                            Message = "Reference to state could not be resolved. This state may have been deleted.",
                            Result = RequestResult.Error,
                            Succeeded = false,
                            Title = "Null State Reference",
                        }

                    };
                


               
            }
            catch (Exception ex)
            {

                return new CityCRUDResponse
                {
                    CityInit = await Cities(@request.Filter),
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
        public async Task<CityBaseInit> Init(Pagination pagination)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@pagination", pagination);
                return new CityBaseInit
                {
                    Cities = await TopMost(@pagination),
                    Countries = await _context.Countries.Include(s => s.Flag).Select(c => new CountryOption
                    {
                        Id = c.Id,
                        Flag  = c.Flag.Name,
                        Name = c.Name

                    }).ToListAsync(),
                    States = await _context.States.Select(s => new StateOption
                    {
                        Id = s.Id,
                        Name = s.StateName,
                        Country = s.CountryId

                    }).ToListAsync(),
                    TotalCity = decimal.Round(await _context.Cities.CountAsync(), 0, MidpointRounding.AwayFromZero)

                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<CityInit> Cities(CityFilterData filter)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@filter", filter);

                bool emptyFilter = EmptyFilter(filter);
                ExpressionStarter<City> pred = ApplyFilter(@filter);

                List<City> query = emptyFilter ?
                   await _context.Cities
                   .OrderBy(s => s.CityName)
                   .Skip(@filter.Skip)
                   .Take(@filter.PageSize)
                   .Include(st => st.State).ThenInclude(ct => ct.Country).ThenInclude(f => f.Flag).ToListAsync()

                    :

                    await _context.Cities
                   .Where(pred)
                   .OrderBy(s => s.CityName)
                   .Skip(@filter.Skip)
                   .Take(@filter.PageSize)
                   .Include(st => st.State).ThenInclude(ct => ct.Country).ThenInclude(f => f.Flag).ToListAsync();


                List<CityModel> cities = query.Select(c => new CityModel
                {

                    Country = string.IsNullOrEmpty(c.StateId) ?
                        new CountryOption { Flag = string.Empty, Id = string.Empty, Name = string.Empty }
                        : new CountryOption { Flag = c.State.Country.Flag.Name, Id = c.State.Country.Id, Name = c.State.Country.Name },
                    State = string.IsNullOrEmpty(c.StateId) ?
                        new StateOption { Id = string.Empty, Name = string.Empty, Country = string.Empty } :
                        new StateOption { Id = c.State.Id, Name = c.State.StateName, Country = c.State.CountryId },
                    DeliveryFee = c.DeliveryFee,
                    Description = c.Description,
                    Id = c.Id,
                    Name = c.CityName
                }).ToList();



                cities = ApplySort(cities, filter);

                return new CityInit
                {
                    Cities = cities,
                    TotalCity = decimal.Round(emptyFilter ? await _context.Cities.CountAsync() : await _context.Cities.Where(pred).CountAsync(), 0, MidpointRounding.AwayFromZero)
                };

            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<List<CityModel>> TopMost(Pagination pagination)
        {
            try
            {
                return await _context.Cities.OrderBy(s => s.CityName)
                    .Skip(pagination.Skip)
                    .Take(pagination.PageSize)
                    .Include(s => s.State).ThenInclude(ct =>ct.Country).ThenInclude(f => f.Flag)
                    .Select(c => new CityModel
                    {
                        
                        Country = string.IsNullOrEmpty(c.StateId) ? 
                        new CountryOption { Flag = string.Empty, Id = string.Empty, Name = string.Empty }
                        : new CountryOption { Flag = c.State.Country.Flag.Name, Id = c.State.Country.Id, Name = c.State.Country.Name },
                        State = string.IsNullOrEmpty(c.StateId) ? 
                        new StateOption { Id = string.Empty, Name = string.Empty, Country = string.Empty } :
                        new StateOption { Id = c.State.Id, Name = c.State.StateName, Country = c.State.CountryId},
                        DeliveryFee = c.DeliveryFee,
                        Description = c.Description,
                        Id = c.Id,
                        Name = c.CityName
                    }).ToListAsync();
            }
            catch (Exception)
            {

                return new List<CityModel>();
            }
        }

        public async Task<CityCRUDResponse> Update(CityWriteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if(await _context.Cities.AnyAsync(e => e.Id == @request.City.Id))
                {
                    City city = await _context.Cities.FindAsync(@request.City.Id);
                    if(city != null)
                    {
                        string oldName = city.CityName;
                        city.CityName = @request.City.Name;
                        city.DeliveryFee = @request.City.DeliveryFee;
                        city.StateId = _context.States.Find(@request.City.State).Id;
                        city.Description = @request.City.Description;

                        _context.Cities.Update(city);
                        int upResult = await _context.SaveChangesAsync();

                        if(upResult > 0)
                        {
                            return new CityCRUDResponse
                            {
                                CityInit = await Cities(@request.Filter),
                                Response = new BaseResponse
                                {
                                    Title = "Successful",
                                    Message = oldName + " updated.",
                                    Result = RequestResult.Success,
                                    Succeeded = true,

                                }

                            };
                        }

                        return new CityCRUDResponse
                        {
                            CityInit = await Cities(@request.Filter),
                            Response = new BaseResponse
                            {
                                Message = "City could not be updated.",
                                Result = RequestResult.Error,
                                Succeeded = false,
                                Title = "Unknown Error"
                            }

                        };
                    }

                }

                return new CityCRUDResponse
                {
                    CityInit = await Cities(@request.Filter),
                    Response = new BaseResponse
                    {
                        Title = "Null City Reference",
                        Message = "Reference to city could not be resolved. This city may have been deleted.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                       
                    }

                };
            }
            catch (Exception ex)
            {
                return new CityCRUDResponse
                {
                    CityInit = await Cities(request.Filter),
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

        public async Task<BaseBoolResponse> Exists(FullCity request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.Cities.AnyAsync(e => e.CityName == @request.Name && (e.StateId == @request.StateId)))
                {
                    return new BaseBoolResponse
                    {
                        Exist = true,
                        Message = "City: <i>" + @request.Name + " </i> already exists in state."
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

        public async Task<BaseBoolResponse> Duplicate(FullCity request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.Cities.AnyAsync(e => e.CityName == @request.Name && (e.Id != @request.Id) && (e.StateId == @request.StateId)))
                {
                    return new BaseBoolResponse
                    {
                        Exist = true,
                        Message = "State: <i>" + @request.Name + " </i> already exists in state."
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

        public async Task<CityCRUDResponse> Delete(CityDeleteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if(await _context.Cities.AnyAsync(e => e.Id == @request.Id))
                {

                    City city = await _context.Cities.FindAsync(@request.Id);
                    if(city != null)
                    {
                        _context.Cities.Remove(city);
                        await _context.SaveChangesAsync();

                        return new CityCRUDResponse
                        {
                            CityInit = await Cities(@request.Filter),
                            Response = new BaseResponse
                            {
                                Message = "Successful",
                                Result = RequestResult.Success,
                                Succeeded = false,
                                Title = city.CityName+ " removed."
                            }
                        };
                    }
                }


                return new CityCRUDResponse
                {
                    CityInit = await Cities(@request.Filter),
                    Response = new BaseResponse
                    {
                        Message = "Null City Reference",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Reference to city could not be resolved. This city may have been deleted."
                    }

                };
            }
            catch (Exception ex)
            {

                return new CityCRUDResponse
                {
                    CityInit = await Cities(@request.Filter),
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

        public async Task<CityCRUDResponse> BatchCreate(MultiCityWriteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (request.Requests.Any())
                {
                    List<City> cities = request.Requests.Select(c => new City
                    {
                        CityName = c.Name,
                        DeliveryFee = c.DeliveryFee,
                        Description = c.Description,
                        StateId = _context.States.Find(c.State).Id,
                    }).ToList();

                    if (cities.Any())
                    {
                        _context.Cities.AddRange(cities);
                        int svResult = await _context.SaveChangesAsync();

                        if(svResult > 0)
                        {
                            return new CityCRUDResponse
                            {
                                CityInit = await Cities(@request.Filter),
                                Response = new BaseResponse
                                {
                                    Message = cities.Count == 1 ? "City created." : "Cities created.",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }

                            };

                        }


                        return new CityCRUDResponse
                        {
                            CityInit = await Cities(@request.Filter),
                            Response = new BaseResponse
                            {
                                Message = "Cities could not be created. Please try again later",
                                Result = RequestResult.Error,
                                Succeeded = false,
                                Title = "Unknown Error"
                            }

                        };
                    }

                    
                }

                return new CityCRUDResponse
                {
                    CityInit = await Cities(@request.Filter),
                    Response = new BaseResponse
                    {
                        Message = "Cities could not be created from an empty list.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Empty Request"
                    }

                };

            }
            catch (Exception ex)
            {

                return new CityCRUDResponse
                {
                    CityInit = await Cities(@request.Filter),
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

        public async Task<CityCRUDResponse> BatchDelete(BatchCityDeleteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                if (request.Ids.Any())
                {
                    List<City> cities = await _context.Cities.Where(e => @request.Ids.Contains(e.Id)).ToListAsync();
                    if (cities.Any())
                    {
                        _context.Cities.RemoveRange(cities);
                        await _context.SaveChangesAsync();

                        return new CityCRUDResponse
                        {
                            CityInit = await Cities(@request.Filter),
                            Response = new BaseResponse
                            {
                                Message = cities.Count == 1 ? "City deletd." : "Cities deletd.",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "SUccessful"
                            }
                        };
                    }
                }

                return new CityCRUDResponse
                {
                    CityInit = await Cities(@request.Filter),
                    Response = new BaseResponse
                    {
                        Message = "Could not delete empty cities",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Empty Request"
                    }

                };

            }
            catch (Exception ex)
            {

                return new CityCRUDResponse
                {
                    CityInit = await Cities(@request.Filter),
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

        public async Task<CityCRUDResponse> BatchUpdate(BatchCityWriteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                List<City> cities = await _context.Cities.Where(e => @request.Request.BatchIds.Contains(e.Id)).ToListAsync();
                if (cities.Any())
                {
                    State state = new();
                    if (!string.IsNullOrEmpty(@request.Request.BatchState))
                    {
                        state = await _context.States.FindAsync(@request.Request.BatchState);
                        if(state == null)
                        {
                            return new CityCRUDResponse
                            {
                                CityInit = await Cities(@request.Filter),
                                Response = new BaseResponse
                                {
                                    Message = "Reference to state could not be resolved. This state may have been deleted.",
                                    Result = RequestResult.Error,
                                    Succeeded = false,
                                    Title = "Null State Reference"
                                }

                            };
                        }
                    }
                    foreach (City city in cities)
                    {
                        if (!string.IsNullOrEmpty(@request.Request.BatchState))
                        {
                            city.StateId = state.Id;
                        }
                        if (@request.Request.BatchDeliveryFee != -1m)
                        {
                            city.DeliveryFee = @request.Request.BatchDeliveryFee;
                        }

                        if (!string.IsNullOrEmpty(@request.Request.BatchDescription))
                        {
                            city.Description = @request.Request.BatchDescription;
                        }
                    }


                    _context.Cities.UpdateRange(cities);
                    int upateResult = await _context.SaveChangesAsync();
                    if(upateResult > 0)
                    {

                        return new CityCRUDResponse
                        {
                            CityInit = await Cities(@request.Filter),
                            Response = new BaseResponse
                            {
                                Message = upateResult == 1 ? "City updated.": "Cities updated.",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            }

                        };

                    }



                    return new CityCRUDResponse
                    {
                        CityInit = await Cities(@request.Filter),
                        Response = new BaseResponse
                        {
                            Message = "Cities could not be updated. Please try again later",
                            Result = RequestResult.Error,
                            Succeeded = false,
                            Title = "Unknown Error"
                        }

                    };




                }

                return new CityCRUDResponse
                {
                    CityInit = await Cities(@request.Filter),
                    Response = new BaseResponse
                    {
                        Message = "Empty city list could not be updated.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Invalid Request"
                    }

                };

            }
            catch (Exception ex)
            {


                return new CityCRUDResponse
                {
                    CityInit = await Cities(@request.Filter),
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
