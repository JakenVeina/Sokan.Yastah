using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Api
{
    internal static class ApiControllerLogMessages
    {
        public static void OperationResultTranslating(
                ILogger logger,
                OperationResult operationResult)
            => _operationResultTranslating.Invoke(
                logger,
                operationResult);
        private static readonly Action<ILogger, OperationResult> _operationResultTranslating
            = LoggerMessage.Define<OperationResult>(
                    LogLevel.Debug,
                    new EventId(4001, nameof(OperationResultTranslating)),
                    $"Translating {nameof(OperationResult)}: {{OperationResult}}")
                .WithoutException();

        public static void OperationResultTranslating<T>(
                ILogger logger,
                OperationResult<T> operationResult)
            => OperationResultTranslatingMessage<T>.Instance.Invoke(
                logger,
                operationResult);
        private static class OperationResultTranslatingMessage<T>
        {
            public static readonly Action<ILogger, OperationResult<T>> Instance
                = LoggerMessage.Define<OperationResult<T>>(
                        LogLevel.Debug,
                        new EventId(4002, nameof(OperationResultTranslating)),
                        $"Translating {nameof(OperationResult)}: {{OperationResult}}")
                    .WithoutException();
        }

        public static void OperationResultTranslated(
                ILogger logger,
                IActionResult actionResult)
            => _operationResultTranslated.Invoke(
                logger,
                actionResult);
        private static readonly Action<ILogger, IActionResult> _operationResultTranslated
            = LoggerMessage.Define<IActionResult>(
                    LogLevel.Debug,
                    new EventId(4003, nameof(OperationResultTranslated)),
                    $"{nameof(OperationResult)} translated: {{ActionResult}}")
                .WithoutException();
    }
}
