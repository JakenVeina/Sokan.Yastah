using System;

using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Business.Auditing
{
    internal static class AuditingLogMessages
    {
        public enum EventType
        {
            AuditingActionCreated = BusinessLogEventType.Auditing + 0x0001
        }

        public static void AuditingActionCreated(
                ILogger logger,
                long actionId)
            => _auditingActionCreated.Invoke(
                logger,
                actionId);
        private static readonly Action<ILogger, long> _auditingActionCreated
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    EventType.AuditingActionCreated.ToEventId(),
                    "AuditingAction created: {ActionId}")
                .WithoutException();
    }
}
