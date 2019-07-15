using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Sokan.Yastah.Api.Authentication
{
    public class ApiAuthenticationOptionsConfigurator
        : IPostConfigureOptions<ApiAuthenticationOptions>
    {
        public ApiAuthenticationOptionsConfigurator(
            AuthenticationConfiguration authenticationConfiguration)
        {
            _authenticationConfiguration = authenticationConfiguration;
        }

        public void PostConfigure(string name, ApiAuthenticationOptions options)
        {
            if (_authenticationConfiguration.TokenLifetime.HasValue)
                options.TokenLifetime = _authenticationConfiguration.TokenLifetime.Value;

            options.TokenSecret = _authenticationConfiguration.TokenSecret;
        }

        private readonly AuthenticationConfiguration _authenticationConfiguration;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddTransient<IPostConfigureOptions<ApiAuthenticationOptions>, ApiAuthenticationOptionsConfigurator>();
    }
}
