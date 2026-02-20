

using api.artpixxel.data.Features.Common;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using api.artpixxel.data.Models;
using api.artpixxel.data.Features.LeadTimes;
using api.artpixxel.data.Features.Sizes;
using api.artpixxel.data.Features.WallArtSizes;
using api.artpixxel.data.Features.HomeGalleries;
using api.artpixxel.data.Features.Metas;

namespace api.artpixxel.repo.Features.Common
{
    public class CommonService : ICommonService
    {
        private readonly ArtPixxelContext _context;
        public CommonService(ArtPixxelContext context)
        {
            _context = context;
        }
        public async Task<List<BaseOption>> Cities(BaseId request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (!string.IsNullOrEmpty(@request.Id))
                {
                    return await _context.Cities
                        .Where(s => s.StateId == @request.Id)
                        .OrderBy(f => f.CityName)
                  .Select(s => new BaseOption
                  {
                      Id = s.Id,
                      Name = s.CityName

                  }).ToListAsync();
                }


                return new List<BaseOption>();
              

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<CountryOption>> Countries()
        {
            try
            {
                return await _context.Countries
                    .Include(f => f.Flag)
                    .OrderBy(ff => ff.Name)
                    .Select(c => new CountryOption
                    { 
                     Id = c.Id,
                     Name = c.Name,
                     Flag = c.Flag.Name
                    })
                    .ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<LocationModel> LocationInfo()
        {
            try
            {
                return new LocationModel
                {
                    Countries = await _context.Countries
                    .Include(f => f.Flag)
                    .OrderBy(ff => ff.Name)
                    .Select(c => new DeliveryCountry
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Flag = c.Flag.Name,
                        DeliveryFee = decimal.Round(c.DeliveryFee,2, MidpointRounding.AwayFromZero)
                    })
                    .ToListAsync(),
                    States = await _context.States
                        .OrderBy(e => e.StateName)
                        .Select(s => new DeliveryState
                        {
                            Id = s.Id,
                            Name = s.StateName,
                            CountryId = s.CountryId,
                            DeliveryFee = decimal.Round(s.DeliveryFee, 2, MidpointRounding.AwayFromZero)

                        }).ToListAsync(),
                    Cities = await _context.Cities
                        .OrderBy(f => f.CityName)
                  .Select(s => new DeliveryCity
                  {
                      Id = s.Id,
                      Name = s.CityName,
                      StateId = s.StateId,
                      DeliveryFee = decimal.Round(s.DeliveryFee, 2, MidpointRounding.AwayFromZero)

                  }).ToListAsync()
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ShoppingInfo> ShoppingInfo()
        {
            try
            {
                List<PublicSize> sizes = new List<PublicSize>();
                List<PublicTemplateSize> templateSizes = new List<PublicTemplateSize>();
                List<PublicWallArtSize> wallArtSizes = new List<PublicWallArtSize>();
                List<PublicHomeGalleryModel> galleries = new List<PublicHomeGalleryModel>();
                List<ProductTypeMeta> products = new List<ProductTypeMeta>();

                Meta price = null;
                Meta kidsImageGalleryPrice = null;
                LeadTime leadTime = null;
                Meta VAT = null;


                //If there is customized product view
               
                if (await _context.Products.AnyAsync())
                {
                    products = await _context.Products.Include(e => e.Templates).Select(e => new ProductTypeMeta
                    {
                        ProductType = e.Type.ToString(),
                        On  =  e.On,
                        Templates = e.Templates.Any() ?  e.Templates.Select(tem => new ProductTemplateUpdateRequestModel
                        {
                            Name = tem.Name,
                            On = tem.On,
                            ProductType = e.Type.ToString()
                           

                        }).ToList()

                        : new List<ProductTemplateUpdateRequestModel>() { }

                    }).ToListAsync();
                }


                if(await _context.HomeGalleries.AnyAsync(e => e.Active == true))
                {
                    galleries = await _context.HomeGalleries.Where(e => e.Active == true)
                        .Select(e => new PublicHomeGalleryModel 
                        { 
                          Id = e.Id,
                          Image = e.ImageAbsURL,
                          Name = e.Name,   
                          Type = e.Type.ToString()
                        
                        })
                        .ToListAsync();
                }

                if (await _context.Metas.AnyAsync(e => e.MetaType == MetaType.UploadedImage))
                {
                    price = await _context.Metas.Where(e => e.MetaType == MetaType.UploadedImage).FirstOrDefaultAsync();
                }
                if(await _context.Metas.AnyAsync(e => e.MetaType == MetaType.KidsGalleryImage))
                {
                    kidsImageGalleryPrice = await _context.Metas.Where(e => e.MetaType == MetaType.KidsGalleryImage).FirstOrDefaultAsync();
                }
               
                if(await _context.LeadTimes.AnyAsync())
                {
                    leadTime = await _context.LeadTimes.FirstOrDefaultAsync();
                }

                if(await _context.Metas.AnyAsync(e => e.MetaType == MetaType.VAT))
                {
                    VAT = await _context.Metas.Where(e => e.MetaType == MetaType.VAT).FirstOrDefaultAsync();
                }


                if (await _context.KidsTemplateSizes.AnyAsync())
                {
                    templateSizes = await _context.KidsTemplateSizes.Select(e => new PublicTemplateSize
                    {
                        Amount = decimal.Round(e.Amount, 2, MidpointRounding.AwayFromZero),
                        Id = e.Id,
                        Name = e.Name,
                        Default = e.Default,
                       TemplateName = e.Template.ToString(),
                       TemplateType = TemplateType.KidsTemplate,
                        Height = e.Height ,
                        Width = e.Width,
                        Description = string.IsNullOrEmpty(e.Description) ? "" : e.Description,


                    }).ToListAsync();
                }


                if (await _context.MixedTemplateSizes.AnyAsync())
                {
                   List<PublicTemplateSize>  mixedTemplateSizes = await _context.MixedTemplateSizes.Select(e => new PublicTemplateSize
                    {
                        Amount = decimal.Round(e.Amount, 2, MidpointRounding.AwayFromZero),
                        Id = e.Id,
                        Name = e.Name,
                        Default = e.Default,
                        TemplateName = e.Template.ToString(),
                        TemplateType = TemplateType.MixedTemplate,
                        Height = e.Height,
                        Width = e.Width,
                        Description = string.IsNullOrEmpty(e.Description) ? "" : e.Description,


                    }).ToListAsync();

                    if (mixedTemplateSizes.Any())
                    {
                        templateSizes.AddRange(mixedTemplateSizes);
                    }

                }


                if (await _context.RegularTemplateSizes.AnyAsync())
                {
                    List<PublicTemplateSize> regularTemplateSizes = await _context.RegularTemplateSizes.Select(e => new PublicTemplateSize
                    {
                        Amount = decimal.Round(e.Amount, 2, MidpointRounding.AwayFromZero),
                        Id = e.Id,
                        Name = e.Name,
                        Default = e.Default,
                        TemplateName = e.Template.ToString(),
                        TemplateType = TemplateType.RegularTemplate,
                        Height = e.Height,
                        Width = e.Width,
                        Description = string.IsNullOrEmpty(e.Description) ? "" : e.Description,


                    }).ToListAsync();

                    if (regularTemplateSizes.Any())
                    {
                        templateSizes.AddRange(regularTemplateSizes);
                    }

                }


                if (await _context.ChristmasTemplateSizes.AnyAsync())
                {
                    List<PublicTemplateSize> christmasTemplateSizes = await _context.ChristmasTemplateSizes.Select(e => new PublicTemplateSize
                    {
                        Amount = decimal.Round(e.Amount, 2, MidpointRounding.AwayFromZero),
                        Id = e.Id,
                        Name = e.Name,
                        Default = e.Default,
                        TemplateName = e.Template.ToString(),
                        TemplateType = TemplateType.ChristmasTemplate,
                        Height = e.Height,
                        Width = e.Width,
                        Description = string.IsNullOrEmpty(e.Description) ? "" : e.Description,


                    }).ToListAsync();

                    if (christmasTemplateSizes.Any())
                    {
                        templateSizes.AddRange(christmasTemplateSizes);
                    }

                }



                if (await _context.Sizes.AnyAsync())
                {
                    sizes = await _context.Sizes.Select(e => new PublicSize
                    {
                        Amount = decimal.Round(e.Amount, 2, MidpointRounding.AwayFromZero),
                        Id = e.Id,
                        Name = e.Name,
                        Default = e.Default,
                        Type = e.Type.ToString(),
                        Height = e.Height < e.Width ? 1 : e.Height == e.Width ? 1 : e.Height / e.Width,
                        Width = e.Width < e.Height ? 1 : e.Width == e.Height ? 1 : e.Width / e.Height,
                        AspectRatio = e.Width + "/" + e.Height,

                    }).ToListAsync();
                }

                if(await _context.WallArtSizes.AnyAsync())
                {

                    wallArtSizes = await _context.WallArtSizes.Select(s => new PublicWallArtSize
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Amount = decimal.Round(s.Amount, 2, MidpointRounding.AwayFromZero)


                    }).ToListAsync();
                }




                return new()
                {
                    UploadedImagePrice = price == null ? Constants.DefaultPrice : string.IsNullOrEmpty(price.Value) ? Constants.DefaultPrice :  decimal.Parse(price.Value),
                    KidsGalleryImagePrice = kidsImageGalleryPrice == null ? Constants.DefaultPrice : string.IsNullOrEmpty(kidsImageGalleryPrice.Value) ? Constants.DefaultPrice : decimal.Parse(kidsImageGalleryPrice.Value),
                    VAT = VAT == null ? 0 :   string.IsNullOrEmpty(VAT.Value) ? 0 : decimal.Parse(VAT.Value) / 100m,
                    Sizes = sizes,
                    TemplateSizes = templateSizes,
                    WallArtSizes = wallArtSizes,
                    Galleries = galleries,
                    Products = products,
                    LeadTime = leadTime == null ? new LeadTimeBase
                    {
                        TimeLimit = Constants.LeadTimeLimit.ToString(),
                        LowerBandQuantifier = Constants.LeadTimeLowerBandQuantifier,
                       
                        UpperBandQuantifier = Constants.LeadTimeUpperBandQuantifier
                    }
                    :

                     new LeadTimeBase
                     {
                         TimeLimit = leadTime.TimeLimit.ToString(),
                         LowerBandQuantifier = leadTime.LowerBandQuantifier,
                         UpperBandQuantifier = leadTime.UpperBandQuantifier
                     }


                };
            }
            catch (Exception)
            {

                return new()
                {
                    UploadedImagePrice = Constants.DefaultPrice,
                    VAT = 0,
                    LeadTime = new LeadTimeBase
                    {
                        TimeLimit = Constants.LeadTimeLimit.ToString(),
                        LowerBandQuantifier = Constants.LeadTimeLowerBandQuantifier,
                        UpperBandQuantifier = Constants.LeadTimeUpperBandQuantifier
                    }
                };
            }
        }

        public async Task<List<BaseOption>> States(BaseId request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (!string.IsNullOrEmpty(request.Id))
                {
                    return await _context.States
                        .Where(e => e.CountryId == request.Id)
                        .OrderBy(e => e.StateName)
                        .Select(s => new BaseOption 
                        {
                            Id = s.Id,
                            Name = s.StateName
                        
                        }).ToListAsync();
                }



                return new List<BaseOption>();



            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
