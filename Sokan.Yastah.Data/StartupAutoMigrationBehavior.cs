using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sokan.Yastah.Data
{
    public class StartupAutoMigrationBehavior
        : IStartupHandler
    {
        public StartupAutoMigrationBehavior(
            YastahDbContext yastahDbContext)
        {
            _yastahDbContext = yastahDbContext;
        }

        public Task OnStartupAsync()
            => _yastahDbContext.Database.MigrateAsync();

        private readonly YastahDbContext _yastahDbContext;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddTransient<IStartupHandler, StartupAutoMigrationBehavior>();
    }
}
