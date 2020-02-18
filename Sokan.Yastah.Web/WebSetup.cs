using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sokan.Yastah.Web
{
    public static class WebSetup
    {
        public static IServiceCollection AddYastahWeb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSpaStaticFiles(options =>
            {
                options.RootPath = "ClientApp/dist";
            });

            return services
                .AddServices(Assembly.GetExecutingAssembly(), configuration);
        }

        public static IApplicationBuilder UseYastahWeb(this IApplicationBuilder applicationBuilder)
        {
            var webHostEnvironment = applicationBuilder.ApplicationServices
                .GetRequiredService<IWebHostEnvironment>();

            applicationBuilder
                .UseStaticFiles()
                .UseSpaStaticFiles();

            applicationBuilder.UseSpa(spaBuilder =>
            {
                // TODO: Is there a better way to configure this path?
                spaBuilder.Options.SourcePath = @"D:\Projects\Sokan.Yastah\Sokan.Yastah.Web\ClientApp";
                if (webHostEnvironment.IsDevelopment())
                    spaBuilder.UseAngularCliServer(npmScript: "start");
            });

            return applicationBuilder;
        }
    }
}
