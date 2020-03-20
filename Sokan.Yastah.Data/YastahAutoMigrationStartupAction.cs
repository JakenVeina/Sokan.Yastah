using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Data
{
    public interface IYastahAutoMigrationStartupAction
        : IScopedStartupAction { }

    public class YastahAutoMigrationStartupAction
        : ScopedStartupActionBase,
            IYastahAutoMigrationStartupAction
    {
        public YastahAutoMigrationStartupAction(
                ILogger<YastahAutoMigrationStartupAction> logger,
                IServiceScopeFactory serviceScopeFactory)
            : base(
                logger,
                serviceScopeFactory) { }

        protected override async Task OnStartingAsync(
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            YastahDbContextLogMessages.ContextMigrating(_logger);
            await serviceProvider.GetRequiredService<YastahDbContext>().Database.MigrateAsync(cancellationToken);
            YastahDbContextLogMessages.ContextMigrated(_logger);
        }
    }

    [ServiceConfigurator]
    public class YastahAutoMigrationStartupActionServiceConfigurator
        : IServiceConfigurator
    {
        public void ConfigureServices(
                IServiceCollection services,
                IConfiguration configuration)
            => services.AddSingleton<IYastahAutoMigrationStartupAction, IBehavior, YastahAutoMigrationStartupAction>();
    }
}
