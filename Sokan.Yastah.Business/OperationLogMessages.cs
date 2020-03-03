using System;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.Logging;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Business
{
    public static class OperationLogMessages
    {
        public static IDisposable BeginOperationScope(
                ILogger logger,
                [CallerMemberName]string operationName = default!)
            => _beginOperationScope.Invoke(
                logger,
                operationName);
        private static readonly Func<ILogger, string, IDisposable> _beginOperationScope
            = LoggerMessage.DefineScope<string>(
                "OperationName: {OperationName}");

        public static void OperationNotAuthorized(
                ILogger logger)
            => _operationNotAuthorized.Invoke(
                logger);
        private static readonly Action<ILogger> _operationNotAuthorized
            = LoggerMessage.Define(
                    LogLevel.Warning,
                    new EventId(2101, nameof(OperationNotAuthorized)),
                    "Operation not authorized")
                .WithoutException();

        public static void OperationPerforming(
                ILogger logger)
            => _operationPerforming.Invoke(
                logger);
        private static readonly Action<ILogger> _operationPerforming
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(3101, nameof(OperationPerforming)),
                    "Performing operation")
                .WithoutException();

        public static void OperationPerformed(
                ILogger logger,
                OperationResult operationResult)
            => _operationPerformed.Invoke(
                logger,
                operationResult);
        private static readonly Action<ILogger, OperationResult> _operationPerformed
            = LoggerMessage.Define<OperationResult>(
                    LogLevel.Information,
                    new EventId(3102, nameof(OperationPerformed)),
                    "Operation performed: {OperationResult}")
                .WithoutException();

        public static void OperationAuthorizing(
                ILogger logger)
            => _operationAuthorizing.Invoke(
                logger);
        private static readonly Action<ILogger> _operationAuthorizing
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4101, nameof(OperationAuthorizing)),
                    "Authorizing operation")
                .WithoutException();

        public static void OperationAuthorized(
                ILogger logger)
            => _operationAuthorized.Invoke(
                logger);
        private static readonly Action<ILogger> _operationAuthorized
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4102, nameof(OperationAuthorized)),
                    "Operation authorized")
                .WithoutException();
    }
}
