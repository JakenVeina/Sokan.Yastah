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
                CancellationToken cancellationToken)
            where TNotification : notnull;
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
            where TNotification : notnull
        {
            MessagingLogMessages.NotificationPublishing(_logger, notification);

            foreach (var handler in _serviceProvider.GetServices<INotificationHandler<TNotification>>())
            {
                MessagingLogMessages.NotificationHandlerInvoking(_logger, handler);
                await handler.OnNotificationPublishedAsync(notification, cancellationToken);
                MessagingLogMessages.NotificationHandlerInvoked(_logger);
            }

            MessagingLogMessages.NotificationPublished(_logger);
        }

        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
    }
}
