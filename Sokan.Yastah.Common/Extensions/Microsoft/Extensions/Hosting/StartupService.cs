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
            StartupLogMessages.StartupBeginning(_logger);

            using var serviceScope = _serviceProvider.CreateScope();
            StartupLogMessages.ServiceScopeCreated(_logger, serviceScope);

            foreach (var startupHandler in serviceScope.ServiceProvider.GetServices<IStartupHandler>())
            {
                StartupLogMessages.StartupHandlerExecuting(_logger, startupHandler);
                await startupHandler.OnStartupAsync(cancellationToken);
                StartupLogMessages.StartupHandlerExecuted(_logger);
            }

            StartupLogMessages.StartupComplete(_logger);
        }

        public Task StopAsync(
                CancellationToken cancellationToken)
            => Task.CompletedTask;

        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
    }
}
