using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Sokan.Yastah.Data.Concurrency;

namespace Sokan.Yastah.Data
{
    public class YastahDbContext
        : DbContext
    {
        // For Sokan.Yastah.Data.Test
        public YastahDbContext(
                IConcurrencyResolutionService concurrencyResolutionService)
            : base()
        {
            _concurrencyResolutionService = concurrencyResolutionService;
        }

        public YastahDbContext(
                DbContextOptions<YastahDbContext> options,
                IConcurrencyResolutionService concurrencyResolutionService)
            : base(options)
        {
            _concurrencyResolutionService = concurrencyResolutionService;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.ApplyAssembly(Assembly.GetExecutingAssembly());

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            // This seems dangerous, but simply serves to handle additional updates being performed to the entity while we are calculating the resolution.
            // Because we manually overwrite OriginalValues with DatabaseValues below, after the resolution, the only way the next call to SaveChangesAsync()
            // can fails is if an additional concurrent update occurs.
            while (true)
            {
                try
                {
                    return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    await _concurrencyResolutionService.HandleExceptionAsync(ex, cancellationToken);
                }
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // This seems dangerous, but simply serves to handle additional updates being performed to the entity while we are calculating the resolution.
            // Because we manually overwrite OriginalValues with DatabaseValues below, after the resolution, the only way the next call to SaveChangesAsync()
            // can fails is if an additional concurrent update occurs.
            while (true)
            {
                try
                {
                    return await base.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    await _concurrencyResolutionService.HandleExceptionAsync(ex, cancellationToken);
                }
            }
        }

        private readonly IConcurrencyResolutionService _concurrencyResolutionService;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services.AddDbContext<YastahDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Sokan.Yastah.Data")));
    }
}
