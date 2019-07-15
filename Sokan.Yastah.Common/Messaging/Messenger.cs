using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        public Task PublishNotificationAsync<TNotification>(
                TNotification notification,
                CancellationToken cancellationToken)
            => Task.WhenAll(_serviceProvider.GetServices<INotificationHandler<TNotification>>()
                .Select(handler => handler.OnNotificationPublishedAsync(notification, cancellationToken)));

        private readonly IServiceProvider _serviceProvider;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddScoped<IMessenger, Messenger>();
    }
}
