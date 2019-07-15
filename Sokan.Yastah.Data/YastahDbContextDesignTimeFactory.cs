using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sokan.Yastah.Data
{
    public class YastahDbContextDesignTimeFactory
        : IDesignTimeDbContextFactory<YastahDbContext>
    {
        public YastahDbContext CreateDbContext(string[] args)
            => new ServiceCollection()
                .AddYastahData(new ConfigurationBuilder()
                    .AddUserSecrets<YastahDbContext>()
                    .Build())
                .BuildServiceProvider()
                .GetRequiredService<YastahDbContext>();
    }
}
