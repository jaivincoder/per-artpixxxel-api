
using api.artpixxel.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace api.artpixxel.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSwaggerUI(this IApplicationBuilder app)
        {
            return app.UseSwagger()
             .UseSwaggerUI(options =>
             {
                 options.SwaggerEndpoint("/swagger/v1/swagger.json", "ARTPIXXEL V1");
                 options.RoutePrefix = string.Empty;
             });


        }
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var services = app.ApplicationServices.CreateScope();
            var dbContext = services.ServiceProvider.GetService<ArtPixxelContext>();
            dbContext.Database.Migrate();
        }
    }
}
