

using api.artpixxel.data.Models;
using api.artpixxel.data.Services;
using api.artpixxel.Data;
using api.artpixxel.Infrastructure.Filters;
using api.artpixxel.repo;
using api.artpixxel.repo.Features.Accounts;
using api.artpixxel.repo.Features.AddressBooks;
using api.artpixxel.repo.Features.Admin;
using api.artpixxel.repo.Features.Checkouts;
using api.artpixxel.repo.Features.Cities;
using api.artpixxel.repo.Features.Common;
using api.artpixxel.repo.Features.Contacts;
using api.artpixxel.repo.Features.Countries;
using api.artpixxel.repo.Features.CustomerCategories;
using api.artpixxel.repo.Features.Customers;
using api.artpixxel.repo.Features.Emails;
using api.artpixxel.repo.Features.HomeGalleries;
using api.artpixxel.repo.Features.HomeSliders;
using api.artpixxel.repo.Features.KidsGalleries;
using api.artpixxel.repo.Features.KidsTemplates;
using api.artpixxel.repo.Features.LeadTimes;
using api.artpixxel.repo.Features.Messages;
using api.artpixxel.repo.Features.MixedTemplatesSizes;
using api.artpixxel.repo.Features.MixnMatchCategories;
using api.artpixxel.repo.Features.MixnMatches;
using api.artpixxel.repo.Features.Newsletters;
using api.artpixxel.repo.Features.Notifications;
using api.artpixxel.repo.Features.Orders;
using api.artpixxel.repo.Features.OrderStatuses;
using api.artpixxel.repo.Features.Payments;
using api.artpixxel.repo.Features.Prices;
using api.artpixxel.repo.Features.Sizes;
using api.artpixxel.repo.Features.SMTPs;
using api.artpixxel.repo.Features.States;
using api.artpixxel.repo.Features.RegularTemplateSizes;
using api.artpixxel.repo.Features.WallArtCategories;
using api.artpixxel.repo.Features.WallArts;
using api.artpixxel.repo.Features.WallArtSizes;
using api.artpixxel.service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using api.artpixxel.repo.Features.ChristmasTemplateSizes;
using api.artpixxel.repo.Features.FestiveDesigns;
using api.artpixxel.repo.Features.Frames;
using api.artpixxel.repo.Features.FrameCategories;
using api.artpixxel.repo.Features.TemplateConfigs;
using api.artpixxel.repo.Features.GalleryImages;

namespace api.artpixxel.Infrastructure.Extensions
{
    public static class ServiceCollectionsExtension
    {
        public static AppSettings GetAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppKeyConfig>(configuration.GetSection("AppKeys"));

            var appSettingsConfig = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsConfig);
            return appSettingsConfig.Get<AppSettings>();
        }


        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
            => services.AddDbContext<ArtPixxelContext>(options =>
            options.UseSqlServer(configuration.GetDefaultConnection()), ServiceLifetime.Transient);
        /// <summary>
        /// Configures identity user and returns it as a service
        /// </summary>

        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services
            //  .AddScoped<IRoleValidator<UserRole>, CustomRoleValidator>() //this line overides the default...
            .AddIdentity<User, UserRole>(options => {

                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;


            })
                //  .AddRoleValidator<CustomRoleValidator>() //this line overrides the default  ...
                .AddEntityFrameworkStores<ArtPixxelContext>()
                .AddDefaultTokenProviders()
                ;

            services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromMinutes(30));

            return services;
        }




        /// <summary>
        /// Configures Jwt Authentication and returns it as a service
        /// </summary>

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, AppSettings appSettings)
        {



            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            return services;
        }



        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
       => services
                   .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
                   .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>()
                   .AddScoped<ICurrentUserService, CurrentUserService>()
                   .AddScoped<IIPAddressService, IPAddressService>()
                   .AddScoped<IFrameCategoriesService, FrameCategoriesService>()
                   .AddScoped<ITemplateCondigsService, TemplateConfingService>()
                   .AddTransient<IAdminServices, AdminService>()
                   .AddTransient<IMixnMatchService, MixnMatchService>()
                   .AddTransient<IMixnMatchCategoryService, MixnMatchCategoryService>()
                   .AddTransient<IWallArtSizeService, WallArtSizeService>()
                   .AddTransient<IWallArtCategoryService, WallArtCategoryService>()
                   .AddTransient<IWallArtService, WallArtService>()
                   .AddTransient<ICustomerCategoryService, CustomerCategoryService>()
                   .AddTransient<IAccountService, AccountService>()
                   .AddTransient<IEmailService, EmailService>()  
                   .AddTransient<ICustomerService, CustomerService>()
                   .AddTransient<ICommonService, CommonService>()
                   .AddTransient<IAddressBookService, AddressBookService>()
                   .AddTransient<INewsletterService, NewsletterService>()
                   .AddTransient<IMessageService, MessageService>()
                   .AddTransient<INotificationService, NotificationService>()
                   .AddTransient<ICarouselService, CarouselService>()
                   .AddTransient<ICountryService, CountryService>()
                   .AddTransient<IStateService, StateService>()
                   .AddTransient<ICityService, CityService>()
                   .AddTransient<ISMTPService, SMTPService>()
                   .AddTransient<IMetaService, MetaService>()
                   .AddTransient<IOrderService, OrderService>()
                   .AddTransient<ILeadTimeService, LeadTimeService>()
                   .AddTransient<IOrderStatusService, OrderStatusService>()
                   .AddTransient<ICheckoutService, CheckoutService>()
                   .AddTransient<ICheckOutNewService, CheckOutNewService>()
                   .AddTransient<ISizeService, SizeService>()
                   .AddTransient<IPaymentService, PaymentService>()
                   .AddTransient<IContactService, ContactService>()
                   .AddTransient<IMixedTemplateSizeService, MixedTemplateSizeService>()
                   .AddTransient<IKidsTemplateSizeService, KidsTemplateSizeService>()
                   .AddTransient<IKidsGalleryImageService, KidsGalleryImageService>()
                   .AddTransient<IHomeGalleryService, HomeGalleryService>()
                   .AddTransient<IRegularTemplateSizeService, RegularTemplateSizeService>()
                   .AddTransient<IChristmasTemplateSizeService, ChristmasTemplateSizeService>()
                   .AddTransient<IFestiveDesignService, FestiveDesignService>()
                   .AddTransient<IFrameService, FrameService>()
                   .AddTransient<IGalleryImageService, GalleryImageService>();


        



        public static IServiceCollection AddSwagger(this IServiceCollection services)
          =>
           services.AddSwaggerGen(c =>
           {
               c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "ARTPIXXEL API",
                        Version = "v1"
                    });
               c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
               {
                   Description = "JWT Authorization header using the Bearer scheme",
                   Name = "Authorization",
                   In = ParameterLocation.Header,
                   Type = SecuritySchemeType.ApiKey,
                   Scheme = "Bearer"
               });
               c.AddSecurityRequirement(new OpenApiSecurityRequirement
               {
                   {
                       new OpenApiSecurityScheme
                       {
                           Reference = new OpenApiReference
                           {
                               Type = ReferenceType.SecurityScheme,
                               Id = "Bearer"
                           }
                       },
                       new string[] {}
                   }
               });
           });

        public static void AddApiControllers(this IServiceCollection services)
         => services.AddControllers(options => options
                            .Filters
                            .Add<ModelOrNotFoundActionFilter>());

    }
}
