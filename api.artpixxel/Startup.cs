
using api.artpixxel.data.Models;
using api.artpixxel.Data;
using api.artpixxel.Infrastructure.Extensions;
using api.artpixxel.Infrastructure.Middleware;
using api.artpixxel.repo.Seed;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace api.artpixxel
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        => services.AddDatabase(this.Configuration)
                  .AddIdentity() //registers the services
                  .AddJwtAuthentication(services.GetAppSettings(this.Configuration))
                  .AddApplicationServices()
                  .AddSwagger()
                  .AddApiControllers();

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ArtPixxelContext cont, RoleManager<UserRole> roleManager, UserManager<User> userManager)
        {


            if (env.IsDevelopment())
            {

                // app.UseDatabaseErrorPage();
            }

           

            app.UseSwaggerUI()
             .UseRouting()
             .UseCors(options => options
             .AllowAnyOrigin()
             .AllowAnyHeader()
             .AllowAnyMethod()
             
             )
             .UseStaticFiles()
             .UseAuthentication()
             .UseAuthorization()

             .UseEndpoints(endpoints =>
             {
                 endpoints.MapControllers();

             })
             .ApplyMigrations();

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            Seed.InitializeDataBase(app).Wait();
            UserSeed.Initialize(cont, userManager, roleManager, env).Wait();

        }
    }
}
