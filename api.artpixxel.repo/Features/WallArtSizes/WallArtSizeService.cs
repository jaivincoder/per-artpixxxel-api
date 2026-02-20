

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.WallArtSizes;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using api.artpixxel.data.Models;

namespace api.artpixxel.repo.Features.WallArtSizes
{

   public class WallArtSizeService : IWallArtSizeService
    {
        private readonly ArtPixxelContext _context;
        public WallArtSizeService(ArtPixxelContext context)
        {
            _context = context;
        }

        public async Task<WallArtSizeCRUDResponse> BatchCreate(List<WallArtSizeBase> request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                if (request.Any())
                {
                    List<WallArtSize> wallArtSizes = @request.Select(sh => new WallArtSize
                    {
                        Name = sh.WallArtSizeName.Trim(),
                        Amount = sh.WallArtSizeAmount,
                        IsDefault = sh.IsWallArtSizeDefault,
                        Description = sh.WallArtSizeDescription


                    }).ToList();


                     _context.WallArtSizes.AddRange(wallArtSizes);
                    int saveResult = await _context.SaveChangesAsync();

                    if(saveResult > 0)
                    {
                        return new WallArtSizeCRUDResponse
                        {
                            WallArtSizes = await WallArtSizes(),
                            Response = new BaseResponse
                            {
                                Message = "WallArt sizes created",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            }
                        };
                    }
                }


                return new WallArtSizeCRUDResponse
                {
                    WallArtSizes = await WallArtSizes(),
                    Response = new BaseResponse
                    {
                        Message = "Couldn't create wallart sizes from empty object list. Request terminated",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Empty Object List"
                    }
                };



            }
            catch (Exception ex)
            {

                return new WallArtSizeCRUDResponse
                {
                    WallArtSizes = await WallArtSizes(),
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

        public async Task<WallArtSizeCRUDResponse> Create(WallArtSizeBase request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if(!await _context.WallArtSizes.AnyAsync(e => e.Name == @request.WallArtSizeName))
                {

                    WallArtSize wallArtSize = new()
                    {
                        Name = @request.WallArtSizeName.Trim(),
                        Amount = @request.WallArtSizeAmount,
                        IsDefault = @request.IsWallArtSizeDefault,
                        Description = @request.WallArtSizeDescription
                    };

                     _context.Add(wallArtSize);
                    int saveResult = await _context.SaveChangesAsync();

                    if(saveResult > 0)
                    {
                        return new WallArtSizeCRUDResponse
                        {
                            WallArtSizes = await WallArtSizes(),
                            Response = new BaseResponse
                            {
                                Message = string.Format("WallArt size '{0}' created", @request.WallArtSizeName),
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Success"
                            }
                        };
                    }

                }



                return new WallArtSizeCRUDResponse
                {
                    WallArtSizes = await WallArtSizes(),
                    Response = new BaseResponse
                    {
                        Message = "WallArt size already exists",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Duplicate Entry"
                    }
                };

            }
            catch (Exception ex)
            {
              

                return new WallArtSizeCRUDResponse
                {
                    WallArtSizes = await WallArtSizes(),
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

        public async Task<WallArtSizeCRUDResponse> Delete(BaseId request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.WallArtSizes.AnyAsync(e => e.Id == @request.Id))
                {

                    WallArtSize wallArtSize = await _context.WallArtSizes.FindAsync(@request.Id);
                    if(wallArtSize != null)
                    {
                        
                        if(!await _context.WallArts.AnyAsync(e => e.WallArtSizeId == wallArtSize.Id))
                        {
                            _context.Remove(wallArtSize);
                            int deleteResult = await _context.SaveChangesAsync();

                            if (deleteResult > 0)
                            {
                                return new WallArtSizeCRUDResponse
                                {
                                    WallArtSizes = await WallArtSizes(),
                                    Response = new BaseResponse
                                    {
                                        Message = string.Format("WallArt size '{0}' deleted", wallArtSize.Name),
                                        Result = RequestResult.Success,
                                        Succeeded = true,
                                        Title = "Successful"
                                    }
                                };
                            }
                        }



                        return new WallArtSizeCRUDResponse
                        {
                            WallArtSizes = await WallArtSizes(),
                            Response = new BaseResponse
                            {
                                Message = string.Format("WallArt size '{0}' has maintained wall arts and could not be deleted", wallArtSize.Name),
                                Result = RequestResult.Error,
                                Succeeded = false,
                                Title = "Error"
                            }
                        };

                    }
                }


                return new WallArtSizeCRUDResponse
                {
                    WallArtSizes = await WallArtSizes(),
                    Response = new BaseResponse
                    {
                        Message = "Reference to wallart could not be resolved. This wallart may have been deleted.",
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

                return new WallArtSizeCRUDResponse
                {
                    WallArtSizes = await WallArtSizes(),
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

                if (await _context.WallArtSizes.AnyAsync(e => e.Name == @request.Name && e.Id != @request.Id))
                {
                    return new BaseBoolResponse
                    {
                        Exist = true,
                        Message = "Wall ArtSize: <i>" + @request.Name + " </i> already exists "
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

                if (await _context.WallArtSizes.AnyAsync(e => e.Name == @request.Name))
                {
                    return new BaseBoolResponse
                    {
                        Exist = true,
                        Message = "WallArt Size: <i>" + @request.Name + " </i> already exists "
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

        public async Task<BaseBoolResponse> HasDefault()
        {
            try
            {
                if(!await _context.WallArtSizes.AnyAsync(e => e.IsDefault == true))
                {
                    return new BaseBoolResponse
                    {
                        Exist = false,
                        Message = "No default wallart size defined"
                    };
                }

                return new BaseBoolResponse
                {
                    Exist =true,
                    Message = string.Empty
                };
            }
            catch (Exception ex)
            {

                return new BaseBoolResponse
                {
                    Exist = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<WallArtSizeCRUDResponse> Update(WallArtSizeBase request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.WallArtSizes.AnyAsync(e => e.Id == @request.WallArtSizeId)){

                   
                    WallArtSize wallArtSize = await _context.WallArtSizes.FindAsync(@request.WallArtSizeId);
                    if(wallArtSize != null)
                    {
                        string oldName = wallArtSize.Name;
                        wallArtSize.Name = @request.WallArtSizeName;
                        wallArtSize.Description = @request.WallArtSizeDescription;
                        wallArtSize.Amount = @request.WallArtSizeAmount;
                        wallArtSize.IsDefault = @request.IsWallArtSizeDefault;


                        _context.WallArtSizes.Update(wallArtSize);
                        int updateResult = await _context.SaveChangesAsync();

                        if(updateResult > 0)
                        {
                            return new WallArtSizeCRUDResponse
                            {
                                WallArtSizes = await WallArtSizes(),
                                Response = new BaseResponse
                                {
                                    Message = string.Format("WallArt Size '{0}' updated", oldName),
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }
                            };
                        }
                    }

                }


                return new WallArtSizeCRUDResponse
                {
                    WallArtSizes = await WallArtSizes(),
                    Response = new BaseResponse
                    {
                        Message = "Reference to wallart could not be resolved. This wallart may have been deleted.",
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

                return new WallArtSizeCRUDResponse
                {
                    WallArtSizes = await WallArtSizes(),
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

        public async Task<List<BaseOption>> WallArtSizeOptions()
        {
            try
            {
                return await _context.WallArtSizes.OrderBy(e => e.Name).Select(w => 
                new BaseOption
                {
                    Id = w.Id,
                    Name = w.Name
                })
                .ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<WallArtSizeResponse>> WallArtSizes()
        {
            try
            {
                return await _context.WallArtSizes.OrderBy(e => e.Name).Include(w => w.WallArts).Select(w => 
                new WallArtSizeResponse 
                { 
                  WallArtSizeId = w.Id,
                  WallArtSizeName = w.Name,
                  IsWallArtSizeDefault = w.IsDefault,
                  WallArtSizeAmount = decimal.Round(w.Amount, 2 ,MidpointRounding.AwayFromZero),
                  WallArtSizeWallArtCount = decimal.Round(w.WallArts.Count(), 0, MidpointRounding.AwayFromZero),
                  WallArtSizeDescription = w.Description
                
                }).ToListAsync();

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
