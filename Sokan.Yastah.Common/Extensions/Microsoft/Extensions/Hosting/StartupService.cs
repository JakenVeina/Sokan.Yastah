using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting
{
    public class StartupService
        : IHostedService
    {
        public StartupService(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var serviceScope = _serviceProvider.CreateScope();

            foreach (var startupHandler in serviceScope.ServiceProvider.GetServices<IStartupHandler>())
                await startupHandler.OnStartupAsync(cancellationToken);
        }

        public Task StopAsync(
                CancellationToken cancellationToken)
            => Task.CompletedTask;

        private readonly IServiceProvider _serviceProvider;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
            => services
                .AddTransient<IHostedService, StartupService>();
    }
}
