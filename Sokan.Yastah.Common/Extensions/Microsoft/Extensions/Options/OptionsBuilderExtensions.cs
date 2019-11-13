using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.Options
{
    public static class OptionsBuilderExtensions
    {
        public static OptionsBuilder<TOptions> ValidateOnStartup<TOptions>(this OptionsBuilder<TOptions> optionsBuilder)
            where TOptions : class, new()
        {
            optionsBuilder.Services
                .AddTransient<IStartupHandler, OptionsValidationStartupHandler<TOptions>>();

            return optionsBuilder;
        }
    }
}
