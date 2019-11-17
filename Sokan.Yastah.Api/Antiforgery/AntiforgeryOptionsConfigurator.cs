using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Sokan.Yastah.Api.Antiforgery
{
    public class AntiforgeryOptionsConfigurator
        : IPostConfigureOptions<AntiforgeryOptions>
    {
        public AntiforgeryOptionsConfigurator(
            IOptions<AntiforgeryConfiguration> antiforgeryConfiguration)
        {
            _antiforgeryConfiguration = antiforgeryConfiguration.Value;
        }

        public void PostConfigure(string name, AntiforgeryOptions options)
        {
            options.Cookie = new CookieBuilder()
            {
                HttpOnly = true,
                Name = _antiforgeryConfiguration.StateTokenCookieName ?? AntiforgeryDefaults.StateTokenCookieName,
                SameSite = SameSiteMode.Strict,
                SecurePolicy = CookieSecurePolicy.Always
            };
            options.HeaderName = _antiforgeryConfiguration.RequestTokenHeaderName ?? AntiforgeryDefaults.RequestTokenHeaderName;
        }

        private readonly AntiforgeryConfiguration _antiforgeryConfiguration;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
            => services
                .AddTransient<IPostConfigureOptions<AntiforgeryOptions>, AntiforgeryOptionsConfigurator>();
    }
}
