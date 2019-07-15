using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using AspNet.Security.OAuth.Discord;

using Sokan.Yastah.Business;

namespace Sokan.Yastah.Api.Authentication
{
    public class DiscordAuthenticationOptionsConfigurator
        : IPostConfigureOptions<DiscordAuthenticationOptions>
    {
        public DiscordAuthenticationOptionsConfigurator(
            DiscordClientConfiguration discordClientConfiguration)
        {
            _discordClientConfiguration = discordClientConfiguration;
        }

        public void PostConfigure(string name, DiscordAuthenticationOptions options)
        {
            options.ClientId = _discordClientConfiguration.ClientId.ToString();
            options.ClientSecret = _discordClientConfiguration.ClientSecret;
        }

        private readonly DiscordClientConfiguration _discordClientConfiguration;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddTransient<IPostConfigureOptions<DiscordAuthenticationOptions>, DiscordAuthenticationOptionsConfigurator>();
    }
}
