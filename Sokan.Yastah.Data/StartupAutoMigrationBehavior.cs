using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sokan.Yastah.Data
{
    [ServiceBinding(ServiceLifetime.Transient)]
    public class StartupAutoMigrationBehavior
        : IStartupHandler
    {
        public StartupAutoMigrationBehavior(
            YastahDbContext yastahDbContext)
        {
            _yastahDbContext = yastahDbContext;
        }

        public Task OnStartupAsync(CancellationToken cancellationToken)
            => _yastahDbContext.Database.MigrateAsync(cancellationToken);

        private readonly YastahDbContext _yastahDbContext;
    }
}
