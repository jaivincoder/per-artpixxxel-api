


using api.artpixxel.data.Models;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Collections.Generic;
using api.artpixxel.data.Features.Common;
using System;
using api.artpixxel.data.Features.Metas;
using Microsoft.Extensions.Options;
using api.artpixxel.repo.Extensions;

namespace api.artpixxel.repo.Features.Prices
{
    public class MetaService : IMetaService
    {
        private readonly ArtPixxelContext _context;
        private readonly AppKeyConfig _appkeys;
        public MetaService(ArtPixxelContext context, IOptions<AppKeyConfig> appkeys)
        {
            _context = context;
            _appkeys = appkeys.Value;
        }


        private async Task UpdateMixnMatchPrice(decimal price)
        {
            List<MixnMatch> mixnMatches = await _context.MixnMatches.ToListAsync();
            if (mixnMatches.Any())
            {
                foreach (MixnMatch mixnMatch in mixnMatches)
                {
                    mixnMatch.Price = price;
                }

                _context.MixnMatches.UpdateRange(mixnMatches);
                await _context.SaveChangesAsync();


            }
        }




        public async Task<MetaResponse> Set(MetaBase request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);



                if (await _context.Metas.AnyAsync(e => e.MetaType == MetaType.PublishableKey))
                {
                    Meta publishableKey = await _context.Metas.Where(e => e.MetaType == MetaType.PublishableKey).FirstOrDefaultAsync();
                    if (publishableKey != null)
                    {
                        publishableKey.Value = @request.PublishableKey.Encrypt(_appkeys.EncryptionKey);
                        _context.Metas.Update(publishableKey);
                        await _context.SaveChangesAsync();

                    }

                    else
                    {
                        Meta _publishableKey = new()
                        {
                            Value = @request.PublishableKey.Encrypt(_appkeys.EncryptionKey),
                            MetaType = MetaType.PublishableKey,

                        };


                        _context.Metas.Add(_publishableKey);
                        await _context.SaveChangesAsync();

                    }
                }


                else
                {
                    Meta _publishableKey = new()
                    {
                        Value = @request.PublishableKey.Encrypt(_appkeys.EncryptionKey),
                        MetaType = MetaType.PublishableKey,

                    };


                    _context.Metas.Add(_publishableKey);
                    await _context.SaveChangesAsync();


                }






                if (await _context.Metas.AnyAsync(e => e.MetaType == MetaType.SecretKey))
                {
                    Meta secretKey = await _context.Metas.Where(e => e.MetaType == MetaType.SecretKey).FirstOrDefaultAsync();
                    if (secretKey != null)
                    {
                        secretKey.Value = @request.SecretKey.Encrypt(_appkeys.EncryptionKey);
                        _context.Metas.Update(secretKey);
                        await _context.SaveChangesAsync();

                    }

                    else
                    {
                        Meta _secretKey = new()
                        {
                            Value = @request.SecretKey.Encrypt(_appkeys.EncryptionKey),
                            MetaType = MetaType.SecretKey,

                        };


                        _context.Metas.Add(_secretKey);
                        await _context.SaveChangesAsync();

                    }
                }


                else
                {

                    Meta _secretKey = new()
                    {
                        Value = @request.SecretKey.Encrypt(_appkeys.EncryptionKey),
                        MetaType = MetaType.SecretKey,

                    };


                    _context.Metas.Add(_secretKey);
                    await _context.SaveChangesAsync();


                }









