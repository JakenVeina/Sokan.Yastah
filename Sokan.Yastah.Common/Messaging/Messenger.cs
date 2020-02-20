using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Common.Messaging
{
    public interface IMessenger
    {
        Task PublishNotificationAsync<TNotification>(
            TNotification notification,
            CancellationToken cancellationToken);
    }

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class Messenger
        : IMessenger
    {
        public Messenger(
            ILogger<Messenger> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task PublishNotificationAsync<TNotification>(
                TNotification notification,
                CancellationToken cancellationToken)
        {
            _logger.LogDebug("Publishing Notification: {0}", notification);

            foreach (var handler in _serviceProvider.GetServices<INotificationHandler<TNotification>>())
            {
                _logger.LogDebug("Invoking Handler: {0}", handler);
                await handler.OnNotificationPublishedAsync(notification, cancellationToken);
                _logger.LogDebug("Handler invoked successfully: {0}", handler);
            }

            _logger.LogDebug("Notification published successfully: {0}", notification);
        }

        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
    }
}
