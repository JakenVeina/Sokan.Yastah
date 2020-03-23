using System;

using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Data.Auditing
{
    internal static class AuditingLogMessages
    {
        public enum EventType
        {
            AuditableActionCreating = DataLogEventType.Auditable + 0x0001,
            AuditableActionCreated  = DataLogEventType.Auditable + 0x0002
        }

        public static void AuditableActionCreated(
                ILogger logger,
                long actionId)
            => _auditableActionCreated.Invoke(
                logger,
                actionId);
        private static readonly Action<ILogger, long> _auditableActionCreated
            = LoggerMessage.Define<long>(
                    LogLevel.Information,
                    EventType.AuditableActionCreated.ToEventId(),
                    $"{nameof(AuditableActionEntity)} created: {{ActionId}}")
                .WithoutException();

        public static void AuditableActionCreating(
                ILogger logger,
                int typeId,
                DateTimeOffset performed,
                ulong? performedById)
            => _auditableActionCreating.Invoke(
                logger,
                typeId,
                performed,
                performedById);
        private static readonly Action<ILogger, int, DateTimeOffset, ulong?> _auditableActionCreating
            = LoggerMessage.Define<int, DateTimeOffset, ulong?>(
                    LogLevel.Information,
                    EventType.AuditableActionCreating.ToEventId(),
                    $"Creating {nameof(AuditableActionEntity)}:\r\n\tTypeId: {{TypeId}}\r\n\tPerformed: {{Performed}}\r\n\tPerformedById: {{PerformedById}}")
                .WithoutException();
    }
}
