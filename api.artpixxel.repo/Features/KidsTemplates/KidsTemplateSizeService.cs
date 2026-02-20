

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.TemplatesSizes;
using api.artpixxel.data.Models;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace api.artpixxel.repo.Features.KidsTemplates
{
    public class KidsTemplateSizeService : IKidsTemplateSizeService
    {
        private readonly ArtPixxelContext _context;
        public KidsTemplateSizeService(ArtPixxelContext context)
        {
            _context = context;
        }
        public async Task<TemplateSizCRUDResponse> BatchCreate(List<TemplateSizeModel> request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                if (@request.Any())
                {
                    List<KidsTemplateSize> newTemplateSizes = new List<KidsTemplateSize>();
                    List<KidsTemplateSize> updatedTemplateSizes = new List<KidsTemplateSize>();
                    int affectedRows = 0;
                    KidsTemplate kidsTemplate = KidsTemplate.Setof3Set1;

                    foreach (TemplateSizeModel sizeModel in @request)
                    {
                        if (Enum.TryParse(sizeModel.SizeTemplate, out KidsTemplate template))
                        {
                            if (kidsTemplate != template)
                            {
                                kidsTemplate = template;
                            }


                            KidsTemplateSize size = new()
                            {
                                Template = template,
                                Name = sizeModel.SizeName,
                                Width = sizeModel.SizeWidth,
                                Height = sizeModel.SizeHeight,
                                Amount = sizeModel.SizeAmount,
                                Description = sizeModel.SizeDescription,
                                Default = sizeModel.SizeDefault == false ? newTemplateSizes.Any(e => e.Default == true && (e.Template == template)) ? false : true : sizeModel.SizeDefault,
                            };

                            newTemplateSizes.Add(size);


                        }

                        else
                        {

                            return new TemplateSizCRUDResponse
                            {
                                Sizes = new List<TemplateSizeModel>(),
                                Response = new BaseResponse
                                {
                                    Message = $"Template '{sizeModel.SizeTemplate}' does not exist. This template may have been removed or changed.",
                                    Result = RequestResult.Error,
                                    Succeeded = false,
                                    Title = "Empty Request"
                                }
                            };
                        }

                    }



                    // All passsed the test
                    if (newTemplateSizes.Any())
                    {


                        List<KidsTemplateSize> defaultTemplateSizes = await _context.KidsTemplateSizes
                             .Where(e => e.Default == true && (e.Template == kidsTemplate))
                             .ToListAsync();

                        if (newTemplateSizes.Any(e => e.Default == true))
                        {
                            if (defaultTemplateSizes.Any())
                            {
                                foreach (KidsTemplateSize defaultTemplateSize in defaultTemplateSizes)
                                {

                                    defaultTemplateSize.Default = false;
                                    updatedTemplateSizes.Add(defaultTemplateSize);
                                }
                            }

                        }



                        if (updatedTemplateSizes.Any())
                        {
                            _context.KidsTemplateSizes.UpdateRange(updatedTemplateSizes);
                            affectedRows = await _context.SaveChangesAsync();

                            if (affectedRows <= 0)
                            {

                                return new TemplateSizCRUDResponse
                                {
                                    Sizes = new List<TemplateSizeModel>(),
                                    Response = new BaseResponse
                                    {
                                        Message = "Defaut sizes  could not be set to non-default. Please try again later.",
                                        Result = RequestResult.Error,
                                        Succeeded = false,
                                        Title = "Unknown Error"
                                    }
                                };

                            }

                        }



                        _context.KidsTemplateSizes.AddRange(newTemplateSizes);
                        affectedRows = await _context.SaveChangesAsync();

                        List<TemplateSizeModel> sizes = affectedRows > 0 ? newTemplateSizes.Select(e => new TemplateSizeModel
                        {
                            SizeId = e.Id,
                            SizeAmount = decimal.Round(e.Amount, 2, MidpointRounding.AwayFromZero),
                            SizeDefault = e.Default,
                            SizeDescription = e.Description,
                            SizeHeight = e.Height,
                            SizeName = e.Name,
                            SizeTemplate = e.Template.ToString(),
                            SizeWidth = e.Width

                        }).ToList()

                         : new List<TemplateSizeModel>()
                         ;


                        return new TemplateSizCRUDResponse
                        {
                            Sizes = sizes,
                            Response = new BaseResponse
                            {
                                Message = affectedRows > 0 ? "Sizes successfully created." : "An error occurred. Sizes could not be created. Please try again later.",
                                Result = affectedRows > 0 ? RequestResult.Success : RequestResult.Error,
                                Succeeded = affectedRows > 0,
                                Title = affectedRows > 0 ? "Successful" : "Unknown Error"
                            }
                        };












                    }



                }

                return new TemplateSizCRUDResponse
                {
                    Sizes = new List<TemplateSizeModel>(),
                    Response = new BaseResponse
                    {
                        Message = "Mixed tmeplate sizes could not be created from an empty list.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Empty Request"
                    }
                };


            }
            catch (Exception ex)
            {

                return new TemplateSizCRUDResponse
                {
                    Sizes = new List<TemplateSizeModel>(),
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

        public async Task<TemplateSizCRUDResponse> Create(TemplateSizeModel request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (Enum.TryParse(@request.SizeTemplate, out KidsTemplate template))
                {
                    if (!await _context.KidsTemplateSizes.AnyAsync(e => e.Name == @request.SizeName && (e.Template == template)))
                    {
                        KidsTemplateSize defaultTemplateSize = await _context.KidsTemplateSizes
                               .Where(e => e.Template == template && (e.Default == true))
                               .FirstOrDefaultAsync();

                        List<KidsTemplateSize> newSizes = new List<KidsTemplateSize>();
                        int affectedRows = 0;



                        KidsTemplateSize size = new()
                        {
                            Template = template,
                            Name = @request.SizeName,
                            Width = @request.SizeWidth,
                            Height = @request.SizeHeight,
                            Amount = @request.SizeAmount,
                            Description = @request.SizeDescription,
                            Default = defaultTemplateSize == null ? true : @request.SizeDefault
                        };


                        if (@request.SizeDefault == true)
                        {
                            if (defaultTemplateSize != null)
                            {

                                defaultTemplateSize.Default = false;
                                _context.KidsTemplateSizes.Update(defaultTemplateSize);
                                affectedRows = await _context.SaveChangesAsync();

                                if (affectedRows > 0)
                                {
                                    newSizes.Add(defaultTemplateSize);


                                    _context.KidsTemplateSizes.Add(size);
                                    affectedRows = await _context.SaveChangesAsync();


                                    if (affectedRows > 0)
                                    {
                                        newSizes.Add(size);
                                    }


                                    List<TemplateSizeModel> sizes = affectedRows > 0 ? newSizes.Select(e => new TemplateSizeModel
                                    {
                                        SizeId = e.Id,
                                        SizeAmount = decimal.Round(e.Amount, 2, MidpointRounding.AwayFromZero),
                                        SizeDefault = e.Default,
                                        SizeDescription = e.Description,
                                        SizeHeight = e.Height,
                                        SizeName = e.Name,
                                        SizeTemplate = e.Template.ToString(),
                                        SizeWidth = e.Width

                                    }).ToList()

                                    : new List<TemplateSizeModel>()
                                    {
                                         request
                                     }
                                    ;


                                    return new TemplateSizCRUDResponse
                                    {
                                        Sizes = sizes,
                                        Response = new BaseResponse
                                        {
                                            Message = affectedRows > 0 ? $"Size ${request.SizeName} has been created." : $"An error occurred. Size ${request.SizeName} could not be created. Please try again later.",
                                            Result = affectedRows > 0 ? RequestResult.Success : RequestResult.Error,
                                            Succeeded = affectedRows > 0,
                                            Title = affectedRows > 0 ? "Successful" : "Unknown Error"
                                        }
                                    };





                                }


                                return new TemplateSizCRUDResponse
                                {
                                    Sizes = new List<TemplateSizeModel>(),
                                    Response = new BaseResponse
                                    {
                                        Message = $"Defaut size '{defaultTemplateSize.Name}' could not be set to non-default. Please try again later.",
                                        Result = RequestResult.Error,
                                        Succeeded = false,
                                        Title = "Unknown Error"
                                    }
                                };



                            }
                        }


                        _context.KidsTemplateSizes.Add(size);
                        affectedRows = await _context.SaveChangesAsync();

                        if (affectedRows > 0)
                        {
                            newSizes.Add(size);
                        }

                        List<TemplateSizeModel> brandNewSizes = newSizes.Any() ? newSizes.Select(e => new TemplateSizeModel
                        {
                            SizeId = e.Id,
                            SizeAmount = decimal.Round(e.Amount, 2, MidpointRounding.AwayFromZero),
                            SizeDefault = e.Default,
                            SizeDescription = e.Description,
                            SizeHeight = e.Height,
                            SizeName = e.Name,
                            SizeTemplate = e.Template.ToString(),
                            SizeWidth = e.Width

                        }).ToList()

                        : new List<TemplateSizeModel>()
                        ;


                        return new TemplateSizCRUDResponse
                        {
                            Sizes = brandNewSizes,
                            Response = new BaseResponse
                            {
                                Message = affectedRows > 0 ? $"Size ${request.SizeName} has been created." : $"An error occurred. Size ${request.SizeName} could not be created. Please try again later.",
                                Result = affectedRows > 0 ? RequestResult.Success : RequestResult.Error,
                                Succeeded = affectedRows > 0,
                                Title = affectedRows > 0 ? "Successful" : "Unknown Error"
                            }
                        };




                    }


                    return new TemplateSizCRUDResponse
                    {
                        Sizes = new List<TemplateSizeModel>(),
                        Response = new BaseResponse
                        {
                            Message = $"Size '{request.SizeName}'  already exists in template '{request.SizeTemplate}'.",
                            Result = RequestResult.Error,
                            Succeeded = false,
                            Title = "Duplicate Size"
                        }
                    };

                }

                return new TemplateSizCRUDResponse
                {
                    Sizes = new List<TemplateSizeModel>(),
                    Response = new BaseResponse
                    {
                        Message = $"Template '{request.SizeTemplate}' does not exist. This template may have been removed or changed.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Non-existent Template"
                    }
                };


            }
            catch (Exception ex)
            {
                return new TemplateSizCRUDResponse
                {
                    Sizes = new List<TemplateSizeModel>(),
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

        public async Task<BaseResponse> Delete(BaseId request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.KidsTemplateSizes.AnyAsync(e => e.Id == @request.Id))
                {

                    KidsTemplateSize kidsTemplateSize = await _context.KidsTemplateSizes.FindAsync(@request.Id);
                    if (kidsTemplateSize != null)
                    {
                        string sizeName = kidsTemplateSize.Name;
                        _context.KidsTemplateSizes.Remove(kidsTemplateSize);
                        await _context.SaveChangesAsync();


                        return new BaseResponse
                        {
                            Message = $"Template size ${sizeName} successfully deleted.",
                            Result = RequestResult.Success,
                            Succeeded = true,
                            Title = "Successful"
                        };
                    }
                }

                return new BaseResponse
                {
                    Message = "Reference to mixed template size could not be resolved. Request terminated.",
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = "Null Size Reference"

                };



            }
            catch (Exception ex)
            {

                return new BaseResponse
                {
                    Message = ex.Message,
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = ex.Message,
                };
            }
        }

        public async Task<BaseBoolResponse> Duplicate(DuplicateTemplateSize request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);


                if (Enum.TryParse(@request.Template, out KidsTemplate template))
                {
                    if (await _context.KidsTemplateSizes.AnyAsync(e => e.Name == @request.Name && (e.Id != @request.Id) && (e.Template == template)))
                    {
                        return new BaseBoolResponse
                        {
                            Exist = true,
                            Message = $"Size '{@request.Name}' already exists in '{template.ParsedName()}' template."
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
                    Message = $"Template {@request.Template} not understood"
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

        public async Task<BaseBoolResponse> Exists(DuplicateTemplateSize request)
        {
           try {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);


                if (Enum.TryParse(@request.Template, out KidsTemplate template))
                {
                    if (await _context.KidsTemplateSizes.AnyAsync(e => e.Name == @request.Name && (e.Template == template)))
                    {
                        return new BaseBoolResponse
                        {
                            Exist = true,
                            Message = $"Size '{@request.Name}' already exists in '{template.ParsedName()}' template."
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
                    Message = $"Template {@request.Template} not understood"
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

        public async Task<TemplateSizCRUDResponse> Sizes(BaseStringRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (Enum.TryParse(@request.Name, out KidsTemplate template))
                {

                    if (await _context.KidsTemplateSizes.AnyAsync(e => e.Template == template))
                    {
                        List<TemplateSizeModel> sizes = await _context.KidsTemplateSizes
                            .Where(e => e.Template == template)
                            .Select(e => new TemplateSizeModel
                            {
                                SizeId = e.Id,
                                SizeAmount = decimal.Round(e.Amount, 2, MidpointRounding.AwayFromZero),
                                SizeDefault = e.Default,
                                SizeDescription = e.Description,
                                SizeHeight = e.Height,
                                SizeName = e.Name,
                                SizeTemplate = e.Template.ToString(),
                                SizeWidth = e.Width

                            }).ToListAsync();

                        if (sizes.Any())
                        {
                            return new TemplateSizCRUDResponse
                            {
                                Sizes = sizes,
                                Response = new BaseResponse
                                {
                                    Message = string.Empty,
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = string.Empty
                                }
                            };
                        }

                    }


                    return new TemplateSizCRUDResponse
                    {
                        Sizes = new List<TemplateSizeModel>(),
                        Response = new BaseResponse
                        {
                            Message = $"No size maintained for template '{template.ParsedName()}' yet.",
                            Result = RequestResult.Warn,
                            Succeeded = true,
                            Title = "Empty List"
                        }

                    };


                }

                return new TemplateSizCRUDResponse
                {
                    Sizes = new List<TemplateSizeModel>(),
                    Response = new BaseResponse
                    {
                        Message = $"Template {@request.Name} not understood.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Invalid Template"
                    }

                };


            }
            catch (Exception ex)
            {

                return new TemplateSizCRUDResponse
                {
                    Sizes = new List<TemplateSizeModel>(),
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

        public async Task<TemplateStatisticsResponse> Statistics()
        {
            try
            {
                if (await _context.MixedTemplateSizes.AnyAsync())
                {
                    List<TemplateStatistics> statistics = await _context.KidsTemplateSizes.GroupBy(e => e.Template).Select(e => new TemplateStatistics
                    {
                        Template = e.Key.ToString(),
                        SizeCount = e.Cast<KidsTemplateSize>().Count(),

                    }).ToListAsync();


                    return new TemplateStatisticsResponse
                    {
                        Response = new BaseResponse
                        {
                            Message = string.Empty,
                            Result = RequestResult.Success,
                            Succeeded = true,
                            Title = string.Empty
                        },
                        Statistics = statistics
                    };
                }


                return new TemplateStatisticsResponse
                {
                    Response = new BaseResponse
                    {
                        Message = string.Empty,
                        Result = RequestResult.Success,
                        Succeeded = true,
                        Title = string.Empty
                    },
                    Statistics = new List<TemplateStatistics>()
                };

            }
            catch (Exception ex)
            {

                return new TemplateStatisticsResponse
                {
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    },
                    Statistics = new List<TemplateStatistics>()
                };
            }
        }

        public async Task<TemplateSizCRUDResponse> Update(TemplateSizeModel request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.KidsTemplateSizes.AnyAsync(e => e.Id == @request.SizeId))
                {

                    KidsTemplateSize kidsTemplateSize = await _context.KidsTemplateSizes.FindAsync(@request.SizeId);
                    if (kidsTemplateSize != null)
                    {
                        string oldName = kidsTemplateSize.Name;

                        if (Enum.TryParse(request.SizeTemplate, out KidsTemplate template))
                        {
                            List<KidsTemplateSize> updatedSizes = new List<KidsTemplateSize>();

                            KidsTemplateSize defaultTemplateSize = await _context.KidsTemplateSizes
                                .Where(e => e.Template == template && (e.Default == true))
                                .FirstOrDefaultAsync();

                            kidsTemplateSize.Template = template;
                            kidsTemplateSize.Name = @request.SizeName;
                            kidsTemplateSize.Width = @request.SizeWidth;
                            kidsTemplateSize.Height = @request.SizeHeight;
                            kidsTemplateSize.Amount = @request.SizeAmount;
                            kidsTemplateSize.Description = @request.SizeDescription;
                            kidsTemplateSize.Default = @request.SizeDefault;



                            if (@request.SizeDefault == true)
                            {

                                if (defaultTemplateSize != null && (defaultTemplateSize.Id != @request.SizeId))
                                {
                                    defaultTemplateSize.Default = false;

                                    updatedSizes.Add(defaultTemplateSize);

                                }
                            }

                            else
                            {
                                if (defaultTemplateSize == null || defaultTemplateSize.Id == @request.SizeId)
                                {
                                    kidsTemplateSize.Default = true;
                                }
                            }


                            updatedSizes.Add(kidsTemplateSize);

                            if (updatedSizes.Any())
                            {
                                _context.KidsTemplateSizes.UpdateRange(updatedSizes);
                                int upResult = await _context.SaveChangesAsync();

                                List<TemplateSizeModel> sizes = upResult > 0 ? updatedSizes.Select(e => new TemplateSizeModel
                                {
                                    SizeId = e.Id,
                                    SizeAmount = decimal.Round(e.Amount, 2, MidpointRounding.AwayFromZero),
                                    SizeDefault = e.Default,
                                    SizeDescription = e.Description,
                                    SizeHeight = e.Height,
                                    SizeName = e.Name,
                                    SizeTemplate = e.Template.ToString(),
                                    SizeWidth = e.Width

                                }).ToList()

                                : new List<TemplateSizeModel>()
                                {
                                   request
                                }

                                ;


                                return new TemplateSizCRUDResponse
                                {
                                    Sizes = sizes,
                                    Response = new BaseResponse
                                    {
                                        Message = upResult > 0 ? $"Size '{oldName}' has been updated." : $"An error occurred. Size '{oldName}' could not be updated. Please try again later.",
                                        Result = upResult > 0 ? RequestResult.Success : RequestResult.Error,
                                        Succeeded = upResult > 0,
                                        Title = upResult > 0 ? "Successful" : "Unknown Error"
                                    }
                                };


                            }

                        }


                        return new TemplateSizCRUDResponse
                        {
                            Sizes = new List<TemplateSizeModel>(),
                            Response = new BaseResponse
                            {
                                Message = $"Template '{request.SizeTemplate}' does not exist. This template may have been removed or changed.",
                                Result = RequestResult.Error,
                                Succeeded = false,
                                Title = "Non-existent Template"
                            }
                        };

                    }


                }


                return new TemplateSizCRUDResponse
                {
                    Sizes = new List<TemplateSizeModel>(),
                    Response = new BaseResponse
                    {
                        Message = "Reference to size could not be resolved. This size may have been deleted.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Null Size Reference"
                    }
                };
            }
            catch (Exception ex)
            {

                return new TemplateSizCRUDResponse
                {
                    Sizes = new List<TemplateSizeModel>(),
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
