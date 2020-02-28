using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Hosting
{
    internal static class StartupLogMessages
    {
        public static void StartupBeginning(
                ILogger logger)
            => _startupBeginning.Invoke(
                logger);
        private static readonly Action<ILogger> _startupBeginning
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(3001, nameof(StartupBeginning)),
                    "Beginning Startup...")
                .WithoutException();

        public static void StartupComplete(
                ILogger logger)
            => _startupComplete.Invoke(
                logger);
        private static readonly Action<ILogger> _startupComplete
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(3002, nameof(StartupComplete)),
                    "Startup Complete.")
                .WithoutException();

        public static void ServiceScopeCreated(
                ILogger logger,
                IServiceScope serviceScope)
            => _serviceScopeCreated.Invoke(
                logger,
                serviceScope);
        private static readonly Action<ILogger, IServiceScope> _serviceScopeCreated
            = LoggerMessage.Define<IServiceScope>(
                    LogLevel.Debug,
                    new EventId(4001, nameof(ServiceScopeCreated)),
                    $"{nameof(IServiceScope)} created: {{ServiceScope}}")
                .WithoutException();

        public static void StartupHandlerExecuting(
                ILogger logger,
                IStartupHandler startupHandler)
            => _startupHandlerExecuting.Invoke(
                logger,
                startupHandler);
        private static readonly Action<ILogger, IStartupHandler> _startupHandlerExecuting
            = LoggerMessage.Define<IStartupHandler>(
                    LogLevel.Debug,
                    new EventId(4002, nameof(StartupHandlerExecuting)),
                    $"Executing {nameof(IStartupHandler)}: {{StartupHandler}}...")
                .WithoutException();

        public static void StartupHandlerExecuted(
                ILogger logger)
            => _startupHandlerExecuted.Invoke(
                logger);
        private static readonly Action<ILogger> _startupHandlerExecuted
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4003, nameof(StartupHandlerExecuted)),
                    $"{nameof(IStartupHandler)} Executed...")
                .WithoutException();
    }
}
