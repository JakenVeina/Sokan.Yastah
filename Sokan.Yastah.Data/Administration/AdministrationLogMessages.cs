using System;

using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Data.Administration
{
    internal static class AdministrationLogMessages
    {
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
                    new EventId(3001, nameof(AdministrationActionCreating)),
                    $"Creating {nameof(AdministrationActionEntity)}: \r\n\tTypeId: {{TypeId}}\r\n\tPerformed: {{Performed}}\r\n\tPerformedById: {{PerformedById}}")
                .WithoutException();

        public static void AdministrationActionCreated(
                ILogger logger,
                long actionId)
            => _administrationActionCreated.Invoke(
                logger,
                actionId);
        private static readonly Action<ILogger, long> _administrationActionCreated
            = LoggerMessage.Define<long>(
                    LogLevel.Information,
                    new EventId(3002, nameof(AdministrationActionCreated)),
                    $"{nameof(AdministrationActionEntity)} created: {{ActionId}}")
                .WithoutException();
    }
}
