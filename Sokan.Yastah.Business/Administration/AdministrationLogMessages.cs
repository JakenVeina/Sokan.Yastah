using System;

using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Business.Administration
{
    internal static class AdministrationLogMessages
    {
        public enum EventType
        {
            AdministrationActionCreated = BusinessLogEventType.Administration + 0x0001
        }

        public static void AdministrationActionCreated(
                ILogger logger,
                long actionId)
            => _administrationActionCreated.Invoke(
                logger,
                actionId);
        private static readonly Action<ILogger, long> _administrationActionCreated
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    EventType.AdministrationActionCreated.ToEventId(),
                    "AdministrationAction created: {ActionId}")
                .WithoutException();
    }
}
