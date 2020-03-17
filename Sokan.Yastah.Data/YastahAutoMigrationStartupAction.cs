using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Data
{
    public interface IYastahAutoMigrationStartupAction
        : IScopedStartupAction { }

    [ServiceBinding(ServiceLifetime.Singleton)]
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
}
