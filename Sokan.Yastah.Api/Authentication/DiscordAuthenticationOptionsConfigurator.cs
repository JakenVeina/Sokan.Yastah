using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using AspNet.Security.OAuth.Discord;

using Sokan.Yastah.Business;

namespace Sokan.Yastah.Api.Authentication
{
    [ServiceBinding(ServiceLifetime.Transient)]
    public class DiscordAuthenticationOptionsConfigurator
        : IPostConfigureOptions<DiscordAuthenticationOptions>
    {
        public DiscordAuthenticationOptionsConfigurator(
            IOptions<DiscordClientConfiguration> discordClientConfiguration)
        {
            _discordClientConfiguration = discordClientConfiguration.Value;
        }

        public void PostConfigure(string name, DiscordAuthenticationOptions options)
        {
            options.ClientId = _discordClientConfiguration.ClientId.ToString();
            options.ClientSecret = _discordClientConfiguration.ClientSecret;
        }

        private readonly DiscordClientConfiguration _discordClientConfiguration;
    }
}
