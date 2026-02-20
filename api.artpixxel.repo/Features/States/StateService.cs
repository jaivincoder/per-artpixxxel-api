
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.States;
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
using static api.artpixxel.repo.Utils.States.StateFilterUtils;

namespace api.artpixxel.repo.Features.States
{
   public class StateService : IStateService
    {
        private readonly ArtPixxelContext _context;
        public StateService(ArtPixxelContext context)
        {
            _context = context;
        }

        public async Task<StateCRUDResponse> BatchDelete(BatchStateDeleteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                if (@request.Ids.Any())
                {
                    List<State> states = await _context.States.Where(e => @request.Ids.Contains(e.Id)).Include(c => c.Cities).ToListAsync();
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


                        return new StateCRUDResponse
                        {
                            StateInit = await States(@request.Filter),
                            Response = new BaseResponse
                            {
                                Title = "Successful",
                                Message = states.Count == 1 ? "State removed" : "States removed",
                                Result = RequestResult.Success,
                                Succeeded = true
                            }
                        };
                    }
                }

                return new StateCRUDResponse
                {
                    StateInit = await States(@request.Filter),
                    Response = new BaseResponse
                    {
                        Title = "Empty Request",
                        Message = "Couldn't delete empty states. Request terminated.",
                        Result = RequestResult.Error,
                        Succeeded = false
                    }
                };

            }
            catch (Exception ex)
            {

                return new StateCRUDResponse
                {
                    StateInit = await States(@request.Filter),
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

        public async Task<StateCRUDResponse> BatchUpdate(BatchStateWriteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (request.Request.BatchIds.Any())
                {
                    List<State> states = await _context.States.Where(e => @request.Request.BatchIds.Contains(e.Id)).ToListAsync();

                    if (states.Any())
                    {
                        Country country = new();
                        if (!string.IsNullOrEmpty(@request.Request.BatchCountry))
                        {
                            country = await _context.Countries.FindAsync(@request.Request.BatchCountry);
                            if(country == null)
                            {
                                return new StateCRUDResponse
                                {
                                    StateInit = await States(@request.Filter),
                                    Response = new BaseResponse
                                    {
                                        Title = "Null Country Reference",
                                        Message ="Reference to country could not be resolved. This country may have been deleted.",
                                        Result = RequestResult.Success,
                                        Succeeded = true
                                    }
                                };
                            }
                        }

                      
                            foreach (State state in states)
                        {
                            if(@request.Request.BatchDeliveryFee != -1m)
                            {
                                state.DeliveryFee = @request.Request.BatchDeliveryFee;
                            }
                            if (!string.IsNullOrEmpty(@request.Request.BatchCountry))
                            {
                                state.CountryId = country.Id;
                            }

                            if (!string.IsNullOrEmpty(@request.Request.BatchDescription))
                            {
                                state.Description = @request.Request.BatchDescription;
                            }
                        }


                        _context.States.UpdateRange(states);
                       int updateResult = await _context.SaveChangesAsync();

                        if(updateResult > 0)
                        {
                            return new StateCRUDResponse
                            {
                                StateInit = await States(@request.Filter),
                                Response = new BaseResponse
                                {
                                    Title = "Successful",
                                    Message = updateResult == 1 ? "State updated." : "States updated",
                                    Result = RequestResult.Success,
                                    Succeeded = true
                                }
                            };
                        }



                        return new StateCRUDResponse
                        {
                            StateInit = await States(@request.Filter),
                            Response = new BaseResponse
                            {
                                Title = "Unknown Error",
                                Message = "Batch update was not successful. Please try again later.",
                                Result = RequestResult.Error,
                                Succeeded = false
                            }
                        };


                    }
                }

                return new StateCRUDResponse
                {
                    StateInit = await States(@request.Filter),
                    Response = new BaseResponse
                    {
                        Title = "Empty Request",
                        Message = "Couldn't update null states. Request terminated.",
                        Result = RequestResult.Error,
                        Succeeded = false
                    }
                };
            }
            catch (Exception ex)
            {

                return new StateCRUDResponse
                {
                    StateInit = await States(@request.Filter),
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

        public async Task<StateCRUDResponse> BulkCreate(MultiStateWriteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (request.Requests.Any())
                {
                    List<State> states = new();
                    foreach(StateRequest state in request.Requests)
                    {
                        State _state = new()
                        {
                            CountryId = _context.Countries.Find(state.Country).Id,
                            StateName = state.Name,
                            DeliveryFee = state.DeliveryFee,
                            Description = state.Description

                        };
                        states.Add(_state);
                    }


                    if (states.Any())
                    {
                        _context.States.AddRange(states);
                        int svResult = await _context.SaveChangesAsync();

                        if(svResult > 0)
                        {
                            return new StateCRUDResponse
                            {
                                StateInit = await States(@request.Filter),
                                Response = new BaseResponse
                                {
                                    Title = "Successful",
                                    Message = states.Count == 1 ? states.First().StateName+ " created State " : "states created.",
                                    Result = RequestResult.Success,
                                    Succeeded = true
                                }
                            };
                        }



                        return new StateCRUDResponse
                        {
                            StateInit = await States(@request.Filter),
                            Response = new BaseResponse
                            {
                                Title = "Unknown Error",
                                Message = "States couldn't be created. Please try again later",
                                Result = RequestResult.Success,
                                Succeeded = false
                            }
                        };



                    }
                }


                return new StateCRUDResponse
                {
                    StateInit = await States(@request.Filter),
                    Response = new BaseResponse
                    {
                        Title = "Empty Request",
                        Message = "Couldn't create states from an empty list. Request terminated.",
                        Result = RequestResult.Error,
                        Succeeded = false
                    }
                };
            }
            catch (Exception ex)
            {
                return new StateCRUDResponse
                {
                    StateInit = await States(@request.Filter),
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

        public async Task<StateCRUDResponse> Create(StateWriteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if(await _context.Countries.AnyAsync(e=> e.Id == request.Request.Country))
                {
                    State state = new()
                    {
                        CountryId = _context.Countries.Find(request.Request.Country).Id,
                        StateName = request.Request.Name,
                        DeliveryFee = request.Request.DeliveryFee,
                        Description = request.Request.Description

                    };

                    _context.States.Add(state);
                    int sveResult = await _context.SaveChangesAsync();

                    if(sveResult > 0)
                    {
                        return new StateCRUDResponse
                        {
                            StateInit = await States(@request.Filter),
                            Response = new BaseResponse
                            {
                                Title = "Successful",
                                Message = request.Request.Name+" state created.",
                                Result = RequestResult.Success,
                                Succeeded = true
                            }
                        };
                    }

                    return new StateCRUDResponse
                    {
                        StateInit = await States(@request.Filter),
                        Response = new BaseResponse
                        {
                            Title = "Unknown Error",
                            Message = request.Request.Name + " state couldn't be created. Please try again later",
                            Result = RequestResult.Success,
                            Succeeded = false
                        }
                    };

                }


                return new StateCRUDResponse
                {
                    StateInit = await States(@request.Filter),
                    Response = new BaseResponse
                    {
                        Title = "Null Country Reference",
                        Message = "Reference to country could not be resolved. This country may have been deleted.",
                        Result = RequestResult.Error,
                        Succeeded = false
                    }
                };
            }
            catch (Exception ex)
            {

                return new StateCRUDResponse
                {
                    StateInit = await States(@request.Filter),
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

        public async Task<StateCRUDResponse> Delete(StateDeleteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                if(await _context.States.AnyAsync(e => e.Id == request.Id))
                {
                    State state = await _context.States.FindAsync(@request.Id);
                    if(state != null)
                    {
                        _context.States.Remove(state);
                        await _context.SaveChangesAsync();

                        return new StateCRUDResponse
                        {
                            StateInit = await States(@request.Filter),
                            Response = new BaseResponse
                            {
                                Title = "Successful",
                                Message = state.StateName+" state successfully removed.",
                                Result = RequestResult.Success,
                                Succeeded = true
                            }
                        };
                    }
                }

                return new StateCRUDResponse
                {
                    StateInit = await States(@request.Filter),
                    Response = new BaseResponse
                    {
                        Title = "Null State Reference",
                        Message = "Reference to state could not be resolved. This state may have been deleted.",
                        Result = RequestResult.Error,
                        Succeeded = false
                    }
                };


            }
            catch (Exception ex)
            {

                return new StateCRUDResponse
                {
                    StateInit = await States(@request.Filter),
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

        public async Task<BaseBoolResponse> Duplicate(FullState request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.States.AnyAsync(e => e.StateName == @request.Name && (e.Id != @request.Id) && (e.CountryId == @request.CountryId )))
                {
                    return new BaseBoolResponse
                    {
                        Exist = true,
                        Message = "State: <i>" + @request.Name + " </i> already exists."
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

        public async Task<BaseBoolResponse> Exists(BaseOption request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.States.AnyAsync(e => e.StateName == @request.Name && (e.CountryId == @request.Id)))
                {
                    return new BaseBoolResponse
                    {
                        Exist = true,
                        Message = "State: <i>" + @request.Name + " </i> already exists."
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

        public async Task<StateBaseInit> Init(Pagination pagination)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@pagination", pagination);

                return new StateBaseInit
                {
                    States = await TopMost(@pagination),

                    Options = await _context.Countries.Include(f => f.Flag)
                    .Select(c => new CountryOption 
                    {  
                        Id = c.Id,
                        Flag = c.Flag.Name,
                        Name = c.Name
                    }).ToListAsync(),
                    TotalState = decimal.Round(await _context.States.CountAsync(), 0, MidpointRounding.AwayFromZero)
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<StateInit> States(StateFilterData filter)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@filter", filter);

                bool emptyFilter = EmptyFilter(filter);
                ExpressionStarter<State> pred = ApplyFilter(@filter);

                List<State> query = emptyFilter ?

                    await _context.States
                    .OrderBy(s => s.StateName)
                    .Skip(@filter.Skip)
                    .Take(@filter.PageSize)
                    .Include(ct => ct.Country).ThenInclude(f => f.Flag)
                    .Include(c => c.Cities)
                    .ToListAsync()

                       :

                   await _context.States
                    .Where(pred)
                    .OrderBy(s => s.StateName)
                    .Skip(@filter.Skip)
                    .Take(@filter.PageSize)
                    .Include(ct => ct.Country).ThenInclude(f => f.Flag)
                    .Include(c => c.Cities)
                    .ToListAsync()
                       ;

                List<StateModel> states = query.Select(s => new StateModel
                {
                    CityCount = decimal.Round(s.Cities.Count(), 0, MidpointRounding.AwayFromZero),
                    Country = new CountryOption { Flag = s.Country.Flag.Name, Id = s.Country.Id, Name = s.Country.Name },
                    DeliveryFee = s.DeliveryFee,
                    Description = s.Description,
                    Id = s.Id,
                    Name = s.StateName
                }).ToList();

                states = ApplySort(states, filter);

                return new StateInit
                {
                    States = states,
                    TotalState = decimal.Round(emptyFilter ? await _context.States.CountAsync() : await _context.States.Where(pred).CountAsync(), 0, MidpointRounding.AwayFromZero)
                };


            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<StateCRUDResponse> Update(StateWriteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.Countries.AnyAsync(e => e.Id == @request.Request.Country))
                {
                    if (await _context.States.AnyAsync(s => s.Id == @request.Request.Id))
                    {
                        State state = await _context.States.FindAsync(@request.Request.Id);
                        if(state != null)
                        {
                            string oldName = state.StateName;
                            state.StateName = @request.Request.Name;
                            state.CountryId = _context.Countries.Find(@request.Request.Country).Id;
                            state.DeliveryFee = @request.Request.DeliveryFee;
                            state.Description = request.Request.Description;



                            _context.States.Update(state);
                            int updResult = await _context.SaveChangesAsync();

                            if(updResult > 0)
                            {
                                return new StateCRUDResponse
                                {
                                    StateInit = await States(@request.Filter),
                                    Response = new BaseResponse
                                    {
                                        Title = "Successful",
                                        Message = oldName + " state updated.",
                                        Result = RequestResult.Success,
                                        Succeeded = true
                                    }
                                };
                            }


                            return new StateCRUDResponse
                            {
                                StateInit = await States(@request.Filter),
                                Response = new BaseResponse
                                {
                                    Title = "Unknown Error",
                                    Message =  request.Request.Name + " state couldn't be updated. Please try again later",
                                    Result = RequestResult.Success,
                                    Succeeded = false
                                }
                            };
                        }

                    }
                }


                return new StateCRUDResponse
                {
                    StateInit = await States(@request.Filter),
                    Response = new BaseResponse
                    {
                        Title = "Null Country Reference",
                        Message = "Reference to country could not be resolved. This country may have been deleted.",
                        Result = RequestResult.Error,
                        Succeeded = false
                    }
                };

            }
            catch (Exception ex)
            {

                return new StateCRUDResponse
                {
                    StateInit = await States(@request.Filter),
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

        private async Task<List<StateModel>> TopMost(Pagination pagination)
        {
            try
            {
                return await _context.States
                    .OrderBy(s => s.StateName)
                    .Skip(@pagination.Skip)
                    .Take(pagination.PageSize)
                    .Include(ct => ct.Country).ThenInclude(f => f.Flag)
                    .Include(c => c.Cities)
                    .Select(s => new StateModel
                    {
                        CityCount = decimal.Round(s.Cities.Count(), 0, MidpointRounding.AwayFromZero),
                        Country = new CountryOption { Flag = s.Country.Flag.Name, Id = s.Country.Id, Name = s.Country.Name },
                        DeliveryFee = s.DeliveryFee,
                        Description = s.Description,
                        Id = s.Id,
                        Name = s.StateName
                    }).ToListAsync();
            }
            catch (Exception)
            {

                return new List<StateModel>();
            }
        }


      
    }
}
