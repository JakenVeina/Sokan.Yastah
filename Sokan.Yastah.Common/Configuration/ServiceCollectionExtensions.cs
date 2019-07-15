using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurationOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
                where TOptions : class, new()
            => services
                .Configure<TOptions>(configuration)
                .AddSingleton(provider =>
                    provider.GetRequiredService<IOptions<TOptions>>().Value);

        public static IServiceCollection AddValidatedConfigurationOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
                where TOptions : class, IValidatable, new()
            => services
                .AddConfigurationOptions<TOptions>(configuration)
                .AddSingleton<IValidatable>(provider =>
                    provider.GetRequiredService<TOptions>());
    }
}
