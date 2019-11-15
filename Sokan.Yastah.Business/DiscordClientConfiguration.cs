using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Sokan.Yastah.Business
{
    public class DiscordClientConfiguration
    {
        [Required]
        public ulong ClientId { get; set; }

        [Required]
        public string ClientSecret { get; set; }
            = null!;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services.AddOptions<DiscordClientConfiguration>()
                .Bind(configuration.GetSection("Discord"))
                .ValidateDataAnnotations()
                .ValidateOnStartup();
    }
}
