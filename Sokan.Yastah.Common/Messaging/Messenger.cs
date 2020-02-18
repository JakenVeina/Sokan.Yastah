using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

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
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task PublishNotificationAsync<TNotification>(
                TNotification notification,
                CancellationToken cancellationToken)
        {
            var handlers = _serviceProvider.GetServices<INotificationHandler<TNotification>>();

            foreach (var handler in handlers)
                await handler.OnNotificationPublishedAsync(notification, cancellationToken);
        }

        private readonly IServiceProvider _serviceProvider;
    }
}
