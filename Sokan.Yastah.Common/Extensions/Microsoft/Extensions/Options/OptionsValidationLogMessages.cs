using System;

using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Options
{
    internal static class OptionsValidationLogMessages
    {
        public static void OptionsValidating(
                ILogger logger,
                Type optionsType)
            => _optionsValidating.Invoke(
                logger,
                optionsType);
        private static readonly Action<ILogger, Type> _optionsValidating
            = LoggerMessage.Define<Type>(
                    LogLevel.Information,
                    new EventId(3001, nameof(OptionsValidating)),
                    $"Validating {nameof(IOptions<object>)}: {{OptionsType}}")
                .WithoutException();

        public static void OptionsValidated(
                ILogger logger,
                object options)
            => _optionsValidated.Invoke(
                logger,
                options);
        private static readonly Action<ILogger, object> _optionsValidated
            = LoggerMessage.Define<object>(
                    LogLevel.Information,
                    new EventId(3002, nameof(OptionsValidated)),
                    $"{nameof(IOptions<object>)} validated: {{Options}}")
                .WithoutException();
    }
}
