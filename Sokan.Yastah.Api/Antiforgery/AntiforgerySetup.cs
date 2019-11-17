using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sokan.Yastah.Api.Antiforgery
{
    public static class AntiforgerySetup
    {
        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
            => services.AddAntiforgery();

        public static IApplicationBuilder UseAntiforgery(this IApplicationBuilder applicationBuilder)
            => applicationBuilder.UseMiddleware<AntiforgeryMiddleware>();
    }
}
