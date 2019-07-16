using System.Reflection;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sokan.Yastah.Data
{
    public class YastahDbContext
        : DbContext
    {
        public YastahDbContext(
                DbContextOptions<YastahDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.ApplyConfiguration(Assembly.GetExecutingAssembly());

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<YastahDbContext>(options =>
                options
                    .UseNpgsql(configuration.GetConnectionString("Sokan.Yastah.Data")));
        }
    }
}
