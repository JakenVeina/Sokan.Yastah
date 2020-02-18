using System;
using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Sokan.Yastah.Api.Authentication
{
    public class AuthenticationConfiguration
    {
        public TimeSpan? TokenLifetime { get; set; }

        [Required]
        public string TokenSecret { get; set; }
            = null!;
    }

    [ServiceConfigurator]
    public class AuthenticationConfigurationConfigurator
        : IServiceConfigurator
    {
        public void ConfigureServices(
                IServiceCollection services,
                IConfiguration configuration)
            => services.AddOptions<AuthenticationConfiguration>()
                .Bind(configuration.GetSection("Authentication"))
                .ValidateDataAnnotations()
                .ValidateOnStartup();

    }
}
