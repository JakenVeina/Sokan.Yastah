using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Options
{
    public class OptionsValidationBehavior<TOptions>
            : IBehavior
        where TOptions : class, new()
    {
        #region Construction

        public OptionsValidationBehavior(
            ILogger<OptionsValidationBehavior<TOptions>> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        #endregion Construction

        #region IBehavior

        public Task StartAsync(
            CancellationToken cancellationToken)
        {
            OptionsValidationLogMessages.OptionsValidating(_logger, typeof(TOptions));
            var options = _serviceProvider.GetRequiredService<IOptions<TOptions>>();
            OptionsValidationLogMessages.OptionsValidated(_logger, options.Value);

            return Task.CompletedTask;
        }

        public Task StopAsync(
                CancellationToken cancellationToken)
            => Task.CompletedTask;

        #endregion IBehavior

        #region State

        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        #endregion State
    }
}
