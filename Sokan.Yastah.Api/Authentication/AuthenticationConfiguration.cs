using System;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sokan.Yastah.Api.Authentication
{
    public class AuthenticationConfiguration
        : AutoValidatableObject
    {
        public TimeSpan? TokenLifetime { get; set; }

        [Required]
        public string TokenSecret { get; set; }

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddValidatedConfigurationOptions<AuthenticationConfiguration>(configuration.GetSection("Authentication"));
    }
}
