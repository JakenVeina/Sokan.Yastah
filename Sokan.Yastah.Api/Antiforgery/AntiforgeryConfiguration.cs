using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sokan.Yastah.Api.Antiforgery
{
    public class AntiforgeryConfiguration
    {
        public string? RequestTokenCookieName { get; set; }

        public string? RequestTokenHeaderName { get; set; }

        public string? StateTokenCookieName { get; set; }
    }

    [ServiceConfigurator]
    public class AntiforgeryConfigurationConfigurator
        : IServiceConfigurator
    {
        public void ConfigureServices(
                IServiceCollection services,
                IConfiguration configuration)
            => services.AddOptions<AntiforgeryConfiguration>()
                .Bind(configuration.GetSection("Antiforgery"));
    }
}
