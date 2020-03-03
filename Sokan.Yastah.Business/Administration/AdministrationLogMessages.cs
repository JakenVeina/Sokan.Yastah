using System;

using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Business.Administration
{
    public static class AdministrationLogMessages
    {
        public static void AdministrationActionCreated(
                ILogger logger,
                long actionId)
            => _administrationActionCreated.Invoke(
                logger,
                actionId);
        private static readonly Action<ILogger, long> _administrationActionCreated
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    new EventId(4201, nameof(AdministrationActionCreated)),
                    "AdministrationAction created: {ActionId}")
                .WithoutException();
    }
}
