using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Data
{
    [ServiceBinding(ServiceLifetime.Transient)]
    public class StartupAutoMigrationBehavior
        : IStartupHandler
    {
        public StartupAutoMigrationBehavior(
            ILogger<StartupAutoMigrationBehavior> logger,
            YastahDbContext yastahDbContext)
        {
            _logger = logger;
            _yastahDbContext = yastahDbContext;
        }

        public async Task OnStartupAsync(
            CancellationToken cancellationToken)
        {
            YastahDbContextLogMessages.ContextMigrating(_logger);
            await _yastahDbContext.Database.MigrateAsync(cancellationToken);
            YastahDbContextLogMessages.ContextMigrated(_logger);
        }

        private readonly ILogger _logger;
        private readonly YastahDbContext _yastahDbContext;
    }
}
