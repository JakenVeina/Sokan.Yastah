using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Hosting
{
    [ServiceBinding(ServiceLifetime.Transient)]
    public class StartupService
        : IHostedService
    {
        public StartupService(
            ILogger<StartupService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Beginning Startup");

            using var serviceScope = _serviceProvider.CreateScope();

            _logger.LogDebug("IServiceScope created");

            foreach (var startupHandler in serviceScope.ServiceProvider.GetServices<IStartupHandler>())
            {
                _logger.LogDebug("Executing StartupHandler: {0}", startupHandler);
                await startupHandler.OnStartupAsync(cancellationToken);
                _logger.LogDebug("StartupHandler executed successfully: {0}", startupHandler);
            }

            _logger.LogInformation("Startup completed successfully");
        }

        public Task StopAsync(
                CancellationToken cancellationToken)
            => Task.CompletedTask;

        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
    }
}
