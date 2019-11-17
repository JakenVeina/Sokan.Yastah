using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAssembly(this IServiceCollection services, Assembly assembly, IConfiguration configuration)
        {
            var handlers = OnConfigureServicesAttribute
                .EnumerateAttachedMethods(assembly);

            foreach (var handler in handlers)
            {
                if (handler is ConfigureServicesHandler csh)
                    csh.Invoke(services);
                else if(handler is ConfigureServicesWithConfigurationHandler cswch)
                    cswch.Invoke(services, configuration);
            }

            return services;
        }
    }
}
