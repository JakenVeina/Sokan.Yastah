using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sokan.Yastah.Business.Authorization
{
    public class AuthorizationConfiguration
        : AutoValidatableObject
    {
        [Required]
        public ulong[] AdminUserIds { get; set; }

        [Required]
        public ulong[] MemberGuildIds { get; set; }

        [OnConfigureServices]
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddValidatedConfigurationOptions<AuthorizationConfiguration>(configuration.GetSection("Authorization"));
    }
}
