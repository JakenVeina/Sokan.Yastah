using System;

using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Common.Messaging
{
    internal static class MessagingLogMessages
    {
        public enum EventType
        {
            NotificationPublishing      = CommonLogEventType.Messaging + 0x0001,
            NotificationPublished       = CommonLogEventType.Messaging + 0x0002,
            NotificationHandlerInvoking = CommonLogEventType.Messaging + 0x0003,
            NotificationHandlerInvoked  = CommonLogEventType.Messaging + 0x0004
        }

        public static void NotificationHandlerInvoked(
                ILogger logger)
            => _notificationHandlerInvoked.Invoke(
                logger);
        private static readonly Action<ILogger> _notificationHandlerInvoked
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.NotificationHandlerInvoking.ToEventId(),
                    $"{nameof(INotificationHandler<object>)} Invoked Successfully")
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
                    EventType.NotificationHandlerInvoking.ToEventId(),
                    $"Invoking {nameof(INotificationHandler<object>)}: {{NotificationHandler}}")
                .WithoutException();

        public static void NotificationPublished(
                ILogger logger)
            => _notificationPublished.Invoke(
                logger);
        private static readonly Action<ILogger> _notificationPublished
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.NotificationPublished.ToEventId(),
                    "Notification Published Successfully.")
                .WithoutException();

        public static void NotificationPublishing(
                ILogger logger,
                object notification)
            => _notificationPublishing.Invoke(
                logger,
                notification);
        private static readonly Action<ILogger, object> _notificationPublishing
            = LoggerMessage.Define<object>(
                    LogLevel.Debug,
                    EventType.NotificationPublishing.ToEventId(),
                    "Publishing Notification: {Notification}...")
                .WithoutException();
    }
}
