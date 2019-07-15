using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                .AddAssembly(Assembly.GetExecutingAssembly(), configuration);
        }

        public static IApplicationBuilder UseYastahWeb(this IApplicationBuilder applicationBuilder)
        {
            var hostingEnvironment = applicationBuilder.ApplicationServices
                .GetRequiredService<IHostingEnvironment>();

            applicationBuilder
                .UseStaticFiles()
                .UseSpaStaticFiles();

            applicationBuilder.UseSpa(spaBuilder =>
            {
                spaBuilder.Options.SourcePath = "ClientApp";

                if (hostingEnvironment.IsDevelopment())
                    spaBuilder.UseAngularCliServer(npmScript: "start");
            });

            return applicationBuilder;
        }
    }
}
