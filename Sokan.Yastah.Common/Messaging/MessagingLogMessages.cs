using System;

using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Common.Messaging
{
    internal static class MessagingLogMessages
    {
        public static void NotificationPublishing(
                ILogger logger,
                object notification)
            => _notificationPublishing.Invoke(
                logger,
                notification);
        private static readonly Action<ILogger, object> _notificationPublishing
            = LoggerMessage.Define<object>(
                    LogLevel.Debug,
                    new EventId(4001, nameof(NotificationPublishing)),
                    "Publishing Notification: {Notification}...")
                .WithoutException();

        public static void NotificationPublished(
                ILogger logger)
            => _notificationPublished.Invoke(
                logger);
        private static readonly Action<ILogger> _notificationPublished
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4002, nameof(NotificationPublished)),
                    "Notification Published Successfully.")
                .WithoutException();

        public static void NotificationHandlerInvoking(
                ILogger logger,
                object notificationHandler)
            => _notificationHandlerInvoking.Invoke(
                logger,
                notificationHandler);
        private static readonly Action<ILogger, object> _notificationHandlerInvoking
            = LoggerMessage.Define<object>(
                    LogLevel.Debug,
                    new EventId(4003, nameof(NotificationHandlerInvoking)),
                    $"Invoking {nameof(INotificationHandler<object>)}: {{NotificationHandler}}")
                .WithoutException();

        public static void NotificationHandlerInvoked(
                ILogger logger)
            => _notificationHandlerInvoked.Invoke(
                logger);
        private static readonly Action<ILogger> _notificationHandlerInvoked
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4004, nameof(NotificationHandlerInvoking)),
                    $"{nameof(INotificationHandler<object>)} Invoked Successfully")
                .WithoutException();
    }
}
