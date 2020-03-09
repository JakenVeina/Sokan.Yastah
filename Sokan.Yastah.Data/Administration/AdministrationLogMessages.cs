using System;

using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Data.Administration
{
    internal static class AdministrationLogMessages
    {
        public enum EventType
        {
            AdministrationActionCreating    = DataLogEventType.Administration + 0x0001,
            AdministrationActionCreated     = DataLogEventType.Administration + 0x0002
        }

        public static void AdministrationActionCreated(
                ILogger logger,
                long actionId)
            => _administrationActionCreated.Invoke(
                logger,
                actionId);
        private static readonly Action<ILogger, long> _administrationActionCreated
            = LoggerMessage.Define<long>(
                    LogLevel.Information,
                    EventType.AdministrationActionCreated.ToEventId(),
                    $"{nameof(AdministrationActionEntity)} created: {{ActionId}}")
                .WithoutException();

        public static void AdministrationActionCreating(
                ILogger logger,
                int typeId,
                DateTimeOffset performed,
                ulong? performedById)
            => _administrationActionCreating.Invoke(
                logger,
                typeId,
                performed,
                performedById);
        private static readonly Action<ILogger, int, DateTimeOffset, ulong?> _administrationActionCreating
            = LoggerMessage.Define<int, DateTimeOffset, ulong?>(
                    LogLevel.Information,
                    EventType.AdministrationActionCreating.ToEventId(),
                    $"Creating {nameof(AdministrationActionEntity)}:\r\n\tTypeId: {{TypeId}}\r\n\tPerformed: {{Performed}}\r\n\tPerformedById: {{PerformedById}}")
                .WithoutException();
    }
}
