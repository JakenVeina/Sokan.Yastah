using System;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sokan.Yastah.Data
{
    public partial class YastahDbContext
        : DbContext
    {
        public YastahDbContext(
                DbContextOptions<YastahDbContext> options,
                IServiceProvider serviceProvider)
            : base(options)
        {
            _serviceProvider = serviceProvider;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var modelCreatingHandlers = _serviceProvider
                .GetServices<IModelCreatingHandler<YastahDbContext>>();

            foreach (var handler in modelCreatingHandlers)
                handler.OnModelCreating(modelBuilder);
        }

        private readonly IServiceProvider _serviceProvider;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            ModelCreatingHandlerAttribute
                .EnumerateAttachedTypes<YastahDbContext>(Assembly.GetExecutingAssembly())
                .ForEach(handlerType => services.AddTransient(typeof(IModelCreatingHandler<YastahDbContext>), handlerType));

            services.AddDbContext<YastahDbContext>(options =>
                options
                    .UseNpgsql(configuration.GetConnectionString("Sokan.Yastah.Data")));
        }
    }
}
