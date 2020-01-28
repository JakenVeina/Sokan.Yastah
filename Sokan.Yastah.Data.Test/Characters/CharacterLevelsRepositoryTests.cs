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
            public static TestContext CreateReadOnly()
                => new TestContext(
                    CharacterLevelsTestEntitySetBuilder.SharedSet);

            public static TestContext CreateReadWrite()
                => new TestContext(
                    CharacterLevelsTestEntitySetBuilder.NewSet());

            private TestContext(
                    YastahTestEntitySet entities)
                : base(
                    entities)
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

        #region AnyAsync() Tests

        public static IReadOnlyList<TestCaseData> AnyAsync_TestCaseData
            = new[]
            {
                /*                  level,                      experienceThreshold,            isDeleted                           expectedResult  */
                new TestCaseData(   Optional<int>.Unspecified,  Optional<int>.Unspecified,      Optional<bool>.Unspecified,         true            ).SetName("{m}(No criteria specified)"),
                new TestCaseData(   Optional<int>.FromValue(1), Optional<int>.Unspecified,      Optional<bool>.Unspecified,         true            ).SetName("{m}(Level 1 exists)"),
                new TestCaseData(   Optional<int>.FromValue(2), Optional<int>.Unspecified,      Optional<bool>.Unspecified,         true            ).SetName("{m}(Level 2 exists)"),
                new TestCaseData(   Optional<int>.FromValue(3), Optional<int>.Unspecified,      Optional<bool>.Unspecified,         true            ).SetName("{m}(Level 3 exists)"),
                new TestCaseData(   Optional<int>.FromValue(4), Optional<int>.Unspecified,      Optional<bool>.Unspecified,         false           ).SetName("{m}(Level 4 does not exist)"),
                new TestCaseData(   Optional<int>.Unspecified,  Optional<int>.FromValue(0),     Optional<bool>.Unspecified,         true            ).SetName("{m}(ExperienceThreshold 0 exists)"),
                new TestCaseData(   Optional<int>.Unspecified,  Optional<int>.FromValue(10),    Optional<bool>.Unspecified,         false           ).SetName("{m}(ExperienceThreshold 10 does not exist)"),
                new TestCaseData(   Optional<int>.Unspecified,  Optional<int>.FromValue(11),    Optional<bool>.Unspecified,         false           ).SetName("{m}(ExperienceThreshold 11 does not exist)"),
                new TestCaseData(   Optional<int>.Unspecified,  Optional<int>.FromValue(20),    Optional<bool>.Unspecified,         true            ).SetName("{m}(ExperienceThreshold 20 exists)"),
                new TestCaseData(   Optional<int>.Unspecified,  Optional<int>.FromValue(21),    Optional<bool>.Unspecified,         false           ).SetName("{m}(ExperienceThreshold 21 does not exist)"),
                new TestCaseData(   Optional<int>.Unspecified,  Optional<int>.FromValue(31),    Optional<bool>.Unspecified,         true            ).SetName("{m}(ExperienceThreshold 31 exists)"),
                new TestCaseData(   Optional<int>.Unspecified,  Optional<int>.Unspecified,      Optional<bool>.FromValue(true),     true            ).SetName("{m}(Deleted definitions exist)"),
                new TestCaseData(   Optional<int>.Unspecified,  Optional<int>.Unspecified,      Optional<bool>.FromValue(false),    true            ).SetName("{m}(Undeleted definitions exist)"),
                new TestCaseData(   Optional<int>.FromValue(3), Optional<int>.Unspecified,      Optional<bool>.FromValue(true),     true            ).SetName("{m}(Deleted definition exists)"),
                new TestCaseData(   Optional<int>.FromValue(1), Optional<int>.Unspecified,      Optional<bool>.FromValue(true),     false           ).SetName("{m}(Deleted definition does not exist)"),
                new TestCaseData(   Optional<int>.FromValue(2), Optional<int>.Unspecified,      Optional<bool>.FromValue(false),    true            ).SetName("{m}(Uneleted definition exists)"),
                new TestCaseData(   Optional<int>.FromValue(3), Optional<int>.Unspecified,      Optional<bool>.FromValue(false),    false           ).SetName("{m}(Undeleted definition does not exist)"),
                new TestCaseData(   Optional<int>.FromValue(3), Optional<int>.FromValue(20),    Optional<bool>.FromValue(true),     true            ).SetName("{m}(All criteria specified, match exists)"),
                new TestCaseData(   Optional<int>.FromValue(3), Optional<int>.FromValue(30),    Optional<bool>.FromValue(true),     false           ).SetName("{m}(All criteria specified, ExperienceThreshold match does not exist)"),
                new TestCaseData(   Optional<int>.FromValue(4), Optional<int>.FromValue(20),    Optional<bool>.FromValue(true),     false           ).SetName("{m}(All criteria specified, Level match does not exist)"),
                new TestCaseData(   Optional<int>.FromValue(3), Optional<int>.FromValue(20),    Optional<bool>.FromValue(false),    false           ).SetName("{m}(All criteria specified, IsDeleted match does not exist)"),
            };

        [TestCaseSource(nameof(AnyAsync_TestCaseData))]
        public async Task AnyAsync_Always_ReturnsExpected(
            Optional<int> level,
            Optional<int> experienceThreshold,
            Optional<bool> isDeleted,
            bool expectedResult)
        {
            using var testContext = TestContext.CreateReadOnly();

            var uut = testContext.BuildUut();

            var result = await uut.AnyAsync(
                    level,
                    experienceThreshold,
                    isDeleted,
                    testContext.CancellationToken);

            result.ShouldBe(expectedResult);
        }

        #endregion AnyAsync() Tests

        #region AsyncEnumerateDefinitions() Tests

        public static IReadOnlyList<TestCaseData> AsyncEnumerateDefinitions_TestCaseData
            => new[]
            {
                /*                  isDeleted                           versionIds              */
                new TestCaseData(   Optional<bool>.Unspecified,         new[] { 1L, 5L, 9L }    ).SetName("{m}(All current versions)"),
                new TestCaseData(   Optional<bool>.FromValue(true),     new[] { 5L }            ).SetName("{m}(Deleted current versions)"),
                new TestCaseData(   Optional<bool>.FromValue(false),    new[] { 1L, 9L }        ).SetName("{m}(Undeleted current versions)")
            };

        [TestCaseSource(nameof(AsyncEnumerateDefinitions_TestCaseData))]
        public async Task AsyncEnumerateDefinitions_Always_ReturnsMatchingDefinitions(
            Optional<bool> isDeleted,
            IReadOnlyList<long> versionIds)
        {
            using var testContext = TestContext.CreateReadOnly();

            var uut = testContext.BuildUut();

            var results = await uut.AsyncEnumerateDefinitions(
                    isDeleted)
                .ToArrayAsync();

            var versionEntities = testContext.Entities.CharacterLevelDefinitionVersions;

            results.ShouldNotBeNull();
            results.ForEach(result => result.ShouldNotBeNull());
            results.Select(result => result.Level).ShouldBeSetEqualTo(versionIds.Select(x => versionEntities.First(y => y.Id == x).Level));
            foreach (var result in results)
            {
                var entity = versionEntities.First(y => (y.Level == result.Level) && versionIds.Contains(y.Id));

                result.ExperienceThreshold.ShouldBe(entity.ExperienceThreshold);
            }
            results.Select(result => result.Level).ShouldBeInOrder();

            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        #endregion AsyncEnumerateDefinitions() Tests

        #region MergeDefinitionAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> MergeDefinitionAsync_LevelDoesNotExist_TestCaseData
            = new[]
            {
                /*                  level,  experienceThreshold,    isDeleted,      actionId        */
                new TestCaseData(   4,      default(int),           default(bool),  default(long)   ).SetName("{m}(Default values)"),
                new TestCaseData(   4,      int.MinValue,           false,          long.MinValue   ).SetName("{m}(Min values)"),
                new TestCaseData(   4,      1,                      false,          2L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   4,      3,                      true,           4L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   4,      5,                      false,          6L              ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   4,      int.MaxValue,           true,           long.MaxValue   ).SetName("{m}(Max values)")
            };

        [TestCaseSource(nameof(MergeDefinitionAsync_LevelDoesNotExist_TestCaseData))]
        public async Task MergeDefinitionAsync_LevelDoesNotExist_ResultIsDataNotFound(
            int level,
            int experienceThreshold,
            bool isDeleted,
            long actionId)
        {
            using var testContext = TestContext.CreateReadWrite();

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

            var result = await uut.MergeDefinitionAsync(
                level,
                experienceThreshold,
                isDeleted,
                actionId,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory
                .ShouldHaveReceived(x => x.CreateScope(default));

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
            levelDefinitionVersionEntity.CreationId.ShouldBe(actionId);
            levelDefinitionVersionEntity.PreviousVersionId.ShouldBeNull();
            levelDefinitionVersionEntity.NextVersionId.ShouldBeNull();
            levelDefinitionVersionEntity.Definition.ShouldBeSameAs(levelDefinitionEntity);
        }

        public static IReadOnlyList<TestCaseData> MergeDefinitionAsync_NoChangesGiven_TestCaseData
            => new[]
            {
                /*                  level,  experienceThreshold,    isDeleted,  actionId    */
                new TestCaseData(   1,      0,                      false,      4L          ).SetName("{m}(Level 1, no differences)"),
                new TestCaseData(   2,      31,                     false,      5L          ).SetName("{m}(Level 2, no differences)"),
                new TestCaseData(   3,      20,                     true,       6L          ).SetName("{m}(Level 3, no differences)")
            };

        [TestCaseSource(nameof(MergeDefinitionAsync_NoChangesGiven_TestCaseData))]
        public async Task MergeDefinitionAsync_NoChangesAreNeeded_ResultIsNoChangesGiven(
            int level,
            int experienceThreshold,
            bool isDeleted,
            long actionId)
        {
            using var testContext = TestContext.CreateReadWrite();

            var uut = testContext.BuildUut();

            var result = await uut.MergeDefinitionAsync(
                level,
                experienceThreshold,
                isDeleted,
                actionId,
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

        public static IReadOnlyList<TestCaseData> MergeDefinitionAsync_ChangesGiven_TestCaseData
            => new[]
            {
                /*                  level,  experienceThreshold,    isDeleted,  actionId    */
                new TestCaseData(   1,      13,                     false,      10L         ).SetName("{m}(Level 1, ExperienceThreshold changed)"),
                new TestCaseData(   1,      12,                     true,       11L         ).SetName("{m}(Level 1, IsDeleted changed)"),
                new TestCaseData(   1,      14,                     true,       12L         ).SetName("{m}(Level 1, all properties changed)"),
                new TestCaseData(   2,      21,                     false,      13L         ).SetName("{m}(Level 2, ExperienceThreshold changed)"),
                new TestCaseData(   2,      20,                     true,       14L         ).SetName("{m}(Level 2, IsDeleted changed)"),
                new TestCaseData(   2,      22,                     true,       15L         ).SetName("{m}(Level 2, all properties changed)"),
                new TestCaseData(   3,      32,                     true,       16L         ).SetName("{m}(Level 3, ExperienceThreshold changed)"),
                new TestCaseData(   3,      31,                     false,      17L         ).SetName("{m}(Level 3, IsDeleted changed)"),
                new TestCaseData(   3,      33,                     false,      18L         ).SetName("{m}(Level 3, all properties changed)")
            };

        [TestCaseSource(nameof(MergeDefinitionAsync_ChangesGiven_TestCaseData))]
        public async Task MergeDefinitionAsync_ChangesAreNeeded_ResultIsSuccess(
            int level,
            int experienceThreshold,
            bool isDeleted,
            long actionId)
        {
            using var testContext = TestContext.CreateReadWrite();

            var previousVersionEntity = testContext.Entities.CharacterLevelDefinitionVersions
                .First(x => (x.Level == level) && (x.NextVersionId is null));

            var uut = testContext.BuildUut();

            var result = await uut.MergeDefinitionAsync(
                level,
                experienceThreshold,
                isDeleted,
                actionId,
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
            entity.ExperienceThreshold.ShouldBe(experienceThreshold);
            entity.IsDeleted.ShouldBe(isDeleted);

            previousVersionEntity.NextVersion.ShouldBeSameAs(entity);

            result.IsSuccess.ShouldBeTrue();
        }

        #endregion MergeDefinitionAsync() Tests
    }
}
