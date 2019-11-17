using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sokan.Yastah.Common.Messaging
{
    public interface IMessenger
    {
        Task PublishNotificationAsync<TNotification>(
            TNotification notification,
            CancellationToken cancellationToken);
    }

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

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
            => services
                .AddScoped<IMessenger, Messenger>();
    }
}
