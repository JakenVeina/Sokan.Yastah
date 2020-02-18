using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Sokan.Yastah.Business.Authorization
{
    public class AuthorizationConfiguration
    {
        [Required]
        public IReadOnlyList<ulong> AdminUserIds { get; set; }
            = null!;

        [Required]
        public IReadOnlyList<ulong> MemberGuildIds { get; set; }
            = null!;
    }

    [ServiceConfigurator]
    public class AuthorizationConfigurationConfigurator
        : IServiceConfigurator
    {
        public void ConfigureServices(
                IServiceCollection services,
                IConfiguration configuration)
            => services.AddOptions<AuthorizationConfiguration>()
                .Bind(configuration.GetSection("Authorization"))
                .ValidateDataAnnotations()
                .ValidateOnStartup();
    }
}
