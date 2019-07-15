using System.Threading;
using System.Threading.Tasks;

namespace Sokan.Yastah.Common.Messaging
{
    public interface INotificationHandler<TNotification>
    {
        Task OnNotificationPublishedAsync(TNotification notification, CancellationToken cancellationToken);
    }
}
