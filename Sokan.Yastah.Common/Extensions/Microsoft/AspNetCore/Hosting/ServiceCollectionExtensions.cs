using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAssembly(this IServiceCollection services, Assembly assembly, IConfiguration configuration)
        {
            var handlers = OnConfigureServicesAttribute
                .EnumerateAttachedMethods(assembly);

            foreach (var handler in handlers)
                handler.Invoke(services, configuration);

            return services;
        }
    }
}
