

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.LeadTimes;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Threading.Tasks;
using api.artpixxel.data.Models;

namespace api.artpixxel.repo.Features.LeadTimes
{
    
    public class LeadTimeService : ILeadTimeService
    {
        private readonly ArtPixxelContext _context;
        public LeadTimeService(ArtPixxelContext context)
        {
            _context = context;
        }
        public async Task<LeadTimeResponse> Set(LeadTimeModel request)
        {
            SqlParameter[] myparm = new SqlParameter[1];
            myparm[0] = new SqlParameter("@request", request);

            try
            {
                if (string.IsNullOrEmpty(@request.Id))
                {
                    LeadTime leadTime = new()
                    {
                        TimeLimit = (TimeLimit) Enum.Parse(typeof(TimeLimit), @request.TimeLimit),
                        UpperBandQuantifier = @request.UpperBandQuantifier,
                        LowerBandQuantifier = @request.LowerBandQuantifier,
                        Description = @request.Description
                    };

                    _context.LeadTimes.Add(leadTime);
                    int saveResult = await _context.SaveChangesAsync();
                    if(saveResult > 0)
                    {
                        return new LeadTimeResponse
                        {
                            LeadTime = new LeadTimeModel
                            {
                                Id = leadTime.Id,
                                Description = @request.Description,
                                UpperBandQuantifier = @request.UpperBandQuantifier,
                                LowerBandQuantifier = @request.LowerBandQuantifier,
                                TimeLimit =  @request.TimeLimit
                            },

                            Response = new BaseResponse
                            {
                                Message = "Lead time set.",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            }
                        };
                    }
                }


                else
                {
                    LeadTime leadTime = await _context.LeadTimes.FindAsync(@request.Id);
                    if(leadTime != null)
                    {
                        leadTime.Description = @request.Description;
                        leadTime.UpperBandQuantifier = @request.UpperBandQuantifier;
                        leadTime.LowerBandQuantifier = @request.LowerBandQuantifier;
                        leadTime.TimeLimit = (TimeLimit)Enum.Parse(typeof(TimeLimit), @request.TimeLimit);

                        _context.LeadTimes.Update(leadTime);
                        int updateResult = await _context.SaveChangesAsync();
                        if(updateResult > 0)
                        {
                            return new LeadTimeResponse
                            {
                                LeadTime = new LeadTimeModel
                                {
                                    Id = leadTime.Id,
                                    Description = @request.Description,
                                    UpperBandQuantifier = @request.UpperBandQuantifier,
                                    LowerBandQuantifier = @request.LowerBandQuantifier,
                                    TimeLimit = @request.TimeLimit
                                },

                                Response = new BaseResponse
                                {
                                    Message = "Lead time updated.",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }
                            };
                        }
                    }


                    return new LeadTimeResponse
                    {

                        LeadTime = null,
                        Response = new BaseResponse
                        {
                            Message = "Reference to leadtime could not be resolved. Leadtime may have been deleted",
                            Result = RequestResult.Error,
                            Succeeded = false,
                            Title = "Null Leadtime Reference"
                        }
                    };
                }


                return new LeadTimeResponse
                {
                    LeadTime = string.IsNullOrEmpty(@request.Id) ? null : await DefaultLeadTime(@request.Id),

                    Response = new BaseResponse
                    {
                        Message = "An unknown error occurred. Request terminated",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Unknown Error"
                    }
                };


            }
            catch (Exception ex)
            {

                return new LeadTimeResponse
                {
                    LeadTime = await DefaultLeadTime(@request.Id),

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



        private async Task<LeadTimeModel> DefaultLeadTime(string Id)
        {
            return await _context.LeadTimes.Where(e => e.Id == Id).Select(e => new LeadTimeModel
            {
                Id = e.Id,
                UpperBandQuantifier = e.UpperBandQuantifier,
                LowerBandQuantifier = e.LowerBandQuantifier,
                Description = e.Description,
                TimeLimit = e.TimeLimit.ToString()

            }).FirstOrDefaultAsync();
        }
    }
}
