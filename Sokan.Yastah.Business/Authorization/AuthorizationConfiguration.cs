using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Sokan.Yastah.Business.Authorization
{
    public class AuthorizationConfiguration
    {
        [Required]
        public ulong[] AdminUserIds { get; set; }

        [Required]
        public ulong[] MemberGuildIds { get; set; }

        [OnConfigureServices]
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services.AddOptions<AuthorizationConfiguration>()
                .Bind(configuration.GetSection("Authorization"))
                .ValidateDataAnnotations()
                .ValidateOnStartup();
    }
}
