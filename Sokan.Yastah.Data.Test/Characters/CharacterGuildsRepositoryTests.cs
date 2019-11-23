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
    public class CharacterGuildsRepositoryTests
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

            public CharacterGuildsRepository BuildUut()
                => new CharacterGuildsRepository(
                    MockContext.Object,
                    MockTransactionScopeFactory.Object);
        }

        #region CreateAsync() Tests

        public static IReadOnlyList<TestCaseData> CreateAsync_TestCaseData
            => new[]
            {
                /*                  name            creationId,     guildId         */
                new TestCaseData(   string.Empty,   default(long),  default(long)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   string.Empty,   long.MinValue,  long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   "name 1",       2L,             3L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   "name 4",       5L,             6L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   "name 7",       8L,             9L              ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   "MaxValue",     long.MaxValue,  long.MaxValue   ).SetName("{m}(Max Values)"),
            };

        [TestCaseSource(nameof(CreateAsync_TestCaseData))]
        public async Task CreateAsync_Always_CreatesNewEntities(
            string name,
            long creationId,
            long guildId)
        {
            using var testContext = new TestContext(isReadOnly: false);

            var guildEntity = null as CharacterGuildEntity;
            var guildVersionEntity = null as CharacterGuildVersionEntity;

            testContext.MockContext
                .Setup(x => x.AddAsync(It.IsAny<CharacterGuildEntity>(), It.IsAny<CancellationToken>()))
                .Callback<CharacterGuildEntity, CancellationToken>((x, y) => guildEntity = x);
            testContext.MockContext
                .Setup(x => x.AddAsync(It.IsAny<CharacterGuildVersionEntity>(), It.IsAny<CancellationToken>()))
                .Callback<CharacterGuildVersionEntity, CancellationToken>((x, y) => guildVersionEntity = x);

            testContext.MockContext
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback(() =>
                {
                    if (guildEntity is { })
                    {
                        guildEntity.Id = guildId;
                        if (guildVersionEntity is { })
                            guildVersionEntity.Guild = guildEntity;
                    }
                });

            var uut = testContext.BuildUut();

            var result = await uut.CreateAsync(
                name,
                creationId,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockContext
                .ShouldHaveReceived(x => x.AddAsync(It.IsNotNull<CharacterGuildEntity>(), testContext.CancellationToken));
            testContext.MockContext
                .ShouldHaveReceived(x => x.AddAsync(It.IsNotNull<CharacterGuildVersionEntity>(), testContext.CancellationToken));
            testContext.MockContext
                .ShouldHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken), Times.Exactly(2));

            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            guildEntity!.Id.ShouldBe(guildId);

            guildVersionEntity!.Name.ShouldBe(name);
            guildVersionEntity.CreationId.ShouldBe(creationId);
            guildVersionEntity.PreviousVersionId.ShouldBeNull();
            guildVersionEntity.NextVersionId.ShouldBeNull();
            guildVersionEntity.GuildId.ShouldBe(guildId);
            guildVersionEntity.Guild.ShouldBeSameAs(guildEntity);

            result.ShouldBe(guildId);
        }

        #endregion CreateAsync() Tests

        #region CreateDivisionAsync() Tests

        public static IReadOnlyList<TestCaseData> CreateDivisionAsync_TestCaseData
            => new[]
            {
                /*                  name            guildId,        creationId,     divisionId      */
                new TestCaseData(   string.Empty,   default(long),  default(long),  default(long)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   string.Empty,   long.MinValue,  long.MinValue,  long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   "name 1",       2L,             3L,             4L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   "name 5",       6L,             7L,             8L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   "name 9",       10L,            11L,            12L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   "MaxValue",     long.MaxValue,  long.MaxValue,  long.MaxValue   ).SetName("{m}(Max Values)"),
            };

        [TestCaseSource(nameof(CreateDivisionAsync_TestCaseData))]
        public async Task CreateDivisionAsync_Always_CreatesNewEntities(
            string name,
            long guildId,
            long creationId,
            long divisionId)
        {
            using var testContext = new TestContext(isReadOnly: false);

            var guildDivisionEntity = null as CharacterGuildDivisionEntity;
            var guildDivisionVersionEntity = null as CharacterGuildDivisionVersionEntity;

            testContext.MockContext
                .Setup(x => x.AddAsync(It.IsAny<CharacterGuildDivisionEntity>(), It.IsAny<CancellationToken>()))
                .Callback<CharacterGuildDivisionEntity, CancellationToken>((x, y) => guildDivisionEntity = x);
            testContext.MockContext
                .Setup(x => x.AddAsync(It.IsAny<CharacterGuildDivisionVersionEntity>(), It.IsAny<CancellationToken>()))
                .Callback<CharacterGuildDivisionVersionEntity, CancellationToken>((x, y) => guildDivisionVersionEntity = x);

            testContext.MockContext
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback(() =>
                {
                    if (guildDivisionEntity is { })
                    {
                        guildDivisionEntity.Id = divisionId;
                        if (guildDivisionVersionEntity is { })
                            guildDivisionVersionEntity.Division = guildDivisionEntity;
                    }
                });

            var uut = testContext.BuildUut();

            var result = await uut.CreateDivisionAsync(
                guildId,
                name,
                creationId,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockContext
                .ShouldHaveReceived(x => x.AddAsync(It.IsNotNull<CharacterGuildDivisionEntity>(), testContext.CancellationToken));
            testContext.MockContext
                .ShouldHaveReceived(x => x.AddAsync(It.IsNotNull<CharacterGuildDivisionVersionEntity>(), testContext.CancellationToken));
            testContext.MockContext
                .ShouldHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken), Times.Exactly(2));

            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            guildDivisionEntity!.Id.ShouldBe(divisionId);

            guildDivisionVersionEntity!.Name.ShouldBe(name);
            guildDivisionVersionEntity.CreationId.ShouldBe(creationId);
            guildDivisionVersionEntity.PreviousVersionId.ShouldBeNull();
            guildDivisionVersionEntity.NextVersionId.ShouldBeNull();
            guildDivisionVersionEntity.DivisionId.ShouldBe(divisionId);
            guildDivisionVersionEntity.Division.ShouldBeSameAs(guildDivisionEntity);

            result.ShouldBe(divisionId);
        }

        #endregion CreateDivisionAsync() Tests

        #region UpdateAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> UpdateAsync_GuildIdDoesNotExist_TestCaseData
            = new[]
            {
                /*                  guildId,    actionId        name                                                    isDeleted                       */
                new TestCaseData(   4L,         default(long),  default(Optional<string>),                              default(Optional<bool>)         ).SetName("{m}(Default values)"),
                new TestCaseData(   4L,         long.MinValue,  Optional<string>.FromValue(string.Empty),               Optional<bool>.FromValue(false) ).SetName("{m}(Min values)"),
                new TestCaseData(   4L,         1L,             Optional<string>.FromValue("Bogus Character Guild 1"),  Optional<bool>.FromValue(false) ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   4L,         2L,             Optional<string>.FromValue("Bogus Character Guild 2"),  Optional<bool>.FromValue(true)  ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   4L,         3L,             Optional<string>.FromValue("Bogus Character Guild 3"),  Optional<bool>.FromValue(false) ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   4L,         long.MaxValue,  Optional<string>.FromValue("Bogus Character Guild 4"),  Optional<bool>.FromValue(true)  ).SetName("{m}(Max values)"),
            };

        [TestCaseSource(nameof(UpdateAsync_GuildIdDoesNotExist_TestCaseData))]
        public async Task UpdateAsync_GuildIdDoesNotExist_ResultIsDataNotFound(
            long guildId,
            long actionId,
            Optional<string> name,
            Optional<bool> isDeleted)
        {
            using var testContext = new TestContext(isReadOnly: false);

            var uut = testContext.BuildUut();

            var result = await uut.UpdateAsync(
                guildId,
                actionId,
                name,
                isDeleted,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory
                .ShouldHaveReceived(x => x.CreateScope(default));

            testContext.MockContext
                .ShouldNotHaveReceived(x => x.AddAsync(It.IsAny<CharacterGuildVersionEntity>(), It.IsAny<CancellationToken>()));
            testContext.MockContext
                .ShouldNotHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));

            testContext.MockTransactionScope
                .ShouldNotHaveReceived(x => x.Complete());
            testContext.MockTransactionScope
                .ShouldHaveReceived(x => x.Dispose());

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeOfType<DataNotFoundError>();
            result.Error.Message.ShouldContain(guildId.ToString());
        }

        public static IReadOnlyList<TestCaseData> UpdateAsync_NoChangesGiven_TestCaseData
            => new[]
            {
                /*                  guildId,    actionId,   name,                                               isDeleted                       */
                new TestCaseData(   1L,         4L,         Optional<string>.Unspecified,                       Optional<bool>.Unspecified      ).SetName("{m}(Guild 1, no changes)"),
                new TestCaseData(   1L,         5L,         Optional<string>.FromValue("Character Guild 1"),    Optional<bool>.FromValue(false) ).SetName("{m}(Guild 1, no differences)"),
                new TestCaseData(   2L,         6L,         Optional<string>.Unspecified,                       Optional<bool>.Unspecified      ).SetName("{m}(Guild 2, no changes)"),
                new TestCaseData(   2L,         7L,         Optional<string>.FromValue("Character Guild 2"),    Optional<bool>.FromValue(true)  ).SetName("{m}(Guild 2, no differences)"),
                new TestCaseData(   3L,         8L,         Optional<string>.Unspecified,                       Optional<bool>.Unspecified      ).SetName("{m}(Guild 3, no changes)"),
                new TestCaseData(   3L,         9L,         Optional<string>.FromValue("Character Guild 3a"),   Optional<bool>.FromValue(false) ).SetName("{m}(Guild 3, no differences)")
            };

        [TestCaseSource(nameof(UpdateAsync_NoChangesGiven_TestCaseData))]
        public async Task UpdateAsync_NoChangesAreNeeded_ResultIsNoChangesGiven(
            long guildId,
            long actionId,
            Optional<string> name,
            Optional<bool> isDeleted)
        {
            using var testContext = new TestContext(isReadOnly: false);

            var uut = testContext.BuildUut();

            var result = await uut.UpdateAsync(
                guildId,
                actionId,
                name,
                isDeleted,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory
                .ShouldHaveReceived(x => x.CreateScope(default));

            testContext.MockContext
                .ShouldNotHaveReceived(x => x.AddAsync(It.IsAny<CharacterGuildVersionEntity>(), It.IsAny<CancellationToken>()));
            testContext.MockContext
                .ShouldNotHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));

            testContext.MockTransactionScope
                .ShouldHaveReceived(x => x.Complete());
            testContext.MockTransactionScope
                .ShouldHaveReceived(x => x.Dispose());

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeOfType<NoChangesGivenError>();
            result.Error.Message.ShouldContain(guildId.ToString());
        }

        public static IReadOnlyList<TestCaseData> UpdateAsync_ChangesGiven_TestCaseData
            => new[]
            {
                /*                  guildId,    actionId,   name,                                               isDeleted,                          versionId   */
                new TestCaseData(   1L,         10L,        Optional<string>.FromValue("Character Guild 1a"),   Optional<bool>.Unspecified,         19L         ).SetName("{m}(Guild 1, Name changed)"),
                new TestCaseData(   1L,         11L,        Optional<string>.Unspecified,                       Optional<bool>.FromValue(true),     20L         ).SetName("{m}(Guild 1, IsDeleted changed)"),
                new TestCaseData(   1L,         12L,        Optional<string>.FromValue("Character Guild 1b"),   Optional<bool>.FromValue(true),     21L         ).SetName("{m}(Guild 1, all properties changed)"),
                new TestCaseData(   2L,         13L,        Optional<string>.FromValue("Character Guild 2a"),   Optional<bool>.Unspecified,         22L         ).SetName("{m}(Guild 2, Name changed)"),
                new TestCaseData(   2L,         14L,        Optional<string>.Unspecified,                       Optional<bool>.FromValue(false),    23L         ).SetName("{m}(Guild 2, IsDeleted changed)"),
                new TestCaseData(   2L,         15L,        Optional<string>.FromValue("Character Guild 2b"),   Optional<bool>.FromValue(false),    24L         ).SetName("{m}(Guild 2, all properties changed)"),
                new TestCaseData(   3L,         16L,        Optional<string>.FromValue("Character Guild 3b"),   Optional<bool>.Unspecified,         25L         ).SetName("{m}(Guild 3, Name changed)"),
                new TestCaseData(   3L,         17L,        Optional<string>.Unspecified,                       Optional<bool>.FromValue(true),     26L         ).SetName("{m}(Guild 3, IsDeleted changed)"),
                new TestCaseData(   3L,         18L,        Optional<string>.FromValue("Character Guild 3c"),   Optional<bool>.FromValue(true),     27L         ).SetName("{m}(Guild 3, all properties changed)"),
            };

        [TestCaseSource(nameof(UpdateAsync_ChangesGiven_TestCaseData))]
        public async Task UpdateAsync_Otherwise_ResultIsSuccess(
            long guildId,
            long actionId,
            Optional<string> name,
            Optional<bool> isDeleted,
            long versionId)
        {
            using var testContext = new TestContext(isReadOnly: false);

            testContext.MockContext
                .Setup(x => x.AddAsync(It.IsNotNull<CharacterGuildVersionEntity>(), It.IsAny<CancellationToken>()))
                .Callback<CharacterGuildVersionEntity, CancellationToken>((x, y) => x.Id = versionId);

            var previousVersionEntity = testContext.Entities.CharacterGuildVersions.First(x => (x.GuildId == guildId) && (x.NextVersionId is null));

            var uut = testContext.BuildUut();

            var result = await uut.UpdateAsync(
                guildId,
                actionId,
                name,
                isDeleted,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory
                .ShouldHaveReceived(x => x.CreateScope(default));

            testContext.MockContext
                .ShouldHaveReceived(x => x.AddAsync(It.IsAny<CharacterGuildVersionEntity>(), testContext.CancellationToken));
            testContext.MockContext
                .ShouldHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));

            testContext.MockTransactionScope
                .ShouldHaveReceived(x => x.Complete());
            testContext.MockTransactionScope
                .ShouldHaveReceived(x => x.Dispose());

            var entity = testContext.MockContext
                .Invocations
                .Where(x => x.Method.Name == nameof(YastahDbContext.AddAsync))
                .Select(x => (CharacterGuildVersionEntity)x.Arguments[0])
                .First();

            entity.ShouldNotBeNull();
            entity.GuildId.ShouldBe(guildId);
            entity.CreationId.ShouldBe(actionId);
            entity.NextVersionId.ShouldBeNull();
            entity.PreviousVersionId.ShouldBe(previousVersionEntity.Id);
            entity.Name.ShouldBe(name.IsSpecified ? name.Value : previousVersionEntity.Name);
            entity.IsDeleted.ShouldBe(isDeleted.IsSpecified ? isDeleted.Value : previousVersionEntity.IsDeleted);

            previousVersionEntity.NextVersion.ShouldBeSameAs(entity);

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(versionId);
        }

        #endregion UpdateAsync() Tests

        #region UpdateDivisionAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> UpdateDivisionAsync_DivisionIdDoesNotExist_TestCaseData
            = new[]
            {
                /*                  divisionId, actionId        name                                                            isDeleted                       */
                new TestCaseData(   10L,        default(long),  default(Optional<string>),                                      default(Optional<bool>)         ).SetName("{m}(Default values)"),
                new TestCaseData(   10L,        long.MinValue,  Optional<string>.FromValue(string.Empty),                       Optional<bool>.FromValue(false) ).SetName("{m}(Min values)"),
                new TestCaseData(   10L,        1L,             Optional<string>.FromValue("Bogus Character Guild Division 1"), Optional<bool>.FromValue(false) ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   10L,        2L,             Optional<string>.FromValue("Bogus Character Guild Division 2"), Optional<bool>.FromValue(true)  ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   10L,        3L,             Optional<string>.FromValue("Bogus Character Guild Division 3"), Optional<bool>.FromValue(false) ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   10L,        long.MaxValue,  Optional<string>.FromValue("Bogus Character Guild Division 4"), Optional<bool>.FromValue(true)  ).SetName("{m}(Max values)"),
            };

        [TestCaseSource(nameof(UpdateDivisionAsync_DivisionIdDoesNotExist_TestCaseData))]
        public async Task UpdateDivisionAsync_DivisionIdDoesNotExist_ResultIsDataNotFound(
            long divisionId,
            long actionId,
            Optional<string> name,
            Optional<bool> isDeleted)
        {
            using var testContext = new TestContext(isReadOnly: false);

            var uut = testContext.BuildUut();

            var result = await uut.UpdateDivisionAsync(
                divisionId,
                actionId,
                name,
                isDeleted,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory
                .ShouldHaveReceived(x => x.CreateScope(default));

            testContext.MockContext
                .ShouldNotHaveReceived(x => x.AddAsync(It.IsAny<CharacterGuildDivisionVersionEntity>(), It.IsAny<CancellationToken>()));
            testContext.MockContext
                .ShouldNotHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));

            testContext.MockTransactionScope
                .ShouldNotHaveReceived(x => x.Complete());
            testContext.MockTransactionScope
                .ShouldHaveReceived(x => x.Dispose());

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeOfType<DataNotFoundError>();
            result.Error.Message.ShouldContain(divisionId.ToString());
        }

        public static IReadOnlyList<TestCaseData> UpdateDivisionAsync_NoChangesGiven_TestCaseData
            => new[]
            {
                /*                  divisionId, actionId,   name,                                                           isDeleted                       */
                new TestCaseData(   1L,         10L,        Optional<string>.Unspecified,                                   Optional<bool>.Unspecified      ).SetName("{m}(Division 1, no changes)"),
                new TestCaseData(   1L,         11L,        Optional<string>.FromValue("Character Guild 1, Division 1a"),   Optional<bool>.FromValue(false) ).SetName("{m}(Division 1, no differences)"),
                new TestCaseData(   2L,         12L,        Optional<string>.Unspecified,                                   Optional<bool>.Unspecified      ).SetName("{m}(Division 2, no changes)"),
                new TestCaseData(   2L,         13L,        Optional<string>.FromValue("Character Guild 2, Division 1"),    Optional<bool>.FromValue(false) ).SetName("{m}(Division 2, no differences)"),
                new TestCaseData(   3L,         14L,        Optional<string>.Unspecified,                                   Optional<bool>.Unspecified      ).SetName("{m}(Division 3, no changes)"),
                new TestCaseData(   3L,         15L,        Optional<string>.FromValue("Character Guild 2, Division 2"),    Optional<bool>.FromValue(false) ).SetName("{m}(Division 3, no differences)"),
                new TestCaseData(   4L,         16L,        Optional<string>.Unspecified,                                   Optional<bool>.Unspecified      ).SetName("{m}(Division 4, no changes)"),
                new TestCaseData(   4L,         17L,        Optional<string>.FromValue("Character Guild 1, Division 2"),    Optional<bool>.FromValue(true)  ).SetName("{m}(Division 4, no differences)"),
                new TestCaseData(   5L,         18L,        Optional<string>.Unspecified,                                   Optional<bool>.Unspecified      ).SetName("{m}(Division 5, no changes)"),
                new TestCaseData(   5L,         19L,        Optional<string>.FromValue("Character Guild 3, Division 1a"),   Optional<bool>.FromValue(false) ).SetName("{m}(Division 5, no differences)"),
                new TestCaseData(   6L,         20L,        Optional<string>.Unspecified,                                   Optional<bool>.Unspecified      ).SetName("{m}(Division 6, no changes)"),
                new TestCaseData(   6L,         21L,        Optional<string>.FromValue("Character Guild 2, Division 3"),    Optional<bool>.FromValue(false) ).SetName("{m}(Division 6, no differences)"),
                new TestCaseData(   7L,         22L,        Optional<string>.Unspecified,                                   Optional<bool>.Unspecified      ).SetName("{m}(Division 7, no changes)"),
                new TestCaseData(   7L,         23L,        Optional<string>.FromValue("Character Guild 3, Division 2"),    Optional<bool>.FromValue(false) ).SetName("{m}(Division 7, no differences)"),
                new TestCaseData(   8L,         24L,        Optional<string>.Unspecified,                                   Optional<bool>.Unspecified      ).SetName("{m}(Division 8, no changes)"),
                new TestCaseData(   8L,         25L,        Optional<string>.FromValue("Character Guild 1, Division 3"),    Optional<bool>.FromValue(false) ).SetName("{m}(Division 8, no differences)"),
                new TestCaseData(   9L,         26L,        Optional<string>.Unspecified,                                   Optional<bool>.Unspecified      ).SetName("{m}(Division 9, no changes)"),
                new TestCaseData(   9L,         27L,        Optional<string>.FromValue("Character Guild 3, Division 3b"),   Optional<bool>.FromValue(false) ).SetName("{m}(Division 9, no differences)")
            };

        [TestCaseSource(nameof(UpdateDivisionAsync_NoChangesGiven_TestCaseData))]
        public async Task UpdateDivisionAsync_NoChangesAreNeeded_ResultIsNoChangesGiven(
            long divisionId,
            long actionId,
            Optional<string> name,
            Optional<bool> isDeleted)
        {
            using var testContext = new TestContext(isReadOnly: false);

            var uut = testContext.BuildUut();

            var result = await uut.UpdateDivisionAsync(
                divisionId,
                actionId,
                name,
                isDeleted,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory
                .ShouldHaveReceived(x => x.CreateScope(default));

            testContext.MockContext
                .ShouldNotHaveReceived(x => x.AddAsync(It.IsAny<CharacterGuildDivisionVersionEntity>(), It.IsAny<CancellationToken>()));
            testContext.MockContext
                .ShouldNotHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));

            testContext.MockTransactionScope
                .ShouldHaveReceived(x => x.Complete());
            testContext.MockTransactionScope
                .ShouldHaveReceived(x => x.Dispose());

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeOfType<NoChangesGivenError>();
            result.Error.Message.ShouldContain(divisionId.ToString());
        }

        public static IReadOnlyList<TestCaseData> UpdateDivisionAsync_ChangesGiven_TestCaseData
            => new[]
            {
                /*                  guildId,    actionId,   name,                                                           isDeleted,                          versionId   */
                new TestCaseData(   1L,         28L,        Optional<string>.FromValue("Character Guild 1, Division 1b"),   Optional<bool>.Unspecified,         55L         ).SetName("{m}(Division 1, Name changed)"),
                new TestCaseData(   1L,         29L,        Optional<string>.Unspecified,                                   Optional<bool>.FromValue(true),     56L         ).SetName("{m}(Division 1, IsDeleted changed)"),
                new TestCaseData(   1L,         30L,        Optional<string>.FromValue("Character Guild 1, Division 1c"),   Optional<bool>.FromValue(true),     57L         ).SetName("{m}(Division 1, all properties changed)"),
                new TestCaseData(   2L,         31L,        Optional<string>.FromValue("Character Guild 2, Division 1a"),   Optional<bool>.Unspecified,         58L         ).SetName("{m}(Division 2, Name changed)"),
                new TestCaseData(   2L,         32L,        Optional<string>.Unspecified,                                   Optional<bool>.FromValue(true),     59L         ).SetName("{m}(Division 2, IsDeleted changed)"),
                new TestCaseData(   2L,         33L,        Optional<string>.FromValue("Character Guild 2, Division 1b"),   Optional<bool>.FromValue(true),     60L         ).SetName("{m}(Division 2, all properties changed)"),
                new TestCaseData(   3L,         34L,        Optional<string>.FromValue("Character Guild 2, Division 2a"),   Optional<bool>.Unspecified,         61L         ).SetName("{m}(Division 3, Name changed)"),
                new TestCaseData(   3L,         35L,        Optional<string>.Unspecified,                                   Optional<bool>.FromValue(true),     62L         ).SetName("{m}(Division 3, IsDeleted changed)"),
                new TestCaseData(   3L,         36L,        Optional<string>.FromValue("Character Guild 2, Division 2b"),   Optional<bool>.FromValue(true),     63L         ).SetName("{m}(Division 3, all properties changed)"),
                new TestCaseData(   4L,         37L,        Optional<string>.FromValue("Character Guild 1, Division 2a"),   Optional<bool>.Unspecified,         64L         ).SetName("{m}(Division 4, Name changed)"),
                new TestCaseData(   4L,         38L,        Optional<string>.Unspecified,                                   Optional<bool>.FromValue(false),    65L         ).SetName("{m}(Division 4, IsDeleted changed)"),
                new TestCaseData(   4L,         39L,        Optional<string>.FromValue("Character Guild 1, Division 2b"),   Optional<bool>.FromValue(false),    66L         ).SetName("{m}(Division 4, all properties changed)"),
                new TestCaseData(   5L,         30L,        Optional<string>.FromValue("Character Guild 3, Division 1b"),   Optional<bool>.Unspecified,         67L         ).SetName("{m}(Division 5, Name changed)"),
                new TestCaseData(   5L,         41L,        Optional<string>.Unspecified,                                   Optional<bool>.FromValue(true),     68L         ).SetName("{m}(Division 5, IsDeleted changed)"),
                new TestCaseData(   5L,         42L,        Optional<string>.FromValue("Character Guild 3, Division 1c"),   Optional<bool>.FromValue(true),     69L         ).SetName("{m}(Division 5, all properties changed)"),
                new TestCaseData(   6L,         43L,        Optional<string>.FromValue("Character Guild 2, Division 3a"),   Optional<bool>.Unspecified,         70L         ).SetName("{m}(Division 6, Name changed)"),
                new TestCaseData(   6L,         44L,        Optional<string>.Unspecified,                                   Optional<bool>.FromValue(true),     71L         ).SetName("{m}(Division 6, IsDeleted changed)"),
                new TestCaseData(   6L,         45L,        Optional<string>.FromValue("Character Guild 2, Division 3b"),   Optional<bool>.FromValue(true),     72L         ).SetName("{m}(Division 6, all properties changed)"),
                new TestCaseData(   7L,         46L,        Optional<string>.FromValue("Character Guild 3, Division 2a"),   Optional<bool>.Unspecified,         73L         ).SetName("{m}(Division 7, Name changed)"),
                new TestCaseData(   7L,         47L,        Optional<string>.Unspecified,                                   Optional<bool>.FromValue(true),     74L         ).SetName("{m}(Division 7, IsDeleted changed)"),
                new TestCaseData(   7L,         48L,        Optional<string>.FromValue("Character Guild 3, Division 2b"),   Optional<bool>.FromValue(true),     75L         ).SetName("{m}(Division 7, all properties changed)"),
                new TestCaseData(   8L,         49L,        Optional<string>.FromValue("Character Guild 1, Division 3a"),   Optional<bool>.Unspecified,         76L         ).SetName("{m}(Division 8, Name changed)"),
                new TestCaseData(   8L,         40L,        Optional<string>.Unspecified,                                   Optional<bool>.FromValue(true),     77L         ).SetName("{m}(Division 8, IsDeleted changed)"),
                new TestCaseData(   8L,         51L,        Optional<string>.FromValue("Character Guild 1, Division 3b"),   Optional<bool>.FromValue(true),     78L         ).SetName("{m}(Division 8, all properties changed)"),
                new TestCaseData(   9L,         52L,        Optional<string>.FromValue("Character Guild 3, Division 3c"),   Optional<bool>.Unspecified,         79L         ).SetName("{m}(Division 9, Name changed)"),
                new TestCaseData(   9L,         53L,        Optional<string>.Unspecified,                                   Optional<bool>.FromValue(true),     80L         ).SetName("{m}(Division 9, IsDeleted changed)"),
                new TestCaseData(   9L,         54L,        Optional<string>.FromValue("Character Guild 3, Division 3d"),   Optional<bool>.FromValue(true),     81L         ).SetName("{m}(Division 9, all properties changed)"),
            };

        [TestCaseSource(nameof(UpdateDivisionAsync_ChangesGiven_TestCaseData))]
        public async Task UpdateDivisionAsync_Otherwise_ResultIsSuccess(
            long divisionId,
            long actionId,
            Optional<string> name,
            Optional<bool> isDeleted,
            long versionId)
        {
            using var testContext = new TestContext(isReadOnly: false);

            testContext.MockContext
                .Setup(x => x.AddAsync(It.IsNotNull<CharacterGuildDivisionVersionEntity>(), It.IsAny<CancellationToken>()))
                .Callback<CharacterGuildDivisionVersionEntity, CancellationToken>((x, y) => x.Id = versionId);

            var previousVersionEntity = testContext.Entities.CharacterGuildDivisionVersions.First(x => (x.DivisionId == divisionId) && (x.NextVersionId is null));

            var uut = testContext.BuildUut();

            var result = await uut.UpdateDivisionAsync(
                divisionId,
                actionId,
                name,
                isDeleted,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory
                .ShouldHaveReceived(x => x.CreateScope(default));

            testContext.MockContext
                .ShouldHaveReceived(x => x.AddAsync(It.IsAny<CharacterGuildDivisionVersionEntity>(), testContext.CancellationToken));
            testContext.MockContext
                .ShouldHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));

            testContext.MockTransactionScope
                .ShouldHaveReceived(x => x.Complete());
            testContext.MockTransactionScope
                .ShouldHaveReceived(x => x.Dispose());

            var entity = testContext.MockContext
                .Invocations
                .Where(x => x.Method.Name == nameof(YastahDbContext.AddAsync))
                .Select(x => (CharacterGuildDivisionVersionEntity)x.Arguments[0])
                .First();

            entity.ShouldNotBeNull();
            entity.DivisionId.ShouldBe(divisionId);
            entity.CreationId.ShouldBe(actionId);
            entity.NextVersionId.ShouldBeNull();
            entity.PreviousVersionId.ShouldBe(previousVersionEntity.Id);
            entity.Name.ShouldBe(name.IsSpecified ? name.Value : previousVersionEntity.Name);
            entity.IsDeleted.ShouldBe(isDeleted.IsSpecified ? isDeleted.Value : previousVersionEntity.IsDeleted);

            previousVersionEntity.NextVersion.ShouldBeSameAs(entity);

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(versionId);
        }

        #endregion UpdateDivisionAsync() Tests
    }
}
