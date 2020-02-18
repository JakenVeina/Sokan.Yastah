using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Sokan.Yastah.Api.Authentication
{
    [ServiceBinding(ServiceLifetime.Transient)]
    public class ApiAuthenticationOptionsConfigurator
        : IPostConfigureOptions<ApiAuthenticationOptions>
    {
        public ApiAuthenticationOptionsConfigurator(
            IOptions<AuthenticationConfiguration> authenticationConfiguration)
        {
            _authenticationConfiguration = authenticationConfiguration.Value;
        }

        public void PostConfigure(string name, ApiAuthenticationOptions options)
        {
            if (_authenticationConfiguration.TokenLifetime.HasValue)
                options.TokenLifetime = _authenticationConfiguration.TokenLifetime.Value;

            options.TokenSecret = _authenticationConfiguration.TokenSecret;
        }

        private readonly AuthenticationConfiguration _authenticationConfiguration;
    }
}
