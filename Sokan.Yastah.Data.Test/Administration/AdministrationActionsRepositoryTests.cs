using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;
using Moq;
using Shouldly;

using Sokan.Yastah.Data.Administration;

using Sokan.Yastah.Common.Test;
using Sokan.Yastah.Data.Concurrency;

namespace Sokan.Yastah.Data.Test.Administration
{
    [TestFixture]
    public class AdministrationActionsRepositoryTests
    {
        #region CreateAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> CreateAsyncTestCaseData
            = new[]
            {
                /*                  typeId          performed                   performedById       id              */
                new TestCaseData(   default(int),   default(DateTimeOffset),    default(ulong?),    default(long))  .SetName("{m}(Default Values)"),
                new TestCaseData(   int.MinValue,   DateTimeOffset.MinValue,    ulong.MinValue,     long.MinValue)  .SetName("{m}(Min Values)"),
                new TestCaseData(   1,              DateTimeOffset.UnixEpoch,   4UL,                7L)             .SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   2,              DateTimeOffset.Now,         5UL,                8L)             .SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   3,              DateTimeOffset.UtcNow,      6UL,                9L)             .SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   int.MaxValue,   DateTimeOffset.MaxValue,    ulong.MaxValue,     long.MaxValue)  .SetName("{m}(Max Values)"),
            };

        [TestCaseSource(nameof(CreateAsyncTestCaseData))]
        public async Task CreateAsync_Always_AddsNewEntity(
            int typeId,
            DateTimeOffset performed,
            ulong? performedById,
            long id)
        {
            using var testContext = new AsyncMethodTestContext();
            
            var mockContext = new Mock<YastahDbContext>(
                new Mock<IConcurrencyResolutionService>().Object);
            mockContext
                .Setup(x => x.AddAsync(It.IsAny<AdministrationActionEntity>(), It.IsAny<CancellationToken>()))
                .Callback<AdministrationActionEntity, CancellationToken>((x, y) => x.Id = id);

            var uut = new AdministrationActionsRepository(
                mockContext.Object);

            var result = await uut.CreateAsync(
                typeId,
                performed,
                performedById,
                testContext.CancellationToken);

            mockContext
                .ShouldHaveReceived(x => x.AddAsync(It.IsNotNull<AdministrationActionEntity>(), testContext.CancellationToken));
            mockContext
                .ShouldHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));

            var entity = mockContext
                .Invocations
                .Where(x => x.Method.Name == nameof(YastahDbContext.AddAsync))
                .Select(x => (AdministrationActionEntity)x.Arguments[0])
                .First();

            entity.TypeId.ShouldBe(typeId);
            entity.Performed.ShouldBe(performed);
            entity.PerformedById.ShouldBe(performedById);

            result.ShouldBe(id);
        }

        #endregion CreateAsync() Tests
    }
}
