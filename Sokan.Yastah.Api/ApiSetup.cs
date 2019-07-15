using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sokan.Yastah.Api
{
    public static class ApiSetup
    {
        public static IServiceCollection AddYastahApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvcCore()
                .AddAuthorization()
                .AddFormatterMappings()
                .AddJsonFormatters()
                .AddCors();

            return services
                .AddAssembly(Assembly.GetExecutingAssembly(), configuration);
        }

        public static IApplicationBuilder UseYastahApi(this IApplicationBuilder applicationBuilder)
            => applicationBuilder
                .UseAuthentication()
                .UseMvc();
    }
}
