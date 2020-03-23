using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using NUnit.Framework;
using Moq;
using Shouldly;

using Microsoft.Extensions.Internal;

using Sokan.Yastah.Business.Characters;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data;
using Sokan.Yastah.Data.Auditing;
using Sokan.Yastah.Data.Characters;

using Sokan.Yastah.Common.Test;

namespace Sokan.Yastah.Business.Test.Characters
{
    [TestFixture]
    public class CharacterGuildDivisionsServiceTests
    {
        #region Test Context

        internal class TestContext
            : AsyncMethodWithLoggerTestContext
        {
            public TestContext()
            {
                UtcNow = default;
                NextAdministrationActionId = default;
                NextDivisionId = default;

                MockAuditableActionsRepository = new Mock<IAuditableActionsRepository>();
                MockAuditableActionsRepository
                    .Setup(x => x.CreateAsync(
                        It.IsAny<int>(),
                        It.IsAny<DateTimeOffset>(),
                        It.IsAny<ulong>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => NextAdministrationActionId);

                MockCharacterGuildsRepository = new Mock<ICharacterGuildsRepository>();
                
                MockCharacterGuildDivisionsRepository = new Mock<ICharacterGuildDivisionsRepository>();
                MockCharacterGuildDivisionsRepository
                    .Setup(x => x.CreateAsync(
                        It.IsAny<long>(),
                        It.IsAny<string>(),
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => NextDivisionId);

                MockSystemClock = new Mock<ISystemClock>();
                MockSystemClock
                    .Setup(x => x.UtcNow)
                    .Returns(() => UtcNow);

                MockTransactionScopeFactory = new Mock<ITransactionScopeFactory>();
                MockTransactionScopeFactory
                    .Setup(x => x.CreateScope(It.IsAny<IsolationLevel?>()))
                    .Returns(() => MockTransactionScope.Object);

                MockTransactionScope = new Mock<ITransactionScope>();
            }

            public DateTimeOffset UtcNow;
            public long NextAdministrationActionId;
            public long NextDivisionId;

            public readonly Mock<IAuditableActionsRepository> MockAuditableActionsRepository;
            public readonly Mock<ICharacterGuildsRepository> MockCharacterGuildsRepository;
            public readonly Mock<ICharacterGuildDivisionsRepository> MockCharacterGuildDivisionsRepository;
            public readonly Mock<ISystemClock> MockSystemClock;
            public readonly Mock<ITransactionScopeFactory> MockTransactionScopeFactory;
            public readonly Mock<ITransactionScope> MockTransactionScope;

            public CharacterGuildDivisionsService BuildUut()
                => new CharacterGuildDivisionsService(
                    MockAuditableActionsRepository.Object,
                    MockCharacterGuildsRepository.Object,
                    MockCharacterGuildDivisionsRepository.Object,
                    LoggerFactory.CreateLogger<CharacterGuildDivisionsService>(),
                    MockSystemClock.Object,
                    MockTransactionScopeFactory.Object);

            public void SetDivisionUpdateError(OperationError error)
                => MockCharacterGuildDivisionsRepository
                    .Setup(x => x.UpdateAsync(
                        It.IsAny<long>(),
                        It.IsAny<long>(),
                        It.IsAny<Optional<string>>(),
                        It.IsAny<Optional<bool>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(error);

            public void SetDivisionUpdateVersionId(long versionId)
                => MockCharacterGuildDivisionsRepository
                    .Setup(x => x.UpdateAsync(
                        It.IsAny<long>(),
                        It.IsAny<long>(),
                        It.IsAny<Optional<string>>(),
                        It.IsAny<Optional<bool>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(versionId);

            public void SetGuildUpdateError(OperationError error)
                => MockCharacterGuildDivisionsRepository
                    .Setup(x => x.UpdateAsync(
                        It.IsAny<long>(),
                        It.IsAny<long>(),
                        It.IsAny<Optional<string>>(),
                        It.IsAny<Optional<bool>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(error);

            public void SetGuildUpdateVersionId(long versionId)
                => MockCharacterGuildDivisionsRepository
                    .Setup(x => x.UpdateAsync(
                        It.IsAny<long>(),
                        It.IsAny<long>(),
                        It.IsAny<Optional<string>>(),
                        It.IsAny<Optional<bool>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(versionId);

            public void SetIsDivisionNameInUse(long guildId, string name, bool isNameInUse)
                => MockCharacterGuildDivisionsRepository
                    .Setup(x => x.AnyVersionsAsync(
                        guildId,
                        It.IsAny<Optional<IEnumerable<long>>>(),
                        name,
                        false,
                        true,
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(isNameInUse);

            public void SetIsGuildIdActive(long guildId, bool isGuildIdActive)
                => MockCharacterGuildsRepository
                    .Setup(x => x.AnyVersionsAsync(
                        guildId,
                        It.IsAny<Optional<IEnumerable<long>>>(),
                        It.IsAny<Optional<string>>(),
                        false,
                        true,
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(isGuildIdActive);
        }

        #endregion Test Context

        #region CreateAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> CreateAsync_ValidationFailure_TestCaseData
            = new[]
            {
                /*                  guildId,        performedById,  name            */
                new TestCaseData(   default(long),  default(ulong), string.Empty    ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue,  ulong.MinValue, string.Empty    ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2UL,            "Name 3"        ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   4L,             5UL,            "Name 6"        ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   7L,             8UL,            "Name 9"        ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  ulong.MaxValue, "Max Value"     ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(CreateAsync_ValidationFailure_TestCaseData))]
        public async Task CreateAsync_GuildIdIsInvalid_ReturnsDataNotFoundError(
            long guildId,
            ulong performedById,
            string name)
        {
            using var testContext = new TestContext();

            testContext.SetIsGuildIdActive(guildId, false);
            testContext.SetIsDivisionNameInUse(guildId, name, false);

            var uut = testContext.BuildUut();

            var creationModel = new CharacterGuildDivisionCreationModel()
            {
                Name = name
            };

            var result = await uut.CreateAsync(
                guildId,
                creationModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    guildId,
                    default,
                    default,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockTransactionScope.ShouldNotHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            result.IsFailure.ShouldBeTrue();
            var error = result.Error.ShouldBeOfType<DataNotFoundError>();
            error.Message.ShouldContain(guildId.ToString());
        }

        [TestCaseSource(nameof(CreateAsync_ValidationFailure_TestCaseData))]
        public async Task CreateAsync_NameIsInUse_ReturnsNameInUseError(
            long guildId,
            ulong performedById,
            string name)
        {
            using var testContext = new TestContext();

            testContext.SetIsGuildIdActive(guildId, true);
            testContext.SetIsDivisionNameInUse(guildId, name, true);

            var uut = testContext.BuildUut();

            var creationModel = new CharacterGuildDivisionCreationModel()
            {
                Name = name
            };

            var result = await uut.CreateAsync(
                guildId,
                creationModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockCharacterGuildDivisionsRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    guildId,
                    default,
                    name,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockTransactionScope.ShouldNotHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            result.IsFailure.ShouldBeTrue();
            var error = result.Error.ShouldBeOfType<NameInUseError>();
            error.Message.ShouldContain(name);
        }

        public static readonly IReadOnlyList<TestCaseData> CreateAsync_Success_TestCaseData
            = new[]
            {
                /*                  guildId,        actionId,       performed,                          performedById,  name,           divisionId      */
                new TestCaseData(   default(long),  default(long),  default(DateTimeOffset),            default(ulong), string.Empty,   default(long)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue,  long.MinValue,  DateTimeOffset.MinValue,            ulong.MinValue, string.Empty,   long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2L,             DateTimeOffset.Parse("2003-04-05"), 6UL,            "Name 7",       8L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   9L,             10L,            DateTimeOffset.Parse("2011-12-13"), 14UL,           "Name 15",      16L             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   17L,            18L,            DateTimeOffset.Parse("2019-08-21"), 22UL,           "Name 23",      24L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue,  DateTimeOffset.MaxValue,            ulong.MaxValue, "Max Value",    long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(CreateAsync_Success_TestCaseData))]
        public async Task CreateAsync_CreationModelIsValid_PerformesCreatesAndWipesCacheAndReturnsSuccess(
            long guildId,
            long actionId,
            DateTimeOffset performed,
            ulong performedById,
            string name,
            long divisionId)
        {
            using var testContext = new TestContext()
            {
                UtcNow = performed,
                NextAdministrationActionId = actionId,
                NextDivisionId = divisionId
            };

            testContext.SetIsGuildIdActive(guildId, true);
            testContext.SetIsDivisionNameInUse(guildId, name, false);

            var uut = testContext.BuildUut();

            var creationModel = new CharacterGuildDivisionCreationModel()
            {
                Name = name
            };

            var result = await uut.CreateAsync(
                guildId,
                creationModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    guildId,
                    default,
                    default,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockCharacterGuildDivisionsRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    guildId,
                    default,
                    name,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockAuditableActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)CharacterManagementAdministrationActionType.DivisionCreated,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockCharacterGuildDivisionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    guildId,
                    name,
                    actionId,
                    testContext.CancellationToken));

            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(divisionId);
        }

        #endregion CreateAsync() Tests

        #region DeleteAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> DeleteAsync_ValidationFailure_TestCaseData
            = new[]
            {
                /*                  guildId,        divisionId,     performedById   */
                new TestCaseData(   default(long),  default(long),  default(ulong)  ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue,  long.MinValue,  ulong.MinValue  ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2L,             3UL             ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   4L,             5L,             6UL             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   7L,             8L,             9UL             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue,  ulong.MaxValue  ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(DeleteAsync_ValidationFailure_TestCaseData))]
        public async Task DeleteAsync_GuildIdIsNotValid_ReturnsDataNotFound(
            long guildId,
            long divisionId,
            ulong performedById)
        {
            using var testContext = new TestContext();

            testContext.SetIsGuildIdActive(guildId, false);

            var uut = testContext.BuildUut();

            var result = await uut.DeleteAsync(
                guildId,
                divisionId,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockAuditableActionsRepository.ShouldNotHaveReceived(x => x
                .CreateAsync(
                    It.IsAny<int>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<ulong?>(),
                    It.IsAny<CancellationToken>()));

            testContext.MockCharacterGuildDivisionsRepository.ShouldNotHaveReceived(x => x
                .UpdateAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<Optional<string>>(),
                    It.IsAny<Optional<bool>>(),
                    It.IsAny<CancellationToken>()));

            testContext.MockTransactionScope.ShouldNotHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            result.IsFailure.ShouldBeTrue();
            var error = result.Error.ShouldBeOfType<DataNotFoundError>();
            error.Message.ShouldContain(guildId.ToString());
        }

        public static readonly IReadOnlyList<TestCaseData> DeleteAsync_UpdateFailure_TestCaseData
            = new[]
            {
                /*                  guildId,        divisionId,     actionId,       performed,                          performedById,  */
                new TestCaseData(   default(long),  default(long),  default(long),  default(DateTimeOffset),            default(ulong)  ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue,  long.MinValue,  long.MinValue,  DateTimeOffset.MinValue,            ulong.MinValue  ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2L,             3L,             DateTimeOffset.Parse("2004-05-06"), 7UL             ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   8L,             9L,             10L,            DateTimeOffset.Parse("2011-12-13"), 14UL            ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   15L,            16L,            17L,            DateTimeOffset.Parse("2018-07-20"), 21UL            ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue,  long.MaxValue,  DateTimeOffset.MaxValue,            ulong.MaxValue  ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(DeleteAsync_UpdateFailure_TestCaseData))]
        public async Task DeleteAsync_UpdateAsyncFails_ReturnsImmediately(
            long guildId,
            long divisionId,
            long actionId,
            DateTimeOffset performed,
            ulong performedById)
        {
            using var testContext = new TestContext()
            {
                UtcNow = performed,
                NextAdministrationActionId = actionId
            };

            testContext.SetIsGuildIdActive(guildId, true);

            var mockError = new Mock<OperationError>("Mock Message");
            testContext.SetDivisionUpdateError(mockError.Object);

            var uut = testContext.BuildUut();

            var result = await uut.DeleteAsync(
                guildId,
                divisionId,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockAuditableActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)CharacterManagementAdministrationActionType.DivisionDeleted,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockCharacterGuildDivisionsRepository.ShouldHaveReceived(x => x
                .UpdateAsync(
                    divisionId,
                    actionId,
                    Optional<string>.Unspecified,
                    true,
                    testContext.CancellationToken));

            testContext.MockTransactionScope.ShouldNotHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);
        }

        public static readonly IReadOnlyList<TestCaseData> DeleteAsync_Success_TestCaseData
            = new[]
            {
                /*                  guildId,        divisionId,     actionId,       performed,                          performedById,  versionId       */
                new TestCaseData(   default(long),  default(long),  default(long),  default(DateTimeOffset),            default(ulong), default(long)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue,  long.MinValue,  long.MinValue,  DateTimeOffset.MinValue,            ulong.MinValue, long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2L,             3L,             DateTimeOffset.Parse("2004-05-06"), 7UL,            8L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   9L,             10L,            11L,            DateTimeOffset.Parse("2012-01-14"), 15UL,           16L             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   17L,            18L,            19L,            DateTimeOffset.Parse("2020-09-22"), 23UL,           24L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue,  long.MaxValue,  DateTimeOffset.MaxValue,            ulong.MaxValue, long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(DeleteAsync_Success_TestCaseData))]
        public async Task DeleteAsync_UpdateAsyncSucceeds_PerformsDeleteAndWipesCacheAndReturnsSuccess(
            long guildId,
            long divisionId,
            long actionId,
            DateTimeOffset performed,
            ulong performedById,
            long versionId)
        {
            using var testContext = new TestContext()
            {
                UtcNow = performed,
                NextAdministrationActionId = actionId
            };

            testContext.SetIsGuildIdActive(guildId, true);
            testContext.SetDivisionUpdateVersionId(versionId);

            var uut = testContext.BuildUut();

            var result = await uut.DeleteAsync(
                guildId,
                divisionId,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockAuditableActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)CharacterManagementAdministrationActionType.DivisionDeleted,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockCharacterGuildDivisionsRepository.ShouldHaveReceived(x => x
                .UpdateAsync(
                    divisionId,
                    actionId,
                    Optional<string>.Unspecified,
                    true,
                    testContext.CancellationToken));

            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            result.IsSuccess.ShouldBeTrue();
        }

        #endregion DeleteAsync() Tests

        #region GetCurrentIdentitiesAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> GetCurrentIdentitiesAsync_TestCaseData
            = new[]
            {
                /*                  guildId         */
                new TestCaseData(   default(long)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   2L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   3L              ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(GetCurrentIdentitiesAsync_TestCaseData))]
        public async Task GetCurrentIdentitiesAsync_Always_ReadsIdentities(
            long guildId)
        {
            using var testContext = new TestContext();

            var identities = new CharacterGuildDivisionIdentityViewModel[]
            {
                new CharacterGuildDivisionIdentityViewModel(
                    id:     default,
                    name:   string.Empty)
            };

            testContext.MockCharacterGuildDivisionsRepository
                .Setup(x => x.AsyncEnumerateIdentities(
                    It.IsAny<Optional<long>>(),
                    It.IsAny<Optional<bool>>()))
                .Returns(identities.ToAsyncEnumerable());

            var uut = testContext.BuildUut();

            var result = await uut.GetCurrentIdentitiesAsync(
                guildId,
                testContext.CancellationToken);

            testContext.MockCharacterGuildDivisionsRepository.ShouldHaveReceived(x => x
                .AsyncEnumerateIdentities(guildId, false));

            result.ShouldBeSetEqualTo(identities);
        }

        #endregion GetCurrentIdentitiesAsync() Tests

        #region UpdateAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> UpdateAsync_ValidationFailure_TestCaseData
            = new[]
            {
                /*                  guildId,        divisionId,     name,           performedById   */
                new TestCaseData(   default(long),  default(long),  string.Empty,   default(ulong)  ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue,  long.MinValue,  string.Empty,   ulong.MinValue  ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2L,             "Name 3",       4UL             ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   5L,             6L,             "Name 7",       8UL             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   9L,             10L,            "Name 11",      12UL            ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue,  "MaxValue",     ulong.MaxValue  ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(UpdateAsync_ValidationFailure_TestCaseData))]
        public async Task UpdateAsync_GuildIdIsNotActive_ReturnsDataNotFoundError(
            long guildId,
            long divisionId,
            string name,
            ulong performedById)
        {
            using var testContext = new TestContext();

            testContext.SetIsGuildIdActive(guildId, false);
            testContext.SetIsDivisionNameInUse(guildId, name, false);

            var uut = testContext.BuildUut();

            var updateModel = new CharacterGuildDivisionUpdateModel()
            {
                Name = name,
            };

            var result = await uut.UpdateAsync(
                guildId,
                divisionId,
                updateModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    guildId,
                    default,
                    default,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockTransactionScope.ShouldNotHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeOfType<DataNotFoundError>();
            result.Error.Message.ShouldContain(guildId.ToString());
        }

        [TestCaseSource(nameof(UpdateAsync_ValidationFailure_TestCaseData))]
        public async Task UpdateAsync_NameIsInUse_ReturnsNameInUseError(
            long guildId,
            long divisionId,
            string name,
            ulong performedById)
        {
            using var testContext = new TestContext();

            testContext.SetIsGuildIdActive(guildId, true);
            testContext.SetIsDivisionNameInUse(guildId, name, true);

            var uut = testContext.BuildUut();

            var updateModel = new CharacterGuildDivisionUpdateModel()
            {
                Name = name,
            };

            var result = await uut.UpdateAsync(
                guildId,
                divisionId,
                updateModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockCharacterGuildDivisionsRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    guildId,
                    It.Is<Optional<IEnumerable<long>>>(y => y.IsSpecified && (y.Value != null) && y.Value.SetEquals(divisionId.ToEnumerable())),
                    name,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockTransactionScope.ShouldNotHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeOfType<NameInUseError>();
            result.Error.Message.ShouldContain(name);
        }

        public static readonly IReadOnlyList<TestCaseData> UpdateAsync_UpdateFailure_TestCaseData
            = new[]
            {
                /*                  guildId,        divisionId,     name,           performed,                          performedById,  actionId        */
                new TestCaseData(   default(long),  default(long),  string.Empty,   default(DateTimeOffset),            default(ulong), default(long)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue,  long.MinValue,  string.Empty,   DateTimeOffset.MinValue,            ulong.MinValue, long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2L,             "Name 3",       DateTimeOffset.Parse("2004-05-06"), 7UL,            8L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   9L,             10L,            "Name 11",      DateTimeOffset.Parse("2012-01-14"), 15UL,           16L             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   17L,            18L,            "Name 19",      DateTimeOffset.Parse("2020-09-22"), 23UL,           24L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue,  "MaxValue",     DateTimeOffset.MaxValue,            ulong.MaxValue, long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(UpdateAsync_UpdateFailure_TestCaseData))]
        public async Task UpdateAsync_UpdateAsyncFails_ReturnsImmediately(
            long guildId,
            long divisionId,
            string name,
            DateTimeOffset performed,
            ulong performedById,
            long actionId)
        {
            using var testContext = new TestContext()
            {
                UtcNow = performed,
                NextAdministrationActionId = actionId
            };

            testContext.SetIsGuildIdActive(guildId, true);
            testContext.SetIsDivisionNameInUse(guildId, name, false);

            var mockError = new Mock<OperationError>("Mock Message");
            testContext.SetDivisionUpdateError(mockError.Object);

            var uut = testContext.BuildUut();

            var updateModel = new CharacterGuildDivisionUpdateModel()
            {
                Name = name
            };

            var result = await uut.UpdateAsync(
                guildId,
                divisionId,
                updateModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    guildId,
                    default,
                    default,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockCharacterGuildDivisionsRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    guildId,
                    It.Is<Optional<IEnumerable<long>>>(y => y.IsSpecified && (y.Value != null) && y.Value.SetEquals(divisionId.ToEnumerable())),
                    name,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockAuditableActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)CharacterManagementAdministrationActionType.DivisionModified,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockCharacterGuildDivisionsRepository.ShouldHaveReceived(x => x
                .UpdateAsync(
                    divisionId,
                    actionId,
                    name,
                    default,
                    testContext.CancellationToken));

            testContext.MockTransactionScope.ShouldNotHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);
        }

        public static readonly IReadOnlyList<TestCaseData> UpdateAsync_Success_TestCaseData
            = new[]
            {
                /*                  guildId,        divisionId,     name,           performed,                          performedById,  actionId,       versionId       */
                new TestCaseData(   default(long),  default(long),  string.Empty,   default(DateTimeOffset),            default(ulong), default(long),  default(long)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue,  long.MinValue,  string.Empty,   DateTimeOffset.MinValue,            ulong.MinValue, long.MinValue,  long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2L,             "Name 3",       DateTimeOffset.Parse("2004-05-06"), 7UL,            8L,             9L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   10L,            11L,            "Name 12",      DateTimeOffset.Parse("2013-02-15"), 16UL,           17L,            18L             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   19L,            20L,            "Name 21",      DateTimeOffset.Parse("2022-11-24"), 25UL,           26L,            27L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue,  "MaxValue",     DateTimeOffset.MaxValue,            ulong.MaxValue, long.MaxValue,  long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(UpdateAsync_Success_TestCaseData))]
        public async Task UpdateAsync_UpdateSucceeds_PerformsUpdateAndWipesCacheAndReturnsSuccess(
            long guildId,
            long divisionId,
            string name,
            DateTimeOffset performed,
            ulong performedById,
            long actionId,
            long versionId)
        {
            using var testContext = new TestContext()
            {
                UtcNow = performed,
                NextAdministrationActionId = actionId
            };

            testContext.SetIsGuildIdActive(guildId, true);
            testContext.SetIsDivisionNameInUse(guildId, name, false);
            testContext.SetDivisionUpdateVersionId(versionId);

            var uut = testContext.BuildUut();

            var updateModel = new CharacterGuildDivisionUpdateModel()
            {
                Name = name
            };

            var result = await uut.UpdateAsync(
                guildId,
                divisionId,
                updateModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    guildId,
                    default,
                    default,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockCharacterGuildDivisionsRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    guildId,
                    It.Is<Optional<IEnumerable<long>>>(y => y.IsSpecified && (y.Value != null) && y.Value.SetEquals(divisionId.ToEnumerable())),
                    name,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockAuditableActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)CharacterManagementAdministrationActionType.DivisionModified,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockCharacterGuildDivisionsRepository.ShouldHaveReceived(x => x
                .UpdateAsync(
                    divisionId,
                    actionId,
                    name,
                    default,
                    testContext.CancellationToken));

            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            result.IsSuccess.ShouldBeTrue();
        }

        #endregion UpdateAsync() Tests
    }
}
