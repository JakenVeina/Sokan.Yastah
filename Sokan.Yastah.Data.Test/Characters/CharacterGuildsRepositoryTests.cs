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

        #region AnyDivisionVersionsAsync() Tests

        internal static IReadOnlyList<TestCaseData> AnyDivisionVersionsAsync_TestCaseData
            => new[]
            {
                /*                  guildId,                        excludedDivisionIds,                                                                            name,                                                           isDeleted,                          isLatestVersion                     expectedResult  */
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}()"),
                new TestCaseData(   Optional<long>.FromValue(1),    Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(guildId: 1)"),
                new TestCaseData(   Optional<long>.FromValue(2),    Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(guildId: 2)"),
                new TestCaseData(   Optional<long>.FromValue(3),    Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(guildId: 3)"),
                new TestCaseData(   Optional<long>.FromValue(4),    Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         false           ).SetName("{m}(guildId: 4)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(Array.Empty<long>()),                                     Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: Empty)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] {     1L, 2L, 3L, 4L, 5L, 6L, 7L, 8L, 9L      }),   Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         false           ).SetName("{m}(excludedRoleIds: 1, 2, 3, 4, 5, 6, 7, 8, 9)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] { 0L, 1L, 2L, 3L, 4L, 5L, 6L, 7L, 8L, 9L, 10L }),   Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         false           ).SetName("{m}(excludedRoleIds: 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] { 0L                                          }),   Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: 0)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] {     1L                                      }),   Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: 1)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] {         2L                                  }),   Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: 2)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] {             3L                              }),   Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: 3)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] {                 4L                          }),   Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: 4)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] {                     5L                      }),   Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: 5)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] {                         6L                  }),   Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: 6)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] {                             7L              }),   Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: 7)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] {                                 8L          }),   Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: 8)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] {                                     9L      }),   Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: 9)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] {                                         10L }),   Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: 10)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] {     1L,     3L,     5L,     7L,     9L      }),   Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: 1, 3, 5, 7, 9)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.FromValue(new[] { 0L,     2L,     4L,     6L,     8L,     10L }),   Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: 0, 2, 4, 6, 8, 10)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.FromValue("Character Guild 1, Division 1"),    Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(name: Character Guild 1, Division 1)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.FromValue("Character Guild 1, Division 1a"),   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(name: Character Guild 1, Division 1a)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.FromValue("Character Guild 2, Division 2"),    Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(name: Character Guild 2, Division 2)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.FromValue("Character Guild 2, Division 2a"),   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         false           ).SetName("{m}(name: Character Guild 2, Division 2a)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.FromValue("Character Guild 2, Division 3"),    Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(name: Character Guild 2, Division 3)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.FromValue("Character Guild 2, Division 3a"),   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(name: Character Guild 2, Division 3a)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.FromValue("Character Guild 2, Division 3b"),   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         false           ).SetName("{m}(name: Character Guild 2, Division 3b)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.FromValue("Character Guild 3, Division 4"),    Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         false           ).SetName("{m}(name: Character Guild 3, Division 4)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.Unspecified,                                   Optional<bool>.FromValue(true),     Optional<bool>.Unspecified,         true            ).SetName("{m}(isDeleted: true)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.Unspecified,                                   Optional<bool>.FromValue(false),    Optional<bool>.Unspecified,         true            ).SetName("{m}(isDeleted: false)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.FromValue(true),     true            ).SetName("{m}(isLatestVersion: true)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.Unspecified,                                   Optional<bool>.Unspecified,         Optional<bool>.FromValue(false),    true            ).SetName("{m}(isLatestVersion: false)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.FromValue("Character Guild 1, Division 1a"),   Optional<bool>.FromValue(true),     Optional<bool>.Unspecified,         true            ).SetName("{m}(Division has deleted version)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.FromValue("Character Guild 1, Division 1"),    Optional<bool>.FromValue(true),     Optional<bool>.Unspecified,         false           ).SetName("{m}(Division does not have deleted version)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.FromValue("Character Guild 1, Division 2"),    Optional<bool>.FromValue(false),    Optional<bool>.Unspecified,         true            ).SetName("{m}(Division has undeleted version)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.FromValue("Character Guild 2, Division 3"),    Optional<bool>.Unspecified,         Optional<bool>.FromValue(true),     true            ).SetName("{m}(Division has current version)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.FromValue("Character Guild 2, Division 3a"),   Optional<bool>.Unspecified,         Optional<bool>.FromValue(true),     false           ).SetName("{m}(Division does not have current version)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.FromValue("Character Guild 1, Division 2"),    Optional<bool>.Unspecified,         Optional<bool>.FromValue(false),    true            ).SetName("{m}(Division has prior versions)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<IEnumerable<long>>.Unspecified,                                                        Optional<string>.FromValue("Character Guild 2, Division 1"),    Optional<bool>.Unspecified,         Optional<bool>.FromValue(false),    false           ).SetName("{m}(Division does not have prior versions)"),
                new TestCaseData(   Optional<long>.FromValue(0),    Optional<IEnumerable<long>>.FromValue(new[] { 1L }),                                            Optional<string>.FromValue("name 2"),                           Optional<bool>.FromValue(true),     Optional<bool>.FromValue(false),    false           ).SetName("{m}(All criteria specified)")
            };

        [TestCaseSource(nameof(AnyDivisionVersionsAsync_TestCaseData))]
        public async Task AnyDivisionVersionsAsync_Always_ResultIsExpected(
            Optional<long> guildId,
            Optional<IEnumerable<long>> excludedDivisionIds,
            Optional<string> name,
            Optional<bool> isDeleted,
            Optional<bool> isLatestVersion,
            bool expectedResult)
        {
            using var testContext = new TestContext();

            var uut = testContext.BuildUut();

            var result = await uut.AnyDivisionVersionsAsync(
                guildId,
                excludedDivisionIds,
                name,
                isDeleted,
                isLatestVersion,
                testContext.CancellationToken);

            result.ShouldBe(expectedResult);

            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        #endregion AnyDivisionVersionsAsync() Tests

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
            using var testContext = new TestContext();

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

        #region AsyncEnumerateDivisionIdentities() Tests

        public static IReadOnlyList<TestCaseData> AsyncEnumerateDivisionIdentities_TestCaseData
            => new[]
            {
                /*                  guildId,                        isDeleted                           versionIds                                          */
                new TestCaseData(   Optional<long>.Unspecified,     Optional<bool>.Unspecified,         new[] { 2L, 3L, 5L, 8L, 11L, 12L, 14L, 16L, 18L }   ).SetName("{m}(All Guilds, All current versions)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<bool>.FromValue(true),     new[] {         5L                              }   ).SetName("{m}(All Guilds, Deleted current versions)"),
                new TestCaseData(   Optional<long>.Unspecified,     Optional<bool>.FromValue(false),    new[] { 2L, 3L,     8L, 11L, 12L, 14L, 16L, 18L }   ).SetName("{m}(All Guilds, Undeleted current versions)"),
                new TestCaseData(   Optional<long>.FromValue(1),    Optional<bool>.Unspecified,         new[] {         5L,          12L, 14L           }   ).SetName("{m}(Guild 1, All current versions)"),
                new TestCaseData(   Optional<long>.FromValue(1),    Optional<bool>.FromValue(true),     new[] {         5L                              }   ).SetName("{m}(Guild 1, Deleted current versions)"),
                new TestCaseData(   Optional<long>.FromValue(1),    Optional<bool>.FromValue(false),    new[] {                      12L, 14L           }   ).SetName("{m}(Guild 1, Undeleted current versions)"),
                new TestCaseData(   Optional<long>.FromValue(2),    Optional<bool>.Unspecified,         new[] { 2L, 3L,                             18L }   ).SetName("{m}(Guild 2, All current versions)"),
                new TestCaseData(   Optional<long>.FromValue(2),    Optional<bool>.FromValue(true),     Array.Empty<long>()                                 ).SetName("{m}(Guild 2, Deleted current versions)"),
                new TestCaseData(   Optional<long>.FromValue(2),    Optional<bool>.FromValue(false),    new[] { 2L, 3L,                             18L }   ).SetName("{m}(Guild 2, Undeleted current versions)"),
                new TestCaseData(   Optional<long>.FromValue(3),    Optional<bool>.Unspecified,         new[] {             8L, 11L,           16L      }   ).SetName("{m}(Guild 3, All current versions)"),
                new TestCaseData(   Optional<long>.FromValue(3),    Optional<bool>.FromValue(true),     Array.Empty<long>()                                 ).SetName("{m}(Guild 3, Deleted current versions)"),
                new TestCaseData(   Optional<long>.FromValue(3),    Optional<bool>.FromValue(false),    new[] {             8L, 11L,           16L      }   ).SetName("{m}(Guild 3, Undeleted current versions)")
            };

        [TestCaseSource(nameof(AsyncEnumerateDivisionIdentities_TestCaseData))]
        public async Task AsyncEnumerateDivisionIdentities_Always_ReturnsMatchingIdentities(
            Optional<long> guildId,
            Optional<bool> isDeleted,
            IReadOnlyList<long> versionIds)
        {
            using var testContext = new TestContext();

            var uut = testContext.BuildUut();

            var results = await uut.AsyncEnumerateDivisionIdentities(
                    guildId,
                    isDeleted)
                .ToArrayAsync();

            results.ShouldNotBeNull();
            results.ForEach(result => result.ShouldNotBeNull());
            results.Select(x => x?.Id).ShouldBeSetEqualTo(versionIds.Select(x => (long?)testContext.Entities.CharacterGuildDivisionVersions.First(y => y.Id == x).DivisionId));
            foreach (var result in results)
            {
                var entity = testContext.Entities.CharacterGuildDivisionVersions.First(y => (y.DivisionId == result.Id) && versionIds.Contains(y.Id));

                result.Name.ShouldBe(result.Name);
            }

            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        #endregion AsyncEnumerateDivisionIdentities() Tests

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
            using var testContext = new TestContext();

            var uut = testContext.BuildUut();

            var results = await uut.AsyncEnumerateIdentities(
                    isDeleted)
                .ToArrayAsync();

            results.ShouldNotBeNull();
            results.ForEach(result => result.ShouldNotBeNull());
            results.Select(x => x?.Id).ShouldBeSetEqualTo(versionIds.Select(x => (long?)testContext.Entities.CharacterGuildVersions.First(y => y.Id == x).GuildId));
            foreach (var result in results)
            {
                var entity = testContext.Entities.CharacterGuildVersions.First(y => (y.GuildId == result.Id) && versionIds.Contains(y.Id));

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

        #region ReadDivisionIdentityAsync() Tests

        public static IReadOnlyList<TestCaseData> ReadDivisionIdentityAsync_MatchDoesNotExist_TestCaseData
            = new[] {
                /*                  divisionId,     guildId,                                    isDeleted                           */
                new TestCaseData(   default(long),  default(Optional<long>),                    default(Optional<bool>)             ).SetName("{m}(Unspecified Values)"),
                new TestCaseData(   default(long),  Optional<long>.FromValue(default),          Optional<bool>.FromValue(default)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue,  Optional<long>.FromValue(long.MinValue),    Optional<bool>.FromValue(false)     ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             Optional<long>.FromValue(2),                Optional<bool>.FromValue(false)     ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   3L,             Optional<long>.FromValue(4),                Optional<bool>.FromValue(true)      ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   5L,             Optional<long>.FromValue(6),                Optional<bool>.FromValue(false)     ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  Optional<long>.FromValue(long.MaxValue),    Optional<bool>.FromValue(true)      ).SetName("{m}(Max Values)"),
                new TestCaseData(   10L,            Optional<long>.Unspecified,                 Optional<bool>.Unspecified          ).SetName("{m}(Division does not exist)"),
                new TestCaseData(   1L,             Optional<long>.FromValue(2),                Optional<bool>.Unspecified          ).SetName("{m}(Division belongs to different Guild)"),
                new TestCaseData(   4L,             Optional<long>.Unspecified,                 Optional<bool>.FromValue(false)     ).SetName("{m}(Division is deleted)"),
                new TestCaseData(   5L,             Optional<long>.Unspecified,                 Optional<bool>.FromValue(true)      ).SetName("{m}(Division is not deleted)")
            };

        [TestCaseSource(nameof(ReadDivisionIdentityAsync_MatchDoesNotExist_TestCaseData))]
        public async Task ReadDivisionIdentityAsync_MatchDoesNotExist_ReturnsDataNotFoundError(
            long divisionId,
            Optional<long> guildId,
            Optional<bool> isDeleted)
        {
            using var testContext = new TestContext();

            var uut = testContext.BuildUut();

            var result = await uut.ReadDivisionIdentityAsync(
                divisionId,
                guildId,
                isDeleted,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            var error = result.Error.ShouldBeOfType<DataNotFoundError>();
            error.Message.ShouldContain(divisionId.ToString());
            if (guildId.IsSpecified)
                error.Message.ShouldContain(guildId.Value.ToString());
        }

        public static IReadOnlyList<TestCaseData> ReadDivisionIdentityAsync_MatchExists_TestCaseData
            = new[] {
                /*                  divisionId, guildId,                        isDeleted,                          versionId   */
                new TestCaseData(   1L,         Optional<long>.Unspecified,     Optional<bool>.Unspecified,         14L         ).SetName("{m}(Division Exists)"),
                new TestCaseData(   3L,         Optional<long>.FromValue(2),    Optional<bool>.Unspecified,         3L          ).SetName("{m}(Division belongs to Guild)"),
                new TestCaseData(   5L,         Optional<long>.Unspecified,     Optional<bool>.FromValue(false),    8L          ).SetName("{m}(Division is not deleted)"),
                new TestCaseData(   4L,         Optional<long>.Unspecified,     Optional<bool>.FromValue(true),     4L          ).SetName("{m}(Division is deleted)")
            };

        [TestCaseSource(nameof(ReadDivisionIdentityAsync_MatchExists_TestCaseData))]
        public async Task ReadDivisionIdentityAsync_MatchExists_ReturnsMatch(
            long divisionId,
            Optional<long> guildId,
            Optional<bool> isDeleted,
            long versionId)
        {
            using var testContext = new TestContext();

            var uut = testContext.BuildUut();

            var result = await uut.ReadDivisionIdentityAsync(
                divisionId,
                guildId,
                isDeleted,
                testContext.CancellationToken);

            result.IsSuccess.ShouldBeTrue();
            result.Value.Id.ShouldBe(divisionId);

            var entity = testContext.Entities.CharacterGuildDivisionVersions.First(x => x.Id == versionId);
            result.Value.Name.ShouldBe(entity.Name);
        }

        #endregion ReadDivisionIdentityAsync() Tests

        #region ReadIdentityAsync() Tests

        public static IReadOnlyList<TestCaseData> ReadIdentityAsync_MatchDoesNotExist_TestCaseData
            = new[] {
                /*                  guildId,        isDeleted                           */
                new TestCaseData(   default(long),  default(Optional<bool>)             ).SetName("{m}(Unspecified Values)"),
                new TestCaseData(   default(long),  Optional<bool>.FromValue(default)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue,  Optional<bool>.FromValue(false)     ).SetName("{m}(Min Values)"),
                new TestCaseData(   long.MaxValue,  Optional<bool>.FromValue(true)      ).SetName("{m}(Max Values)"),
                new TestCaseData(   4L,             Optional<bool>.Unspecified          ).SetName("{m}(Guild does not exist)"),
                new TestCaseData(   2L,             Optional<bool>.FromValue(false)     ).SetName("{m}(Guild is deleted)"),
                new TestCaseData(   3L,             Optional<bool>.FromValue(true)      ).SetName("{m}(Guild is not deleted)")
            };

        [TestCaseSource(nameof(ReadIdentityAsync_MatchDoesNotExist_TestCaseData))]
        public async Task ReadIdentityAsync_MatchDoesNotExist_ReturnsDataNotFoundError(
            long guildId,
            Optional<bool> isDeleted)
        {
            using var testContext = new TestContext();

            var uut = testContext.BuildUut();

            var result = await uut.ReadIdentityAsync(
                guildId,
                isDeleted,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            var error = result.Error.ShouldBeOfType<DataNotFoundError>();
            error.Message.ShouldContain(guildId.ToString());
        }

        public static IReadOnlyList<TestCaseData> ReadIdentityAsync_MatchExists_TestCaseData
            = new[] {
                /*                  guildId,    isDeleted,                          versionId   */
                new TestCaseData(   1L,         Optional<bool>.Unspecified,         9L          ).SetName("{m}(Guild Exists)"),
                new TestCaseData(   3L,         Optional<bool>.FromValue(false),    8L          ).SetName("{m}(Guild is not deleted)"),
                new TestCaseData(   2L,         Optional<bool>.FromValue(true),     5L          ).SetName("{m}(Guild is deleted)")
            };

        [TestCaseSource(nameof(ReadIdentityAsync_MatchExists_TestCaseData))]
        public async Task ReadIdentityAsync_MatchExists_ReturnsMatch(
            long guildId,
            Optional<bool> isDeleted,
            long versionId)
        {
            using var testContext = new TestContext();

            var uut = testContext.BuildUut();

            var result = await uut.ReadIdentityAsync(
                guildId,
                isDeleted,
                testContext.CancellationToken);

            result.IsSuccess.ShouldBeTrue();
            result.Value.Id.ShouldBe(guildId);

            var entity = testContext.Entities.CharacterGuildVersions.First(x => x.Id == versionId);
            result.Value.Name.ShouldBe(entity.Name);
        }

        #endregion ReadIdentityAsync() Tests

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
