using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Moq;
using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Common.OperationModel;

using Sokan.Yastah.Data.Characters;

namespace Sokan.Yastah.Data.Test.Characters
{
    [TestFixture]
    public class CharacterLevelsRepositoryTests
    {
        internal class TestContext
            : MockYastahDbTestContext
        {
            public TestContext(bool isReadOnly = true)
                : base(isReadOnly)
            {
                MockTransactionScopeFactory = new Mock<ITransactionScopeFactory>();

                MockTransactionScope = new Mock<ITransactionScope>();

                MockTransactionScopeFactory
                    .Setup(x => x.CreateScope(It.IsAny<IsolationLevel?>()))
                    .Returns(() => MockTransactionScope.Object);
            }

            public readonly Mock<ITransactionScopeFactory> MockTransactionScopeFactory;
            public readonly Mock<ITransactionScope> MockTransactionScope;

            public CharacterLevelsRepository BuildUut()
                => new CharacterLevelsRepository(
                    MockContext.Object,
                    MockTransactionScopeFactory.Object);
        }

        #region CreateDefinitionAsync() Tests

        public static IReadOnlyList<TestCaseData> CreateDefinitionAsync_TestCaseData
            => new[]
            {
                /*                  level,          experienceThreshold,    creationId      */
                new TestCaseData(   default(int),   default(decimal),       default(long)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   int.MinValue,   decimal.MinValue,       long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1,              2M,                     3L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   4,              5M,                     6L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   7,              8M,                     9L              ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   int.MaxValue,   decimal.MaxValue,       long.MaxValue   ).SetName("{m}(Max Values)"),
            };

        [TestCaseSource(nameof(CreateDefinitionAsync_TestCaseData))]
        public async Task CreateDefinitionAsync_Always_CreatesNewEntities(
            int level,
            decimal experienceThreshold,
            long creationId)
        {
            using var testContext = new TestContext(isReadOnly: false);

            var levelDefinitionEntity = null as CharacterLevelDefinitionEntity;
            var levelDefinitionVersionEntity = null as CharacterLevelDefinitionVersionEntity;

            testContext.MockContext
                .Setup(x => x.AddAsync(It.IsAny<CharacterLevelDefinitionEntity>(), It.IsAny<CancellationToken>()))
                .Callback<CharacterLevelDefinitionEntity, CancellationToken>((x, y) => levelDefinitionEntity = x);
            testContext.MockContext
                .Setup(x => x.AddAsync(It.IsAny<CharacterLevelDefinitionVersionEntity>(), It.IsAny<CancellationToken>()))
                .Callback<CharacterLevelDefinitionVersionEntity, CancellationToken>((x, y) => levelDefinitionVersionEntity = x);

            testContext.MockContext
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback(() =>
                {
                    if ((levelDefinitionEntity is { })
                            && (levelDefinitionVersionEntity is { }))
                        levelDefinitionVersionEntity.Definition = levelDefinitionEntity;
                });

            var uut = testContext.BuildUut();

            await uut.CreateDefinitionAsnyc(
                level,
                experienceThreshold,
                creationId,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockContext
                .ShouldHaveReceived(x => x.AddAsync(It.IsNotNull<CharacterLevelDefinitionEntity>(), testContext.CancellationToken));
            testContext.MockContext
                .ShouldHaveReceived(x => x.AddAsync(It.IsNotNull<CharacterLevelDefinitionVersionEntity>(), testContext.CancellationToken));
            testContext.MockContext
                .ShouldHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken), Times.Exactly(2));

            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            levelDefinitionEntity!.Level.ShouldBe(level);

            levelDefinitionVersionEntity!.Level.ShouldBe(level);
            levelDefinitionVersionEntity!.ExperienceThreshold.ShouldBe(experienceThreshold);
            levelDefinitionVersionEntity.CreationId.ShouldBe(creationId);
            levelDefinitionVersionEntity.PreviousVersionId.ShouldBeNull();
            levelDefinitionVersionEntity.NextVersionId.ShouldBeNull();
            levelDefinitionVersionEntity.Definition.ShouldBeSameAs(levelDefinitionEntity);
        }

        #endregion CreateDefinitionAsync() Tests

