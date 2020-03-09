using System;

using Microsoft.Extensions.Logging;

using Sokan.Yastah.Common;

namespace Microsoft.Extensions.Hosting
{
    internal static class HostingLogMessages
    {
        public enum EventType
        {
            BehaviorsStarting       = CommonLogEventType.Hosting + 0x0001,
            BehaviorsStarted        = CommonLogEventType.Hosting + 0x0002,
            BehaviorsStopping       = CommonLogEventType.Hosting + 0x0003,
            BehaviorsStopped        = CommonLogEventType.Hosting + 0x0004,
            StartupActionExecuting  = CommonLogEventType.Hosting + 0x0005,
            StartupActionExecuted   = CommonLogEventType.Hosting + 0x0006,
            BehaviorStarting        = CommonLogEventType.Hosting + 0x0007,
            BehaviorStarted         = CommonLogEventType.Hosting + 0x0008,
            BehaviorStopping        = CommonLogEventType.Hosting + 0x0009,
            BehaviorStopped         = CommonLogEventType.Hosting + 0x000A
        }

        public static void BehaviorsStarted(
                ILogger logger)
            => _behaviorsStarted.Invoke(
                logger);
        private static readonly Action<ILogger> _behaviorsStarted
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(3002, nameof(BehaviorsStarted)),
                    $"All {nameof(IBehavior)}'s started")
                .WithoutException();

        public static void BehaviorsStarting(
                ILogger logger)
            => _behaviorsStarting.Invoke(
                logger);
        private static readonly Action<ILogger> _behaviorsStarting
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(3001, nameof(BehaviorsStarting)),
                    $"Starting {nameof(IBehavior)}'s")
                .WithoutException();
                                    
        public static void BehaviorsStopped(
                ILogger logger)
            => _behaviorsStoped.Invoke(
                logger);
        private static readonly Action<ILogger> _behaviorsStoped
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(3004, nameof(BehaviorsStopped)),
                    $"All {nameof(IBehavior)}'s stoped")
                .WithoutException();

        public static void BehaviorsStopping(
                ILogger logger)
            => _behaviorsStoping.Invoke(
                logger);
        private static readonly Action<ILogger> _behaviorsStoping
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(3003, nameof(BehaviorsStopping)),
                    $"Stoping {nameof(IBehavior)}'s")
                .WithoutException();

        public static void BehaviorStarted(
                ILogger logger,
                IBehavior behavior)
            => _behaviorStarted.Invoke(
                logger,
                behavior);
        private static readonly Action<ILogger, IBehavior> _behaviorStarted
            = LoggerMessage.Define<IBehavior>(
                    LogLevel.Debug,
                    new EventId(4002, nameof(BehaviorStarted)),
                    $"{nameof(IBehavior)} started: {{Behavior}}")
                .WithoutException();

        public static void BehaviorStarting(
                ILogger logger,
                IBehavior behavior)
            => _behaviorStarting.Invoke(
                logger,
                behavior);
        private static readonly Action<ILogger, IBehavior> _behaviorStarting
            = LoggerMessage.Define<IBehavior>(
                    LogLevel.Debug,
                    new EventId(4001, nameof(BehaviorsStarting)),
                    $"Starting {nameof(IBehavior)}: {{Behavior}}")
                .WithoutException();

        public static void BehaviorStopped(
                ILogger logger,
                IBehavior behavior)
            => _behaviorStoped.Invoke(
                logger,
                behavior);
        private static readonly Action<ILogger, IBehavior> _behaviorStoped
            = LoggerMessage.Define<IBehavior>(
                    LogLevel.Debug,
                    new EventId(4004, nameof(BehaviorStopped)),
                    $"{nameof(IBehavior)} stoped: {{Behavior}}")
                .WithoutException();

        public static void BehaviorStopping(
                ILogger logger,
                IBehavior behavior)
            => _behaviorStoping.Invoke(
                logger,
                behavior);
        private static readonly Action<ILogger, IBehavior> _behaviorStoping
            = LoggerMessage.Define<IBehavior>(
                    LogLevel.Debug,
                    new EventId(4003, nameof(BehaviorsStopping)),
                    $"Stoping {nameof(IBehavior)}: {{Behavior}}")
                .WithoutException();

        public static void StartupActionExecuted(
                ILogger logger)
            => _startupActionExecuted.Invoke(
                logger);
        private static readonly Action<ILogger> _startupActionExecuted
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(3006, nameof(StartupActionExecuted)),
                    $"{nameof(ScopedStartupActionBase)} executed")
                .WithoutException();

        public static void StartupActionExecuting(
                ILogger logger)
            => _startupActionExecuting.Invoke(
                logger);
        private static readonly Action<ILogger> _startupActionExecuting
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(3005, nameof(StartupActionExecuting)),
                    $"Executing {nameof(ScopedStartupActionBase)}")
                .WithoutException();
    }
}