                if (await _context.Metas.AnyAsync(e => e.MetaType == MetaType.MixnMatch))
                {
                    Meta mixnMatchprice = await _context.Metas.Where(e => e.MetaType == MetaType.MixnMatch).FirstOrDefaultAsync();
                    if (mixnMatchprice != null)
                    {
                        mixnMatchprice.Value = @request.MixNMatchPrice;
                        _context.Metas.Update(mixnMatchprice);
                        int mxPriceUpdateResult = await _context.SaveChangesAsync();
                        if (mxPriceUpdateResult > 0)
                        {
                            await UpdateMixnMatchPrice(decimal.Parse(mixnMatchprice.Value));
                        }
                    }

                    else
                    {
                        Meta price = new()
                        {
                            Value = @request.MixNMatchPrice,
                            MetaType = MetaType.MixnMatch,

                        };


                        _context.Metas.Add(price);
                        int mxPriceCreateResult = await _context.SaveChangesAsync();
                        if (mxPriceCreateResult > 0)
                        {
                            await UpdateMixnMatchPrice(decimal.Parse(price.Value));
                        }
                    }
                }


                else
                {
                    Meta price = new()
                    {
                        Value = @request.MixNMatchPrice,
                        MetaType = MetaType.MixnMatch,

                    };


                    _context.Metas.Add(price);
                    int mxPriceCreateResult = await _context.SaveChangesAsync();
                    if (mxPriceCreateResult > 0)
                    {
                        await UpdateMixnMatchPrice(decimal.Parse(price.Value));
                    }


                }



                if (await _context.Metas.AnyAsync(e => e.MetaType == MetaType.UploadedImage))
                {
                    Meta uploadedImageprice = await _context.Metas.Where(e => e.MetaType == MetaType.UploadedImage).FirstOrDefaultAsync();
                    if (uploadedImageprice != null)
                    {
                        uploadedImageprice.Value = @request.UploadedImagePrice;
                        _context.Metas.Update(uploadedImageprice);
                        await _context.SaveChangesAsync();

                    }


                    else
                    {
                        Meta price = new()
                        {
                            Value = @request.UploadedImagePrice,
                            MetaType = MetaType.UploadedImage,

                        };


                        _context.Metas.Add(price);
                        await _context.SaveChangesAsync();
                    }
                }


                else
                {
                    Meta price = new()
                    {
                        Value = @request.UploadedImagePrice,
                        MetaType = MetaType.UploadedImage,

                    };


                    _context.Metas.Add(price);
                    await _context.SaveChangesAsync();

                }





                if (await _context.Metas.AnyAsync(e => e.MetaType == MetaType.KidsGalleryImage))
                {
                    Meta kidsGalleryImagePrice = await _context.Metas.Where(e => e.MetaType == MetaType.KidsGalleryImage).FirstOrDefaultAsync();
                    if (kidsGalleryImagePrice != null)
                    {
                        kidsGalleryImagePrice.Value = @request.KidsGalleryImagePrice;
                        _context.Metas.Update(kidsGalleryImagePrice);
                        await _context.SaveChangesAsync();

                    }


                    else
                    {
                        Meta price = new()
                        {
                            Value = @request.KidsGalleryImagePrice,
                            MetaType = MetaType.KidsGalleryImage,

                        };


                        _context.Metas.Add(price);
                        await _context.SaveChangesAsync();
                    }
                }


                else
                {
                    Meta price = new()
                    {
                        Value = @request.KidsGalleryImagePrice,
                        MetaType = MetaType.KidsGalleryImage,

                    };


                    _context.Metas.Add(price);
                    await _context.SaveChangesAsync();

                }










                if (await _context.Metas.AnyAsync(e => e.MetaType == MetaType.VAT))
                {
                    Meta VatAmount = await _context.Metas.Where(e => e.MetaType == MetaType.VAT).FirstOrDefaultAsync();
                    if (VatAmount != null)
                    {
                        VatAmount.Value = @request.VATAmount;
                        _context.Metas.Update(VatAmount);
                        await _context.SaveChangesAsync();

                    }


                    else
                    {
                        Meta VAT = new()
                        {
                            Value = @request.VATAmount,
                            MetaType = MetaType.VAT,

                        };


                        _context.Metas.Add(VAT);
                        await _context.SaveChangesAsync();
                    }
                }


                else
                {
                    Meta VAT = new()
                    {
                        Value = @request.VATAmount,
                        MetaType = MetaType.VAT,

                    };


                    _context.Metas.Add(VAT);
                    await _context.SaveChangesAsync();

                }



