using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace System.ComponentModel.DataAnnotations
{
    public class StartupConfigurationValidator
        : IStartupHandler
    {
        public StartupConfigurationValidator(
            IServiceProvider serviceProvider,
            IEnumerable<IValidatable> validatableConfigurations)
        {
            _serviceProvider = serviceProvider;
            _validatableConfigurations = validatableConfigurations;
        }

        public Task OnStartupAsync()
        {
            foreach (var validatableConfiguration in _validatableConfigurations)
                validatableConfiguration.Validate(_serviceProvider);

            return Task.CompletedTask;
        }

        private readonly IServiceProvider _serviceProvider;

        private readonly IEnumerable<IValidatable> _validatableConfigurations;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddTransient<IStartupHandler, StartupConfigurationValidator>();
    }
}
