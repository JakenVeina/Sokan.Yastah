using System;
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
            public static TestContext CreateReadOnly()
                => new TestContext(
                    CharacterGuildsTestEntitySetBuilder.SharedSet);

            public static TestContext CreateReadWrite()
                => new TestContext(
                    CharacterGuildsTestEntitySetBuilder.NewSet());

            public TestContext(
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

            public CharacterGuildsRepository BuildUut()
                => new CharacterGuildsRepository(
                    MockContext.Object,
                    MockTransactionScopeFactory.Object);
        }

        #region AnyVersionsAsync() Tests

        internal static IReadOnlyList<TestCaseData> AnyVersionsAsync_Always_TestCaseData
            => new[]
            {
                /*                  guildId,                        excludedRoleIds,                                                        name,                                               isDeleted,                          isLatestVersion                     expectedResult  */
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.Unspecified,                       Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}()"),
                new TestCaseData(   Optional<long>.FromValue(1),    Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.Unspecified,                       Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(guildId: 1)"),
                new TestCaseData(   Optional<long>.FromValue(2),    Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.Unspecified,                       Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(guildId: 2)"),
                new TestCaseData(   Optional<long>.FromValue(3),    Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.Unspecified,                       Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(guildId: 3)"),
                new TestCaseData(   Optional<long>.FromValue(4),    Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.Unspecified,                       Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         false           ).SetName("{m}(guildId: 4)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(Array.Empty<long>()),             Optional<string>.Unspecified,                       Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedGuildIds: Empty)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] {     1L, 2L, 3L     }),    Optional<string>.Unspecified,                       Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         false           ).SetName("{m}(excludedGuildIds: 1, 2, 3)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] { 0L, 1L, 2L, 3L, 4L }),    Optional<string>.Unspecified,                       Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         false           ).SetName("{m}(excludedGuildIds: 0, 1, 2, 3, 4)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] { 0L                 }),    Optional<string>.Unspecified,                       Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedGuildIds: 0)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] {     1L             }),    Optional<string>.Unspecified,                       Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedGuildIds: 1)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] {         2L         }),    Optional<string>.Unspecified,                       Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedGuildIds: 2)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] {             3L     }),    Optional<string>.Unspecified,                       Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedGuildIds: 3)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] {                 4L }),    Optional<string>.Unspecified,                       Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedGuildIds: 4)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] {     1L,     3L,    }),    Optional<string>.Unspecified,                       Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedGuildIds: 1, 3)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] { 0L,     2L,     4L }),    Optional<string>.Unspecified,                       Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedGuildIds: 0, 2, 4)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Character Guild 1"),    Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(name: Character Guild 1)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Character Guild 1a"),   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(name: Character Guild 1)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Character Guild 2"),    Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(name: Character Guild 2)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Character Guild 2a"),   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         false           ).SetName("{m}(name: Character Guild 2a)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Character Guild 3"),    Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(name: Character Guild 3)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Character Guild 3a"),   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(name: Character Guild 3a)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Character Guild 3b"),   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         false           ).SetName("{m}(name: Character Guild 3b)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Character Guild 4"),    Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         false           ).SetName("{m}(name: Character Guild 4)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.Unspecified,                       Optional<bool>.FromValue(true),     Optional<bool>.Unspecified,         true            ).SetName("{m}(isDeleted: true)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.Unspecified,                       Optional<bool>.FromValue(false),    Optional<bool>.Unspecified,         true            ).SetName("{m}(isDeleted: false)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.Unspecified,                       Optional<bool>.Unspecified,         Optional<bool>.FromValue(true),     true            ).SetName("{m}(isLatestVersion: true)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.Unspecified,                       Optional<bool>.Unspecified,         Optional<bool>.FromValue(false),    true            ).SetName("{m}(isLatestVersion: false)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Character Guild 2"),    Optional<bool>.FromValue(true),     Optional<bool>.Unspecified,         true            ).SetName("{m}(Guild has deleted version)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Character Guild 1"),    Optional<bool>.FromValue(true),     Optional<bool>.Unspecified,         false           ).SetName("{m}(Guild does not have deleted version)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Character Guild 1"),    Optional<bool>.FromValue(false),    Optional<bool>.Unspecified,         true            ).SetName("{m}(Guild has undeleted version)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Character Guild 1"),    Optional<bool>.Unspecified,         Optional<bool>.FromValue(true),     true            ).SetName("{m}(Guild has current version)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Character Guild 3"),    Optional<bool>.Unspecified,         Optional<bool>.FromValue(true),     false           ).SetName("{m}(Guild does not have current version)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Character Guild 1a"),   Optional<bool>.Unspecified,         Optional<bool>.FromValue(false),    true            ).SetName("{m}(Guild has prior versions)"),
                new TestCaseData(   Optional<long>.FromValue(0),    Optional<IEnumerable<long>>.FromValue(new[] { 0L }),                    Optional<string>.FromValue("name"),                 Optional<bool>.FromValue(true),     Optional<bool>.FromValue(false),    false           ).SetName("{m}(All criteria specified)")
            };

        [TestCaseSource(nameof(AnyVersionsAsync_Always_TestCaseData))]
        public async Task AnyVersionsAsync_Always_ResultIsExpected(
            Optional<long> guildId,
            Optional<IEnumerable<long>> excludedGuildIds,
            Optional<string> name,
            Optional<bool> isDeleted,
            Optional<bool> isLatestVersion,
            bool expectedResult)
        {
            using var testContext = TestContext.CreateReadOnly();

            var uut = testContext.BuildUut();

            var result = await uut.AnyVersionsAsync(
                guildId,
                excludedGuildIds,
                name,
                isDeleted,
                isLatestVersion,
                testContext.CancellationToken);

            result.ShouldBe(expectedResult);

            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        #endregion AnyVersionsAsync() Tests

        #region AsyncEnumerateIdentities() Tests

        public static IReadOnlyList<TestCaseData> AsyncEnumerateIdentities_TestCaseData
            => new[]
            {
                /*                  isDeleted                           versionIds              */
                new TestCaseData(   Optional<bool>.Unspecified,         new[] { 5L, 8L, 9L }    ).SetName("{m}(All current versions)"),
                new TestCaseData(   Optional<bool>.FromValue(true),     new[] { 5L }            ).SetName("{m}(Deleted current versions)"),
                new TestCaseData(   Optional<bool>.FromValue(false),    new[] { 8L, 9L }        ).SetName("{m}(Undeleted current versions)")
            };

        [TestCaseSource(nameof(AsyncEnumerateIdentities_TestCaseData))]
        public async Task AsyncEnumerateIdentities_Always_ReturnsMatchingIdentities(
            Optional<bool> isDeleted,
            IReadOnlyList<long> versionIds)
        {
            using var testContext = TestContext.CreateReadOnly();

            var uut = testContext.BuildUut();

            var results = await uut.AsyncEnumerateIdentities(
                    isDeleted)
                .ToArrayAsync();

            var versionEntities = testContext.Entities.CharacterGuildVersions;

            results.ShouldNotBeNull();
            results.ForEach(result => result.ShouldNotBeNull());
            results.Select(x => x?.Id).ShouldBeSetEqualTo(versionIds.Select(x => (long?)versionEntities.First(y => y.Id == x).GuildId));
            foreach (var result in results)
            {
                var entity = versionEntities.First(y => (y.GuildId == result.Id) && versionIds.Contains(y.Id));

                result.Name.ShouldBe(result.Name);
            }

            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        #endregion AsyncEnumerateIdentities() Tests

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
            using var testContext = TestContext.CreateReadOnly();

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
            using var testContext = TestContext.CreateReadWrite();

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
            using var testContext = TestContext.CreateReadWrite();

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
            using var testContext = TestContext.CreateReadWrite();

            testContext.MockContext
                .Setup(x => x.AddAsync(It.IsNotNull<CharacterGuildVersionEntity>(), It.IsAny<CancellationToken>()))
                .Callback<CharacterGuildVersionEntity, CancellationToken>((x, y) => x.Id = versionId);

            var previousVersionEntity = testContext.Entities.CharacterGuildVersions
                .First(x => (x.GuildId == guildId) && (x.NextVersionId is null));

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
    }
}