                return new MetaResponse
                {
                    Meta = await MetaData(),
                    Response = new BaseResponse
                    {
                        Message = "Meta configuration set.",
                        Result = RequestResult.Success,
                        Succeeded = true,
                        Title = "Successful"
                    }
                };

            }
            catch (Exception ex)
            {

                return new MetaResponse
                {
                    Meta = await MetaData(),
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

        public async Task<MetaModelBase> MetaData()
        {

            try
            {
                return new()
                {

                    PublishableKey = await _context.Metas.AnyAsync(e => e.MetaType == MetaType.PublishableKey) ? await _context.Metas.Where(e => e.MetaType == MetaType.PublishableKey).Select(p => new MetaModel
                    {

                        DateSet = p.ModifiedOn == null ? p.CreatedOn.ToString(DefaultDateFormat.ddMMyyyyhhmmsstt) : p.ModifiedOn.GetValueOrDefault().ToString(DefaultDateFormat.ddMMyyyyhhmmsstt),
                        Value = p.Value.Decrypt(_appkeys.EncryptionKey),
                        Type = p.MetaType.ToString()

                    }).FirstOrDefaultAsync() : new()
                    {
                        DateSet = "Not Yet",
                        Value = "",
                        Type = MetaType.PublishableKey.ToString()
                    },

                    SecretKey = await _context.Metas.AnyAsync(e => e.MetaType == MetaType.SecretKey) ? await _context.Metas.Where(e => e.MetaType == MetaType.SecretKey).Select(p => new MetaModel
                    {

                        DateSet = p.ModifiedOn == null ? p.CreatedOn.ToString(DefaultDateFormat.ddMMyyyyhhmmsstt) : p.ModifiedOn.GetValueOrDefault().ToString(DefaultDateFormat.ddMMyyyyhhmmsstt),
                        Value = p.Value.Decrypt(_appkeys.EncryptionKey),
                        Type = p.MetaType.ToString()

                    }).FirstOrDefaultAsync() : new()
                    {
                        DateSet = "Not Yet",
                        Value = "",
                        Type = MetaType.SecretKey.ToString()
                    },

                    MixNMatch = await _context.Metas.AnyAsync(e => e.MetaType == MetaType.MixnMatch) ? await _context.Metas.Where(e => e.MetaType == MetaType.MixnMatch).Select(p => new MetaModel
                    {

                        DateSet = p.ModifiedOn == null ? p.CreatedOn.ToString(DefaultDateFormat.ddMMyyyyhhmmsstt) : p.ModifiedOn.GetValueOrDefault().ToString(DefaultDateFormat.ddMMyyyyhhmmsstt),
                        Value = p.Value,
                        Type = p.MetaType.ToString()

                    }).FirstOrDefaultAsync() : new()
                    {
                        DateSet = "Not Yet",
                        Value = "0",
                        Type = MetaType.MixnMatch.ToString()
                    },
                    UploadedImage = await _context.Metas.AnyAsync(e => e.MetaType == MetaType.UploadedImage) ? await _context.Metas.Where(e => e.MetaType == MetaType.UploadedImage).Select(p => new MetaModel
                    {

                        DateSet = p.ModifiedOn == null ? p.CreatedOn.ToString(DefaultDateFormat.ddMMyyyyhhmmsstt) : p.ModifiedOn.GetValueOrDefault().ToString(DefaultDateFormat.ddMMyyyyhhmmsstt),
                        Value = p.Value,
                        Type = p.MetaType.ToString()

                    }).FirstOrDefaultAsync()
                : new()
                {
                    DateSet = "Not Yet",
                    Value = "0",
                    Type = MetaType.UploadedImage.ToString()
                },

                    KidsGalleryImage = await _context.Metas.AnyAsync(e => e.MetaType == MetaType.KidsGalleryImage) ? await _context.Metas.Where(e => e.MetaType == MetaType.KidsGalleryImage).Select(p => new MetaModel
                    {

                        DateSet = p.ModifiedOn == null ? p.CreatedOn.ToString(DefaultDateFormat.ddMMyyyyhhmmsstt) : p.ModifiedOn.GetValueOrDefault().ToString(DefaultDateFormat.ddMMyyyyhhmmsstt),
                        Value = p.Value,
                        Type = p.MetaType.ToString()

                    }).FirstOrDefaultAsync()
                : new()
                {
                    DateSet = "Not Yet",
                    Value = "0",
                    Type = MetaType.KidsGalleryImage.ToString()
                },

                    VAT = await _context.Metas.AnyAsync(e => e.MetaType == MetaType.VAT) ? await _context.Metas.Where(e => e.MetaType == MetaType.VAT).Select(p => new MetaModel
                    {

                        DateSet = p.ModifiedOn == null ? p.CreatedOn.ToString(DefaultDateFormat.ddMMyyyyhhmmsstt) : p.ModifiedOn.GetValueOrDefault().ToString(DefaultDateFormat.ddMMyyyyhhmmsstt),
                        Value = p.Value,
                        Type = p.MetaType.ToString()

                    }).FirstOrDefaultAsync()
                    :
                     new()
                     {
                         DateSet = "Not Yet",
                         Value = "0",
                         Type = MetaType.UploadedImage.ToString()
                     }
                };
            }
            catch (Exception)
            {

                throw;
            }







        }



        private async Task<List<ProductTypeMeta>> Products()
        {
            try
            {

                List<ProductTypeMeta> products = new();
                if (await _context.Products.AnyAsync())
                {
                    products = await _context.Products.Include(e => e.Templates).Select(e => new ProductTypeMeta
                    {
                        ProductType = e.Type.ToString(),
                        On = e.On,
                        Templates = e.Templates.Any() ? e.Templates.Select(tem => new ProductTemplateUpdateRequestModel
                        {
                            Name = tem.Name,
                            On = tem.On,
                            ProductType = e.Type.ToString()


                        }).ToList()

                        : new List<ProductTemplateUpdateRequestModel>() { }

                    }).ToListAsync();
                }

                return products;

            }

            catch
            {
                return new List<ProductTypeMeta>();
            }
        }

        public async Task<ProductMetaResponse> Template(ProductTemplateUpdateRequestModel request)
        {
            try
            {


                if (Enum.TryParse(request.ProductType, out ProductType productType))
                {

                    if (await _context.Templates.AnyAsync(e => e.Name == request.Name && e.Product.Type == productType))
                    {
                        //update

                        Template template = await _context.Templates.Where(e => e.Name == request.Name && e.Product.Type == productType).FirstOrDefaultAsync();
                        if (template != null)
                        {
                            template.On = request.On;

                            _context.Templates.Update(template);
                            int affected = await _context.SaveChangesAsync();
                            List<ProductTypeMeta> products = await Products();

                            if (affected > 0)
                            {

                                return new ProductMetaResponse
                                {

                                    Products = products,
                                    Response = new BaseResponse
                                    {
                                        Message = "Update Succceeded",
                                        Result = RequestResult.Success,
                                        Succeeded = true,
                                        Title = $"'{request.Name}' updated!"
                                    }
                                };

                            }


                            return new ProductMetaResponse
                            {

                                Products = products,
                                Response = new BaseResponse
                                {
                                    Message = "Update Error",
                                    Result = RequestResult.Error,
                                    Succeeded = false,
                                    Title = $"'{request.Name}' update failed. Please try again later."
                                }
                            };





                        }
                    }

                    else
                    {
                        // create a new one
                        Product product = null;

                        if (await _context.Products.AnyAsync(e => e.Type == productType))
                        {

                            product = await _context.Products.Where(e => e.Type == productType).FirstOrDefaultAsync();
                        }

                        else
                        {
                            product = new()
                            {
                                On = request.On,
                                Type = productType,

                            };

                            _context.Products.Add(product);
                            await _context.SaveChangesAsync();

                        }

                        if (product != null)
                        {
                            Template template = new()
                            {
                                Name = request.Name,
                                On = request.On,
                                ProductId = product.Id,

                            };


                            _context.Templates.Add(template);
                            int affected = await _context.SaveChangesAsync();
                            List<ProductTypeMeta> products = await Products();


                            if (affected > 0)
                            {

                                return new ProductMetaResponse
                                {
                                    Products = products,
                                    Response = new BaseResponse
                                    {
                                        Message = "Update Succceeded",
                                        Result = RequestResult.Success,
                                        Succeeded = true,
                                        Title = $"'{request.Name}' updated!"
                                    }
                                };


                            }



                            return new ProductMetaResponse
                            {
                                Products = products,
                                Response = new BaseResponse
                                {
                                    Message = "Update Error",
                                    Result = RequestResult.Error,
                                    Succeeded = false,
                                    Title = $"'{request.Name}' update ailed. Please try again later."
                                }
                            };

                        }

                    }

                }

                return new ProductMetaResponse
                {

                    Products = new List<ProductTypeMeta>(),
                    Response = new BaseResponse
                    {
                        Message = "Product Not Found!",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = $"{request.ProductType} is not a valid product type."
                    }
                };


            }
            catch (Exception ex)
            {

                return new ProductMetaResponse
                {
                    Products = new List<ProductTypeMeta>(),
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

        public async Task<ProductMetaResponse> Product(ProductUpdateRequestModel request)
        {
            try
            {
                if (Enum.TryParse(request.ProductType, out ProductType productType))
                {

                    if (await _context.Products.AnyAsync(e => e.Type == productType))
                    {
                        //update

                        Product product = await _context.Products.Where(e => e.Type == productType).FirstOrDefaultAsync();
                        if (product != null)
                        {
                            product.On = request.On;

                            _context.Products.Update(product);
                            int affected = await _context.SaveChangesAsync();
                            List<ProductTypeMeta> products = await Products();

                            if (affected > 0)
                            {

                                return new ProductMetaResponse
                                {
                                    Products = products,
                                    Response = new BaseResponse
                                    {
                                        Message = "Update Succceeded",
                                        Result = RequestResult.Success,
                                        Succeeded = true,
                                        Title = $"'{request.ProductType}' updated!"
                                    }
                                };

                            }

                            return new ProductMetaResponse
                            {
                                Products = products,
                                Response = new BaseResponse
                                {
                                    Message = "Update Error",
                                    Result = RequestResult.Error,
                                    Succeeded = false,
                                    Title = $"'{request.ProductType}' update failed. Please try again later."
                                }
                            };



                        }



                    }

                    else
                    {
                        //create a new one

                        Product product = new()
                        {
                            On = request.On,
                            Type = productType,

                        };

                        _context.Products.Add(product);
                        int affected = await _context.SaveChangesAsync();
                        List<ProductTypeMeta> products = await Products();


                        if (affected > 0)
                        {


                            return new ProductMetaResponse
                            {
                                Products = products,
                                Response = new BaseResponse
                                {
                                    Message = "Update Succceeded",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = $"'{request.ProductType}' updated!"
                                }
                            };



                        }


                        return new ProductMetaResponse
                        {
                            Products = products,
                            Response = new BaseResponse
                            {
                                Message = "Update Error",
                                Result = RequestResult.Error,
                                Succeeded = false,
                                Title = $"'{request.ProductType}' update failed. Please try again later."
                            }
                        };




                    }

                }



                return new ProductMetaResponse
                {
                    Products = new List<ProductTypeMeta>(),
                    Response = new BaseResponse
                    {
                        Message = "Product Not Found!",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = $"'{request.ProductType}' is not a valid product type."
                    }
                };


            }
            catch (Exception ex)
            {

                return new ProductMetaResponse
                {
                    Products = new List<ProductTypeMeta>(),
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
