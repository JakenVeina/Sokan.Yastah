using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

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

        public Task OnStartupAsync()
        {
            _serviceProvider.GetRequiredService<IOptions<TOptions>>();

            return Task.CompletedTask;
        }

        private readonly IServiceProvider _serviceProvider;
    }
}