        #region UpdateDefinitionAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> UpdateDefinitionAsync_LevelDoesNotExist_TestCaseData
            = new[]
            {
                /*                  level,  actionId,       experienceThreshold,                            isDeleted                       */
                new TestCaseData(   4,      default(long),  default(Optional<decimal>),                     default(Optional<bool>)         ).SetName("{m}(Default values)"),
                new TestCaseData(   4,      long.MinValue,  Optional<decimal>.FromValue(decimal.MinValue),  Optional<bool>.FromValue(false) ).SetName("{m}(Min values)"),
                new TestCaseData(   4,      1L,             Optional<decimal>.FromValue(2M),                Optional<bool>.FromValue(false) ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   4,      3L,             Optional<decimal>.FromValue(4M),                Optional<bool>.FromValue(true)  ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   4,      5L,             Optional<decimal>.FromValue(6M),                Optional<bool>.FromValue(false) ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   4,      long.MaxValue,  Optional<decimal>.FromValue(decimal.MaxValue),  Optional<bool>.FromValue(true)  ).SetName("{m}(Max values)"),
            };

        [TestCaseSource(nameof(UpdateDefinitionAsync_LevelDoesNotExist_TestCaseData))]
        public async Task UpdateDefinitionAsync_LevelDoesNotExist_ResultIsDataNotFound(
            int level,
            long actionId,
            Optional<decimal> experienceThreshold,
            Optional<bool> isDeleted)
        {
            using var testContext = new TestContext(isReadOnly: false);

            var uut = testContext.BuildUut();

            var result = await uut.UpdateDefinitionAsync(
                level,
                actionId,
                experienceThreshold,
                isDeleted,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory
                .ShouldHaveReceived(x => x.CreateScope(default));

            testContext.MockContext
                .ShouldNotHaveReceived(x => x.AddAsync(It.IsAny<CharacterLevelDefinitionVersionEntity>(), It.IsAny<CancellationToken>()));
            testContext.MockContext
                .ShouldNotHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));

