using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting
{
    public static class ServiceProviderExtensions
    {
        public static async Task HandleStartupAsync(this IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.CreateScope())
                await Task.WhenAll(serviceScope.ServiceProvider
                    .GetServices<IStartupHandler>()
                    .Select(x => x.OnStartupAsync()));
        }
    }
}
