using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sokan.Yastah.Data.Migrations
{
    public class YastahDbContextDesignTimeFactory
        : IDesignTimeDbContextFactory<YastahDbContext>
    {
        public YastahDbContext CreateDbContext(string[] args)
            #pragma warning disable IDISP004 // Don't ignore created IDisposable.
            => new ServiceCollection()
                .AddYastahData(new ConfigurationBuilder()
                    .AddUserSecrets<YastahDbContext>()
                    .Build())
                .BuildServiceProvider()
            #pragma warning restore IDISP004 // Don't ignore created IDisposable.
                .GetRequiredService<YastahDbContext>();
    }
}
