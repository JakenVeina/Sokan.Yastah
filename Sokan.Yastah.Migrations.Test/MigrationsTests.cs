using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

using Moq;
using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Concurrency;
using Sokan.Yastah.Data.Migrations;

namespace Sokan.Yastah.Data.Test
{
    [TestFixture]
    public class MigrationsTests
    {
        [Test]
        public void YastahDbContext_Always_ModelMatchesMigrationsSnapshot()
        {
            var optionsBuilder = new DbContextOptionsBuilder<YastahDbContext>()
                .UseNpgsql("Bogus connection string: we don't actually need to connect to the DB, just build ourselves a model.", optionsBuilder => optionsBuilder
                    .MigrationsAssembly(typeof(YastahDbContextDesignTimeFactory).Assembly.FullName));

            var concurrencyResolutionService = Mock.Of<IConcurrencyResolutionService>();

            using var context = new YastahDbContext(
                optionsBuilder.Options,
                concurrencyResolutionService);

            var infrastructure = context.GetInfrastructure();

            var differences = infrastructure.GetRequiredService<IMigrationsModelDiffer>().GetDifferences(
                    infrastructure.GetRequiredService<IMigrationsAssembly>().ModelSnapshot.Model,
                    context.Model);

            differences.ShouldBeEmpty();
        }
    }
}
