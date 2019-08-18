using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sokan.Yastah.Api.Antiforgery
{
    public static class AntiforgerySetup
    {
        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services.AddAntiforgery();

        public static IApplicationBuilder UseAntiforgery(this IApplicationBuilder applicationBuilder)
            => applicationBuilder.UseMiddleware<AntiforgeryMiddleware>();
    }
}
