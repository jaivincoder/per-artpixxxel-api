

using api.artpixxel.data.Features.Countries;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using api.artpixxel.data.Features.Common;
using System.Collections.Generic;
using api.artpixxel.data.Models;
using System.Data.SqlClient;

namespace api.artpixxel.repo.Features.Countries
{
    public class CountryService : ICountryService
    {
        private readonly ArtPixxelContext _context;
        public CountryService(ArtPixxelContext context)
        {
            _context = context;
        }

        public async Task<CountryCRUDResponse> BulkCreate(MultiCountryWriteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (request.Countries.Any())
                {
                    List<Country> countries = new();

                    foreach(var country in request.Countries)
                    {
                        Country _country = new()
                        {
                            Name = country.Name,
                            FlagId = _context.Flags.Find(country.Flag).Id,
                            DeliveryFee = country.DeliveryFee,
                            Description = country.Description
                        };

                        countries.Add(_country);
                    }


                    if (countries.Any())
                    {
                        _context.Countries.AddRange(countries);
                        int saveResult = await _context.SaveChangesAsync();

                        if(saveResult > 0)
                        {
                            return new CountryCRUDResponse
                            {
                                Country = await BaseInit(),
                                Response = new BaseResponse
                                {
                                    Message = countries.Count == 1 ? "Country created." : "Country created.",
                                    Title = "Successful",
                                    Result = RequestResult.Success,
                                    Succeeded = true
                                }
                            };
                        }

                    }
                }


                return new CountryCRUDResponse
                {
                    Country = await BaseInit(),
                    Response = new BaseResponse
                    {
                        Message = "Countries couldn't be created from an empty request",
                        Title = "Empty Country List",
                        Result = RequestResult.Error,
                        Succeeded = false
                    }
                };

            }
            catch (Exception ex)
            {
                return new CountryCRUDResponse
                {
                    Country = await BaseInit(),
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Title = ex.Source,
                        Result = RequestResult.Error,
                        Succeeded = false
                    }
                };

            }
        }

        public async Task<CountryCRUDResponse> Create(CountryWriteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                Country country = new()
                {
                    Name = @request.Name,
                    FlagId = _context.Flags.Find(@request.Flag).Id,
                    DeliveryFee = @request.DeliveryFee,
                    Description = @request.Description
                };

                _context.Countries.Add(country);
                int saveResult = await _context.SaveChangesAsync();

                if(saveResult > 0)
                {
                    return new CountryCRUDResponse
                    {
                        Country = await BaseInit(),
                        Response = new BaseResponse
                        {
                            Message = "Country created.",
                            Title = "Successful",
                            Result = RequestResult.Success,
                            Succeeded = true
                        }
                    };
                }



                return new CountryCRUDResponse
                {
                    Country = await BaseInit(),
                    Response = new BaseResponse
                    {
                        Message = "Country could not be saved. Please try again later.",
                        Title = "Unknown Error Occurred",
                        Result = RequestResult.Error,
                        Succeeded = false
                    }
                };

            }
            catch (Exception ex)
            {

                return new CountryCRUDResponse
                {
                    Country = await BaseInit(),
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Title = ex.Source,
                        Result = RequestResult.Error,
                        Succeeded = false
                    }
                };
            }
        }

        public async  Task<CountryCRUDResponse> Delete(CountryDeleteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.Countries.AnyAsync(e => e.Id == @request.Id))
                {
                    Country country = await _context.Countries.FindAsync(@request.Id);
                    if(country != null)
                    {

                        if(await _context.States.AnyAsync(e => e.CountryId == country.Id))
                        {
                            List<State> states = await _context.States.Where(e => e.CountryId == country.Id).Include(c => c.Cities).ToListAsync();
                            if (states.Any())
                            {
                                List<City> cities = states.SelectMany(e => e.Cities).ToList();
                                if (cities.Any())
                                {
                                    _context.Cities.RemoveRange(cities);
                                    await _context.SaveChangesAsync();
                                }


                                _context.States.RemoveRange(states);
                                await _context.SaveChangesAsync();
                                
                            }
                        }
                        _context.Countries.Remove(country);
                        await _context.SaveChangesAsync();

                        return new CountryCRUDResponse
                        {
                            Country = await BaseInit(),
                            Response = new BaseResponse
                            {
                                Message = "Country removed.",
                                Title = "Successful",
                                Result = RequestResult.Success,
                                Succeeded = true
                            }
                        };
                    }
                }


                return new CountryCRUDResponse
                {
                    Country = await BaseInit(),
                    Response = new BaseResponse
                    {
                        Message = "Reference to country couldn't be resolved. This country may have been deleted.",
                        Title = "Null Country Reference",
                        Result = RequestResult.Error,
                        Succeeded = false
                    }
                };
            }
            catch (Exception ex)
            {

                return new CountryCRUDResponse
                {
                    Country = await BaseInit(),
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Title = ex.Source,
                        Result = RequestResult.Error,
                        Succeeded = false
                    }
                };
            }
        }

        public async Task<CountryInit> Init()
        {
            try
            {
               

                CountryBaseInit baseInit = await BaseInit();

                return new CountryInit
                {
                    Countries = baseInit.Countries,

                    Options = baseInit.Options,
                    Flags = await _context.Flags.Select(f => new FlagOption
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Country = f.CountryName,

                    }).ToListAsync()
                };
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<BaseBoolResponse> Duplicate(BaseOption request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.Countries.AnyAsync(e => e.Name == @request.Name && e.Id != @request.Id))
                {
                    return new BaseBoolResponse
                    {
                        Exist = true,
                        Message = "Country: <i>" + @request.Name + " </i> already exists."
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

                if (await _context.Countries.AnyAsync(e => e.Name == @request.Name))
                {
                    return new BaseBoolResponse
                    {
                        Exist = true,
                        Message = "Country: <i>" + @request.Name + " </i> already exists."
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

        public async Task<CountryCRUDResponse> Update(CountryWriteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if(await _context.Countries.AnyAsync(e => e.Id == @request.Id))
                {
                    Country country = await _context.Countries.FindAsync(request.Id);
                    if(country != null)
                    {
                        country.Name = @request.Name;
                        country.FlagId = _context.Flags.Find(@request.Flag).Id;
                        country.DeliveryFee = @request.DeliveryFee;
                        country.Description = @request.Description;

                        _context.Countries.Update(country);
                        int updateResult = await _context.SaveChangesAsync();
                        if(updateResult > 0)
                        {
                            return new CountryCRUDResponse
                            {
                                Country = await BaseInit(),
                                Response = new BaseResponse
                                {
                                    Message = "Update succeded.",
                                    Title = "Successful",
                                    Result = RequestResult.Success,
                                    Succeeded = true
                                }
                            };
                        }
                    }
                }


                return new CountryCRUDResponse
                {
                    Country = await BaseInit(),
                    Response = new BaseResponse
                    {
                        Message = "Reference to country couldn't be resolved. This country may have been deleted.",
                        Title = "Null Country Reference",
                        Result = RequestResult.Error,
                        Succeeded = false
                    }
                };

            }
            catch (Exception ex)
            {

                return new CountryCRUDResponse
                {
                    Country = await BaseInit(),
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Title = ex.Source,
                        Result = RequestResult.Error,
                        Succeeded = false
                    }
                };
            }
            
        }

        private async Task<CountryBaseInit> BaseInit()
        {
            try
            {
                List<CountryResponse> countries = await _context.Countries.Include(f => f.Flag).Include(s => s.States).Select(c => new CountryResponse
                {
                    Id = c.Id,
                    DeliveryFee = decimal.Round(c.DeliveryFee, 2, MidpointRounding.AwayFromZero),
                    Description = c.Description,
                    Name = new FlagOption { Id = c.Id, Name = c.Flag.Name, Country = c.Name },
                    Flag = new BaseOption { Id = c.Flag.Id, Name = c.Flag.Name },
                    StateCount = c.States.Count()
                }).ToListAsync();

                return new CountryBaseInit
                {
                    Countries = countries,
                    Options =  countries.Any() ? countries.Select(e => e.Name).ToList() : new List<FlagOption>(),

                };
            }
            catch (Exception)
            {

                throw;
            }

        }

       
    }
}
