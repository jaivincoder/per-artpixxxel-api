using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Sizes;
using api.artpixxel.data.Models;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace api.artpixxel.repo.Features.Sizes
{
    public class SizeService : ISizeService
    {
        private readonly ArtPixxelContext _context;
        public SizeService(ArtPixxelContext context)
        {
              _context = context;
        }

        public async Task<SizeCRUDResponse> BatchCreate(List<SizeModel> requests)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@requests", requests);
                

                if (@requests.Any())
                {

                    List<Size> savedDefaultSizes = new List<Size>();

                    var namesSizeTypes = requests.Select(e => new 
                    { 
                        Type = (SizeType) Enum.Parse(typeof(SizeType), e.SizeType),
                        Name = e.SizeName
                    }).ToList();
                    if (namesSizeTypes.Any())
                    {

                        List<string> names = namesSizeTypes.Select(e => e.Name).ToList();
                        List<SizeType> sizeTypes = namesSizeTypes.Select(e => e.Type).Distinct().ToList();
                        savedDefaultSizes = await _context.Sizes.Where(e => e.Default == true && sizeTypes.Contains(e.Type)).ToListAsync();


                        if (await _context.Sizes.AnyAsync(e =>  names.Contains(e.Name) && (sizeTypes.Contains(e.Type))))
                        {
                            Size duplicate = await _context.Sizes.Where(e => names.Contains(e.Name) && (sizeTypes.Contains(e.Type))).FirstOrDefaultAsync();
                            if(duplicate != null)
                            {
                                return new SizeCRUDResponse
                                {
                                    Response = new BaseResponse
                                    {
                                        Message = string.Format("Size {0} with size type {1} already exists", duplicate.Name, duplicate.Type),
                                        Succeeded = false,
                                        Result = RequestResult.Error,
                                        Title = "Duplicate Entry"
                                    },
                                    Sizes = await Sizes()
                                };

                            }
                        }


                        List<Size> sizes = requests.Select(e => new Size
                        {
                            Name = e.SizeName, 
                            Amount = e.SizeAmount,
                            Default = e.SizeDefault,
                            Description = e.SizeDescription,
                            Type = (SizeType)Enum.Parse(typeof(SizeType), e.SizeType),
                            Height = e.SizeHeight,
                            Width = e.SizeWidth
                        }).ToList();


                        if (sizes.Any())
                        {
                            if(sizes.Any(e => e.Default == true))
                            {


                                List<Size> uniqueDefaultSizes = sizes.Where(e => e.Default == true)
                                    .GroupBy(e => e.Type).Select(e => e.First())
                                    .ToList();

                                List<Size> otherDefaultSizes = sizes.Where(e => e.Default == true && (!uniqueDefaultSizes.Any(a => a.Name == e.Name))).ToList();



                                if (uniqueDefaultSizes.Any()) 
                                {
                                    List<SizeType> defaulTypes = uniqueDefaultSizes.Select(e => e.Type).ToList();
                                    if (defaulTypes.Any())
                                    {




                                        List<Size> currentDefaultSizes = savedDefaultSizes.Where(e => defaulTypes.Contains(e.Type)).ToList();
                                        if(currentDefaultSizes.Any())
                                        {
                                            foreach(Size size in currentDefaultSizes)
                                            {
                                                size.Default = false;


                                            }




                                            _context.Sizes.UpdateRange(currentDefaultSizes);
                                            await _context.SaveChangesAsync();
                                        }
                                    }




                                    if (otherDefaultSizes.Any())
                                    {
                                        foreach (Size otherDefaultSize in otherDefaultSizes)
                                        {
                                            otherDefaultSize.Default = false;
                                        }
                                    }

                                    



                                    if(uniqueDefaultSizes.Any(e => e.Type == SizeType.Regular))
                                    {
                                        Size defSizeRegular = sizes.Where(e => e.Default == true && (e.Type == SizeType.Regular)).FirstOrDefault();
                                        if(defSizeRegular != null)
                                        {
                                            List<MixnMatch> mixnMatches = await _context.MixnMatches.ToListAsync();
                                            if (mixnMatches.Any())
                                            {
                                              

                                                foreach (MixnMatch mixnMatch in mixnMatches)
                                                {
                                                    mixnMatch.Price = defSizeRegular.Amount;
                                                }
                                                _context.MixnMatches.UpdateRange(mixnMatches);
                                                await _context.SaveChangesAsync();
                                            }

                                        }

                                    }

                                
                                }




                               
                                //    if (await _context.Sizes.AnyAsync(e => e.Default == true && (e.Type ==  SizeType.Regular)))
                                //    {
                                //        Size defaultSize = await _context.Sizes.Where(e => e.Default == true && (e.Type == SizeType.Regular)).FirstOrDefaultAsync();
                                //        if (defaultSize != null)
                                //        {
                                //            defaultSize.Default = false;

                                //            _context.Sizes.Update(defaultSize);
                                //            await _context.SaveChangesAsync();
                                //        }
                                //    }


                                //if (await _context.Sizes.AnyAsync(e => e.Default == true && (e.Type == SizeType.FloralLiner)))
                                //{
                                //    Size defaultSizeFloraLiner = await _context.Sizes.Where(e => e.Default == true && (e.Type == SizeType.FloralLiner)).FirstOrDefaultAsync();
                                //    if (defaultSizeFloraLiner != null)
                                //    {
                                //        defaultSizeFloraLiner.Default = false;

                                //        _context.Sizes.Update(defaultSizeFloraLiner);
                                //        await _context.SaveChangesAsync();
                                //    }
                                //}


                                //Size defSizeRegular = sizes.Where(e => e.Default == true && (e.Type == SizeType.Regular)).FirstOrDefault();
                                //if(defSizeRegular != null)
                                //{
                                //    List<Size> _sizes = sizes.Where(e => e.Name != defSizeRegular.Name && (e.Default == true) && (e.Type == SizeType.Regular)).ToList();
                                //    if (_sizes.Any())
                                //    {
                                //        List<string> defregularnames = _sizes.Select(e => e.Name).ToList();
                                //        if (defregularnames.Any())
                                //        {
                                //            sizes.RemoveAll(e => defregularnames.Contains(e.Name));
                                //            _sizes.ForEach(c => c.Default = false);
                                //            sizes.AddRange(_sizes);

                                //        }
                                      
                                //    }


                                //    List<MixnMatch> mixnMatches = await _context.MixnMatches.ToListAsync();
                                //    if (mixnMatches.Any())
                                //    {
                                //        //mixnMatches.ForEach(c => c.Price = defSizeRegular.Amount);

                                //        foreach(MixnMatch mixnMatch in mixnMatches)
                                //        {
                                //            mixnMatch.Price = defSizeRegular.Amount;
                                //        }
                                //        _context.MixnMatches.UpdateRange(mixnMatches);
                                //        await _context.SaveChangesAsync();
                                //    }



                                //}


                                //Size defSizeFloralLiner = sizes.Where(e => e.Default == true && (e.Type == SizeType.FloralLiner)).FirstOrDefault();
                                //if(defSizeFloralLiner != null)
                                //{
                                //    List<Size> _sizes = sizes.Where(e => e.Name != defSizeFloralLiner.Name && (e.Default == true) && (e.Type == SizeType.FloralLiner)).ToList();
                                //    if (_sizes.Any())
                                //    {
                                //        List<string> defFloralLinernames = _sizes.Select(e => e.Name).ToList();
                                //        if (defFloralLinernames.Any())
                                //        {
                                //            sizes.RemoveAll(e => defFloralLinernames.Contains(e.Name));
                                //            _sizes.ForEach(c => c.Default = false);
                                //            sizes.AddRange(_sizes);

                                //        }

                                //    }
                                //}


                            }



                            if (sizeTypes.Any())
                            {
                                foreach(SizeType sizeType in sizeTypes)
                                {
                                    if((!savedDefaultSizes.Any(e => e.Type == sizeType)) && (!sizes.Any(e => e.Type == sizeType && e.Default == true)))
                                    {
                                        Size sizeDefault = sizes.Where(e => e.Type == sizeType).FirstOrDefault();
                                        if(sizeDefault != null)
                                        {
                                            sizeDefault.Default = true;
                                        }
                                    }
                                }
                            }

                           


                            _context.Sizes.AddRange(sizes);
                          int saveResult =  await _context.SaveChangesAsync();

                            if(saveResult > 0)
                            {
                                return new SizeCRUDResponse
                                {
                                    Response = new BaseResponse
                                    {
                                        Message = sizes.Count == 1 ? string.Format("Size {0} created", sizes.First().Name ) : "Sizes created",
                                        Succeeded = true,
                                        Result = RequestResult.Success,
                                        Title = "Success"
                                    },
                                    Sizes = await Sizes()
                                };
                            }




                        }




                        return new SizeCRUDResponse
                        {
                            Response = new BaseResponse
                            {
                                Message = "An error occurred. Size couldn't be created. Please try again later",
                                Succeeded = false,
                                Result = RequestResult.Error,
                                Title = "Unknown Error"
                            },
                            Sizes = await Sizes()
                        };



                    }
                }


                return new SizeCRUDResponse
                {
                    Response = new BaseResponse
                    {
                        Message = "Couldn't create sizes from an empty request.",
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Title = "Empty Request"
                    },
                    Sizes = await Sizes()
                };

            }
            catch (Exception ex)
            {

                return new SizeCRUDResponse
                {
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Title = ex.Source
                    },
                    Sizes = await Sizes()
                };
            }
        }

        public async Task<SizeCRUDResponse> Create(SizeModel request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

               
              if (Enum.TryParse(request.SizeType, out SizeType sizeType))
                {

                // borderStyle = (BorderStyle)Enum.Parse(typeof(BorderStyle), request.SizeBorderStyle);

                if (await _context.Sizes.AnyAsync(e => e.Name == @request.SizeName && (e.Type == sizeType)))
                {
                    return new SizeCRUDResponse
                    {
                        Response = new BaseResponse
                        {
                            Message = "Size already exists for size type '"+request.SizeType+"'.",
                            Succeeded = false,
                            Result = RequestResult.Error,
                            Title = "Duplicate Size"
                        }
                    };
                }



                if (request.SizeDefault == true)
                {
                    if (await _context.Sizes.AnyAsync(e => e.Default == true && (e.Type == sizeType)))
                    {
                        Size defaultSize = await _context.Sizes.Where(e => e.Default == true && (e.Type == sizeType)).FirstOrDefaultAsync();
                        if (defaultSize != null)
                        {
                            defaultSize.Default = false;

                            _context.Sizes.Update(defaultSize);
                            await _context.SaveChangesAsync();
                        }
                    }


                     if(sizeType == SizeType.Regular)
                     {
                            List<MixnMatch> mixnMatches = await _context.MixnMatches.ToListAsync();
                            if (mixnMatches.Any())
                            {
                                foreach(MixnMatch mixnmatch in mixnMatches) 
                                {
                                    mixnmatch.Price = @request.SizeAmount;
                                }
                              
                                _context.MixnMatches.UpdateRange(mixnMatches);
                                await _context.SaveChangesAsync();
                            }
                     }

                  

                }


                Size size = new()
                {
                    Name = @request.SizeName,
                    Amount = @request.SizeAmount,
                    Description = @request.SizeDescription,
                    Type = (SizeType)Enum.Parse(typeof(SizeType), request.SizeType),
                    Height = @request.SizeHeight,
                    Width = @request.SizeWidth,
                    Default = @request.SizeDefault
                };


                _context.Sizes.Add(size);
                int saveResult = await _context.SaveChangesAsync();

                if (saveResult > 0)
                {
                    return new SizeCRUDResponse
                    {
                        Response = new BaseResponse
                        {
                            Message = string.Format("Size {0} created", request.SizeName),
                            Succeeded = true,
                            Result = RequestResult.Success,
                            Title = "Success"
                        },
                        Sizes = await Sizes()
                    };
                }


                return new SizeCRUDResponse
                {
                    Response = new BaseResponse
                    {
                        Message = "An error occurred. Size couldn't be created. Please try again later.",
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Title = "Unknown Error"
                    },
                    Sizes = await Sizes()
                };
            }


                return new SizeCRUDResponse
                {
                    Response = new BaseResponse
                    {
                        Message = "The size type : '"+request.SizeType+"' not understood.",
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Title = "Invalid Border Style"
                    },
                    Sizes = await Sizes()
                };



            }
            catch (Exception ex)
            {

                return new SizeCRUDResponse
                {
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Title = ex.Source
                    },
                    Sizes = await Sizes()
                };
            }
        }

        public async Task<SizeCRUDResponse> Delete(BaseId request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                if (await _context.Sizes.AnyAsync(e => e.Id == @request.Id))
                {
                    Size size = await _context.Sizes.FindAsync(@request.Id);
                    if(size != null)
                    {
                        string name = size.Name;
                        _context.Sizes.Remove(size);
                        await _context.SaveChangesAsync();

                        return new SizeCRUDResponse
                        {
                            Response = new BaseResponse
                            {
                                Message =  string.Format("Size {0} removed", name),
                                Succeeded = true,
                                Result = RequestResult.Success,
                                Title = "Success"
                            },
                            Sizes = await Sizes()
                        };
                    }
                }


                return new SizeCRUDResponse
                {
                    Response = new BaseResponse
                    {
                        Message = "Reference to size couldn't be resolved. This size may have been deleted.",
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Title = "Null Size Reference"
                    },
                    Sizes = await Sizes()
                };

            }
            catch (Exception ex)
            {

                return new SizeCRUDResponse
                {
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Title = ex.Source
                    },
                    Sizes = await Sizes()
                };
            }
        }

        public async Task<BaseBoolResponse> Duplicate(DuplicateSize request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

               
                if (Enum.TryParse(request.SizeType,  out SizeType sizeType))
                {
                   // borderStyle = (SizeType)Enum.Parse(typeof(SizeType), @request.BorderStyle);
                    if (await _context.Sizes.AnyAsync(e => (e.Name == @request.Name && (e.Type == sizeType)) && (e.Id != @request.Id)))
                    {
                        return new BaseBoolResponse
                        {
                            Exist = true,
                            Message = "Size: <i>" + @request.Name + " </i> already exists for the size type: " + request.SizeType,
                        };
                    }


                    return new BaseBoolResponse
                    {
                        Exist = false,
                        Message = string.Empty
                    };
                }

                return new BaseBoolResponse
                {
                    Exist = true,
                    Message = "The size type '" + @request.SizeType + "' not understood."
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

        public async Task<BaseBoolResponse> Exists(DuplicateSize request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

               

                if (Enum.TryParse(request.SizeType, out  SizeType sizeType))
                {
                    

                    if (await _context.Sizes.AnyAsync(e => e.Name == @request.Name && (e.Type == sizeType)))
                    {
                        return new BaseBoolResponse
                        {
                            Exist = true,
                            Message = "Size: <i>" + @request.Name + " </i> already exists for the size type: " + request.SizeType
                        };
                    }


                    return new BaseBoolResponse
                    {
                        Exist = false,
                        Message = string.Empty
                    };
                }


                return new BaseBoolResponse
                {
                    Exist = true,
                    Message = "The size type '"+ @request.SizeType +"' not understood."
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

        public async Task<List<SizeModel>> Sizes()
        {
            try
            {
                if(await _context.Sizes.AnyAsync())
                {
                    return await _context.Sizes.Select(e => new SizeModel
                    {
                        SizeId = e.Id,
                        SizeAmount = decimal.Round(e.Amount, 2 ,MidpointRounding.AwayFromZero),
                        SizeDefault = e.Default,
                        SizeType = e.Type.ToString(),
                        SizeDescription = e.Description,
                        SizeHeight = e.Height,
                        SizeName = e.Name,
                        SizeWidth = e.Width
                    })
                    .OrderBy(e => e.SizeName)
                    .ToListAsync();
                }

                return new List<SizeModel>();
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<SizeCRUDResponse> Update(SizeModel request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                
                if (Enum.TryParse(request.SizeType, out  SizeType sizeType))
                {
                  

                    if (await _context.Sizes.AnyAsync(e => e.Id == request.SizeId))
                   {

                    Size size = await _context.Sizes.FindAsync(request.SizeId);
                    if (size != null)
                    {
                        if (@request.SizeDefault == true && (size.Default == false))
                        {
                            if (await _context.Sizes.AnyAsync(e => e.Default == true && (e.Type == sizeType)))
                            {
                                Size defaultSize = await _context.Sizes.Where(e => e.Default == true && (e.Type == sizeType)).FirstOrDefaultAsync();
                                if (defaultSize != null)
                                {
                                    defaultSize.Default = false;
                                    _context.Sizes.Update(defaultSize);
                                    await _context.SaveChangesAsync();

                                        if(sizeType == SizeType.Regular)
                                        {
                                            if (@request.SizeAmount != size.Amount)
                                            {
                                                List<MixnMatch> mixnMatches = await _context.MixnMatches.ToListAsync();
                                                if (mixnMatches.Any())
                                                {
                                                    mixnMatches.ForEach(c => c.Price = @request.SizeAmount);
                                                    _context.MixnMatches.UpdateRange(mixnMatches);
                                                    await _context.SaveChangesAsync();
                                                }
                                            }
                                        }

                                }
                            }
                        }

                        else if (@request.SizeDefault == false && (size.Default == true))
                        {
                            if (!await _context.Sizes.AnyAsync(e => e.Id != size.Id && (e.Default == true) && (e.Type == sizeType)))
                            {
                                @request.SizeDefault = true;

                                    if (sizeType == SizeType.Regular)
                                    {
                                        if (@request.SizeAmount != size.Amount)
                                        {
                                            List<MixnMatch> mixnMatches = await _context.MixnMatches.ToListAsync();
                                            if (mixnMatches.Any())
                                            {
                                                mixnMatches.ForEach(c => c.Price = @request.SizeAmount);
                                                _context.MixnMatches.UpdateRange(mixnMatches);
                                                await _context.SaveChangesAsync();
                                            }

                                        }
                                    }
                                      
                            }
                        }


                        else if (@request.SizeDefault == false && (size.Default == false))
                        {
                            if (!await _context.Sizes.AnyAsync(e => e.Default == true && (e.Type == sizeType)))
                            {
                                @request.SizeDefault = true;

                                    if (sizeType == SizeType.Regular)
                                    {
                                        if (@request.SizeAmount != size.Amount)
                                        {
                                            List<MixnMatch> mixnMatches = await _context.MixnMatches.ToListAsync();
                                            if (mixnMatches.Any())
                                            {
                                                mixnMatches.ForEach(c => c.Price = @request.SizeAmount);
                                                _context.MixnMatches.UpdateRange(mixnMatches);
                                                await _context.SaveChangesAsync();
                                            }

                                        }
                                    }

                                       
                            }
                        }


                        size.Name = request.SizeName;
                        size.Width = request.SizeWidth;
                        size.Height = request.SizeHeight;
                        size.Amount = request.SizeAmount;
                        size.Default = request.SizeDefault;
                        size.Type = (SizeType)Enum.Parse(typeof(SizeType), request.SizeType);
                        size.Description = request.SizeDescription;


                        _context.Sizes.Update(size);
                        int updateResult = await _context.SaveChangesAsync();

                        if (updateResult > 0)
                        {
                            return new SizeCRUDResponse
                            {
                                Response = new BaseResponse
                                {
                                    Message = string.Format("Size {0} updated", request.SizeName),
                                    Succeeded = true,
                                    Result = RequestResult.Success,
                                    Title = "Success"
                                },
                                Sizes = await Sizes()
                            };
                        }




                        return new SizeCRUDResponse
                        {
                            Response = new BaseResponse
                            {
                                Message = "An error occurred. Size couldn't be created. Please try again later.",
                                Succeeded = false,
                                Result = RequestResult.Error,
                                Title = "Unknown Error"
                            },
                            Sizes = await Sizes()
                        };



                    }
                }


                return new SizeCRUDResponse
                {
                    Response = new BaseResponse
                    {
                        Message = "Reference to size couldn't be resolved. This size may have been deleted.",
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Title = "Null Size Reference"
                    },
                    Sizes = await Sizes()
                };
               }

                return new SizeCRUDResponse
                {
                    Response = new BaseResponse
                    {
                        Message = "The size typ : " + request.SizeType + " not understood.",
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Title = "Invalid Size Type"
                    },
                    Sizes = await Sizes()
                };
            }
            catch (Exception ex)
            {

                return new SizeCRUDResponse
                {
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Title = ex.Source
                    },
                    Sizes = await Sizes()
                };
            }
        }
    }
}
