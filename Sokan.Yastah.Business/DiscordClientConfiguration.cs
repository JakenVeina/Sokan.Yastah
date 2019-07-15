using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sokan.Yastah.Business
{
    public class DiscordClientConfiguration
        : AutoValidatableObject
    {
        [Required]
        public ulong ClientId { get; set; }

        [Required]
        public string ClientSecret { get; set; }

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddValidatedConfigurationOptions<DiscordClientConfiguration>(configuration.GetSection("Discord"));
    }
}
