using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Sokan.Yastah.Api.Antiforgery;

namespace Sokan.Yastah.Api
{
    public static class ApiSetup
    {
        public static IServiceCollection AddYastahApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvcCore()
                .AddAuthorization()
                .AddFormatterMappings()
                .AddCors();

            return services
                .AddAssembly(Assembly.GetExecutingAssembly(), configuration);
        }

        public static IApplicationBuilder UseYastahApi(this IApplicationBuilder applicationBuilder)
            => applicationBuilder
                .UseRouting()
                .UseAuthentication()
                .UseAntiforgery()
                .UseEndpoints(endpointRouteBuilder => endpointRouteBuilder
                    .MapControllers());
    }
}
