using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Api
{
    internal static class ApiControllerLogMessages
    {
        public enum EventType
        {
            OperationModelStateValidating           = ApiLogEventType.Controller + 0x0001,
            OperationModelStateValidationFailed     = ApiLogEventType.Controller + 0x0002,
            OperationModelStateValidationSucceeded  = ApiLogEventType.Controller + 0x0003,
            OperationPerforming                     = ApiLogEventType.Controller + 0x0004,
            OperationPerformed                      = ApiLogEventType.Controller + 0x0005,
            OperationResultTranslating              = ApiLogEventType.Controller + 0x0006,
            OperationResultTranslated               = ApiLogEventType.Controller + 0x0007
        }

        public static void OperationModelStateValidating(
                ILogger logger,
                Delegate operation)
            => _operationModelStateValidating.Invoke(
                logger,
                operation);
        private static readonly Action<ILogger, Delegate> _operationModelStateValidating
            = LoggerMessage.Define<Delegate>(
                    LogLevel.Debug,
                    EventType.OperationModelStateValidating.ToEventId(),
                    "Validating Operation Model State: {Operation}")
                .WithoutException();

        public static void OperationModelStateValidationFailed(
                ILogger logger,
                ModelStateDictionary modelState)
            => _operationModelStateValidationFailed.Invoke(
                logger,
                modelState);
        private static readonly Action<ILogger, ModelStateDictionary> _operationModelStateValidationFailed
            = LoggerMessage.Define<ModelStateDictionary>(
                    LogLevel.Debug,
                    EventType.OperationModelStateValidationFailed.ToEventId(),
                    "Operation Model State is invalid: {ModelState}")
                .WithoutException();

        public static void OperationModelStateValidationSucceeded(
                ILogger logger)
            => _operationModelStateValidationSucceeded.Invoke(
                logger);
        private static readonly Action<ILogger> _operationModelStateValidationSucceeded
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.OperationModelStateValidationSucceeded.ToEventId(),
                    "Operation Model State is valid")
                .WithoutException();

        public static void OperationPerformed(
                ILogger logger,
                Delegate operation)
            => _operationPerformed.Invoke(
                logger,
                operation);
        private static readonly Action<ILogger, Delegate> _operationPerformed
            = LoggerMessage.Define<Delegate>(
                    LogLevel.Debug,
                    EventType.OperationPerformed.ToEventId(),
                    "Operation performed: {Operation}")
                .WithoutException();

        public static void OperationPerforming(
                ILogger logger,
                Delegate operation)
            => _operationPerforming.Invoke(
                logger,
                operation);
        private static readonly Action<ILogger, Delegate> _operationPerforming
            = LoggerMessage.Define<Delegate>(
                    LogLevel.Debug,
                    EventType.OperationPerforming.ToEventId(),
                    "Performing Operation: {Operation}")
                .WithoutException();

        public static void OperationResultTranslated(
                ILogger logger,
                IActionResult actionResult)
            => _operationResultTranslated.Invoke(
                logger,
                actionResult);
        private static readonly Action<ILogger, IActionResult> _operationResultTranslated
            = LoggerMessage.Define<IActionResult>(
                    LogLevel.Debug,
                    EventType.OperationResultTranslated.ToEventId(),
                    $"{nameof(OperationResult)} translated: {{ActionResult}}")
                .WithoutException();

        public static void OperationResultTranslating(
                ILogger logger,
                OperationResult operationResult)
            => _operationResultTranslating.Invoke(
                logger,
                operationResult);
        private static readonly Action<ILogger, OperationResult> _operationResultTranslating
            = LoggerMessage.Define<OperationResult>(
                    LogLevel.Debug,
                    EventType.OperationResultTranslating.ToEventId(),
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
                        EventType.OperationResultTranslating.ToEventId(),
                        $"Translating {nameof(OperationResult)}: {{OperationResult}}")
                    .WithoutException();
        }
    }
}