            testContext.MockTransactionScope
                .ShouldNotHaveReceived(x => x.Complete());
            testContext.MockTransactionScope
                .ShouldHaveReceived(x => x.Dispose());

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeOfType<DataNotFoundError>();
            result.Error.Message.ShouldContain(level.ToString());
        }

        public static IReadOnlyList<TestCaseData> UpdateDefinitionAsync_NoChangesGiven_TestCaseData
            => new[]
            {
                /*                  level,  actionId,   experienceThreshold,                isDeleted                       */
                new TestCaseData(   1,      4L,         Optional<decimal>.Unspecified,      Optional<bool>.Unspecified      ).SetName("{m}(Level 1, no changes)"),
                new TestCaseData(   1,      5L,         Optional<decimal>.FromValue(12M),   Optional<bool>.FromValue(false) ).SetName("{m}(Level 1, no differences)"),
                new TestCaseData(   2,      6L,         Optional<decimal>.Unspecified,      Optional<bool>.Unspecified      ).SetName("{m}(Level 2, no changes)"),
                new TestCaseData(   2,      7L,         Optional<decimal>.FromValue(20M),   Optional<bool>.FromValue(false) ).SetName("{m}(Level 2, no differences)"),
                new TestCaseData(   3,      8L,         Optional<decimal>.Unspecified,      Optional<bool>.Unspecified      ).SetName("{m}(Level 3, no changes)"),
                new TestCaseData(   3,      9L,         Optional<decimal>.FromValue(31M),   Optional<bool>.FromValue(true)  ).SetName("{m}(Level 3, no differences)")
            };

        [TestCaseSource(nameof(UpdateDefinitionAsync_NoChangesGiven_TestCaseData))]
        public async Task UpdateDefinitionAsync_NoChangesAreNeeded_ResultIsNoChangesGiven(
            int level,
            long actionId,
            Optional<decimal> experienceThreshold,
            Optional<bool> isDeleted)
        {
            using var testContext = new TestContext(isReadOnly: false);

            var uut = testContext.BuildUut();

            var result = await uut.UpdateDefinitionAsync(
                level,
                actionId,
                experienceThreshold,
                isDeleted,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory
                .ShouldHaveReceived(x => x.CreateScope(default));

            testContext.MockContext
                .ShouldNotHaveReceived(x => x.AddAsync(It.IsAny<CharacterLevelDefinitionVersionEntity>(), It.IsAny<CancellationToken>()));
            testContext.MockContext
                .ShouldNotHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));

            testContext.MockTransactionScope
                .ShouldHaveReceived(x => x.Complete());
            testContext.MockTransactionScope
                .ShouldHaveReceived(x => x.Dispose());

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeOfType<NoChangesGivenError>();
            result.Error.Message.ShouldContain(level.ToString());
        }

        public static IReadOnlyList<TestCaseData> UpdateDefinitionAsync_ChangesGiven_TestCaseData
            => new[]
            {
                /*                  level,  actionId,   experienceThreshold,                isDeleted,                          versionId   */
                new TestCaseData(   1,      10L,        Optional<decimal>.FromValue(13M),   Optional<bool>.Unspecified,         19L         ).SetName("{m}(Level 1, ExperienceThreshold changed)"),
                new TestCaseData(   1,      11L,        Optional<decimal>.Unspecified,      Optional<bool>.FromValue(true),     20L         ).SetName("{m}(Level 1, IsDeleted changed)"),
                new TestCaseData(   1,      12L,        Optional<decimal>.FromValue(14M),   Optional<bool>.FromValue(true),     21L         ).SetName("{m}(Level 1, all properties changed)"),
                new TestCaseData(   2,      13L,        Optional<decimal>.FromValue(21M),   Optional<bool>.Unspecified,         22L         ).SetName("{m}(Level 2, ExperienceThreshold changed)"),
                new TestCaseData(   2,      14L,        Optional<decimal>.Unspecified,      Optional<bool>.FromValue(true),     23L         ).SetName("{m}(Level 2, IsDeleted changed)"),
                new TestCaseData(   2,      15L,        Optional<decimal>.FromValue(22M),   Optional<bool>.FromValue(true),     24L         ).SetName("{m}(Level 2, all properties changed)"),
                new TestCaseData(   3,      16L,        Optional<decimal>.FromValue(32M),   Optional<bool>.Unspecified,         25L         ).SetName("{m}(Level 3, ExperienceThreshold changed)"),
                new TestCaseData(   3,      17L,        Optional<decimal>.Unspecified,      Optional<bool>.FromValue(false),    26L         ).SetName("{m}(Level 3, IsDeleted changed)"),
                new TestCaseData(   3,      18L,        Optional<decimal>.FromValue(33M),   Optional<bool>.FromValue(false),    27L         ).SetName("{m}(Level 3, all properties changed)")
            };


        [TestCaseSource(nameof(UpdateDefinitionAsync_ChangesGiven_TestCaseData))]
        public async Task UpdateDefinitionAsync_Otherwise_ResultIsSuccess(
            int level,
            long actionId,
            Optional<decimal> experienceThreshold,
            Optional<bool> isDeleted,
            long versionId)
        {
            using var testContext = new TestContext(isReadOnly: false);

            testContext.MockContext
                .Setup(x => x.AddAsync(It.IsNotNull<CharacterLevelDefinitionVersionEntity>(), It.IsAny<CancellationToken>()))
                .Callback<CharacterLevelDefinitionVersionEntity, CancellationToken>((x, y) => x.Id = versionId);

            var previousVersionEntity = testContext.Entities.CharacterLevelDefinitionVersions.First(x => (x.Level == level) && (x.NextVersionId is null));

            var uut = testContext.BuildUut();

            var result = await uut.UpdateDefinitionAsync(
                level,
                actionId,
                experienceThreshold,
                isDeleted,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory
                .ShouldHaveReceived(x => x.CreateScope(default));

            testContext.MockContext
                .ShouldHaveReceived(x => x.AddAsync(It.IsAny<CharacterLevelDefinitionVersionEntity>(), testContext.CancellationToken));
            testContext.MockContext
                .ShouldHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));

            testContext.MockTransactionScope
                .ShouldHaveReceived(x => x.Complete());
            testContext.MockTransactionScope
                .ShouldHaveReceived(x => x.Dispose());

            var entity = testContext.MockContext
                .Invocations
                .Where(x => x.Method.Name == nameof(YastahDbContext.AddAsync))
                .Select(x => (CharacterLevelDefinitionVersionEntity)x.Arguments[0])
                .First();

            entity.ShouldNotBeNull();
            entity.Level.ShouldBe(level);
            entity.CreationId.ShouldBe(actionId);
            entity.NextVersionId.ShouldBeNull();
            entity.PreviousVersionId.ShouldBe(previousVersionEntity.Id);
            entity.ExperienceThreshold.ShouldBe(experienceThreshold.IsSpecified ? experienceThreshold.Value : previousVersionEntity.ExperienceThreshold);
            entity.IsDeleted.ShouldBe(isDeleted.IsSpecified ? isDeleted.Value : previousVersionEntity.IsDeleted);

            previousVersionEntity.NextVersion.ShouldBeSameAs(entity);

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(versionId);
        }

        #endregion UpdateDefinitionAsync() Tests
    }
}
