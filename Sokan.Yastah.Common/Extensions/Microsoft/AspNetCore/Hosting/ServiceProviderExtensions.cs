using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting
{
    public static class ServiceProviderExtensions
    {
        public static async Task HandleStartupAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            using (var serviceScope = serviceProvider.CreateScope())
            {
                foreach (var startupHandler in serviceScope.ServiceProvider.GetServices<IStartupHandler>())
                    await startupHandler.OnStartupAsync(cancellationToken);
            }
        }
    }
}
