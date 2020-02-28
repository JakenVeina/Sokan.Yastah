using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Options
{
    public class OptionsValidationStartupHandler<TOptions>
            : IStartupHandler
        where TOptions : class, new()
    {
        public OptionsValidationStartupHandler(
            ILogger<OptionsValidationStartupHandler<TOptions>> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task OnStartupAsync(
            CancellationToken cancellationToken)
        {
            OptionsValidationLogMessages.OptionsValidating(_logger, typeof(TOptions));
            var options = _serviceProvider.GetRequiredService<IOptions<TOptions>>();
            OptionsValidationLogMessages.OptionsValidated(_logger, options.Value);

            return Task.CompletedTask;
        }

        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
    }
}
