using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sokan.Yastah.Api.Antiforgery
{
    public class AntiforgeryConfiguration
    {
        public string? RequestTokenCookieName { get; set; }

        public string? RequestTokenHeaderName { get; set; }

        public string? StateTokenCookieName { get; set; }

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services.AddOptions<AntiforgeryConfiguration>()
                .Bind(configuration.GetSection("Antiforgery"));
    }
}
