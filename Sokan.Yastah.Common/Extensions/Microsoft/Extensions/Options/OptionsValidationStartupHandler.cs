using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.Options
{
    public class OptionsValidationStartupHandler<TOptions>
        : IStartupHandler
        where TOptions : class, new()
    {
        public OptionsValidationStartupHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task OnStartupAsync(CancellationToken cancellationToken)
        {
            _serviceProvider.GetRequiredService<IOptions<TOptions>>();

            return Task.CompletedTask;
        }

        private readonly IServiceProvider _serviceProvider;
    }
}
