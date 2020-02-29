using System;

using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Data
{
    public static class YastahDbContextLogMessages
    {
        public static void ContextMigrating(
                ILogger logger)
            => _contextMigrating.Invoke(
                logger);
        private static readonly Action<ILogger> _contextMigrating
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(3201, nameof(ContextMigrating)),
                    $"Applying {nameof(YastahDbContext)} migrations")
                .WithoutException();

        public static void ContextMigrated(
                ILogger logger)
            => _contextMigrated.Invoke(
                logger);
        private static readonly Action<ILogger> _contextMigrated
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(3202, nameof(ContextMigrated)),
                    $"{nameof(YastahDbContext)} migrations applied")
                .WithoutException();

        public static void ContextSavingChanges(
                ILogger logger)
            => _contextSavingChanges.Invoke(
                logger);
        private static readonly Action<ILogger> _contextSavingChanges
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4201, nameof(ContextSavingChanges)),
                    $"{nameof(YastahDbContext)} saving changes")
                .WithoutException();
    }
}
