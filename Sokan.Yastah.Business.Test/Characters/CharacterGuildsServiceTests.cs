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
using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Characters;

using Sokan.Yastah.Common.Test;

namespace Sokan.Yastah.Business.Test.Characters
{
    [TestFixture]
    public class CharacterGuildsServiceTests
    {
        #region Test Context

        internal class TestContext
            : AsyncMethodTestContext
        {
            public TestContext()
            {
                MockAdministrationActionsRepository = new Mock<IAdministrationActionsRepository>();
                MockAdministrationActionsRepository
                    .Setup(x => x.CreateAsync(
                        It.IsAny<int>(),
                        It.IsAny<DateTimeOffset>(),
                        It.IsAny<ulong>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => NextAdministrationActionId);

                MockCharacterGuildsRepository = new Mock<ICharacterGuildsRepository>();
                MockCharacterGuildsRepository
                    .Setup(x => x.CreateAsync(
                        It.IsAny<string>(),
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => {
                        return NextGuildId;
                    });
                MockCharacterGuildsRepository
                    .Setup(x => x.CreateDivisionAsync(
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
            public long NextGuildId;
            public long NextDivisionId;

            public readonly Mock<IAdministrationActionsRepository> MockAdministrationActionsRepository;
            public readonly Mock<ICharacterGuildsRepository> MockCharacterGuildsRepository;
            public readonly Mock<ISystemClock> MockSystemClock;
            public readonly Mock<ITransactionScopeFactory> MockTransactionScopeFactory;
            public readonly Mock<ITransactionScope> MockTransactionScope;

            public CharacterGuildsService BuildUut()
                => new CharacterGuildsService(
                    MockAdministrationActionsRepository.Object,
                    MockCharacterGuildsRepository.Object,
                    MockSystemClock.Object,
                    MockTransactionScopeFactory.Object);

            public void SetDivisionUpdateError(IOperationError error)
                => MockCharacterGuildsRepository
                    .Setup(x => x.UpdateDivisionAsync(
                        It.IsAny<long>(),
                        It.IsAny<long>(),
                        It.IsAny<Optional<string>>(),
                        It.IsAny<Optional<bool>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(error.ToError<long>());

            public void SetDivisionUpdateVersionId(long versionId)
                => MockCharacterGuildsRepository
                    .Setup(x => x.UpdateDivisionAsync(
                        It.IsAny<long>(),
                        It.IsAny<long>(),
                        It.IsAny<Optional<string>>(),
                        It.IsAny<Optional<bool>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(versionId);

            public void SetGuildUpdateError(IOperationError error)
                => MockCharacterGuildsRepository
                    .Setup(x => x.UpdateAsync(
                        It.IsAny<long>(),
                        It.IsAny<long>(),
                        It.IsAny<Optional<string>>(),
                        It.IsAny<Optional<bool>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(error.ToError<long>());

            public void SetGuildUpdateVersionId(long versionId)
                => MockCharacterGuildsRepository
                    .Setup(x => x.UpdateAsync(
                        It.IsAny<long>(),
                        It.IsAny<long>(),
                        It.IsAny<Optional<string>>(),
                        It.IsAny<Optional<bool>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(versionId);

            public void SetIsDivisionNameInUse(long guildId, string name, bool isNameInUse)
                => MockCharacterGuildsRepository
                    .Setup(x => x.AnyDivisionVersionsAsync(
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

            public void SetIsNameInUse(string name, bool isNameInUse)
                => MockCharacterGuildsRepository
                    .Setup(x => x.AnyVersionsAsync(
                        It.IsAny<Optional<long>>(),
                        It.IsAny<Optional<IEnumerable<long>>>(),
                        name,
                        false,
                        true,
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(isNameInUse);
        }

        #endregion Test Context

        #region CreateAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> CreateAsync_NameIsInUse_TestCaseData
            = new[]
            {
                /*                  performedById,  name            */
                new TestCaseData(   default(ulong), string.Empty    ).SetName("{m}(Default Values)"),
                new TestCaseData(   ulong.MinValue, string.Empty    ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            "Name 1"        ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   2UL,            "Name 2"        ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   3UL,            "Name 3"        ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, "Max Value"     ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(CreateAsync_NameIsInUse_TestCaseData))]
        public async Task CreateAsync_NameIsInUse_ReturnsNameInUseError(
            ulong performedById,
            string name)
        {
            using var testContext = new TestContext();

            testContext.SetIsNameInUse(name, true);

            var uut = testContext.BuildUut();

            var creationModel = new CharacterGuildCreationModel()
            {
                Name = name
            };

            var result = await uut.CreateAsync(
                creationModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    default,
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
            error.Message.Contains(name);
        }

        public static readonly IReadOnlyList<TestCaseData> CreateAsync_Success_TestCaseData
            = new[]
            {
                /*                  actionId,       performed,                          performedById,  name,           guildId         */
                new TestCaseData(   default(long),  default(DateTimeOffset),            default(ulong), string.Empty,   default(long)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue,  DateTimeOffset.MinValue,            ulong.MinValue, string.Empty,   long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             DateTimeOffset.Parse("2019-01-01"), 2UL,            "Name 3",       4L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   5L,             DateTimeOffset.Parse("2019-01-02"), 6UL,            "Name 7",       9L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   10L,            DateTimeOffset.Parse("2019-01-03"), 11UL,           "Name 12",      16L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  DateTimeOffset.MaxValue,            ulong.MaxValue, "Max Value",    long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(CreateAsync_Success_TestCaseData))]
        public async Task CreateAsync_CreationModelIsValid_PerformesCreatesAndWipesCacheAndReturnsSuccess(
            long actionId,
            DateTimeOffset performed,
            ulong performedById,
            string name,
            long guildId)
        {
            using var testContext = new TestContext()
            {
                UtcNow = performed,
                NextAdministrationActionId = actionId,
                NextGuildId = guildId
            };

            testContext.SetIsNameInUse(name, false);

            var uut = testContext.BuildUut();

            var creationModel = new CharacterGuildCreationModel()
            {
                Name = name
            };

            var result = await uut.CreateAsync(
                creationModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    default,
                    default,
                    name,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockAdministrationActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)CharacterManagementAdministrationActionType.GuildCreated,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    name,
                    actionId,
                    testContext.CancellationToken));

            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(guildId);
        }

        #endregion CreateAsync() Tests

        #region CreateDivisionAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> CreateDivisionAsync_ValidationFailure_TestCaseData
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

        [TestCaseSource(nameof(CreateDivisionAsync_ValidationFailure_TestCaseData))]
        public async Task CreateDivisionAsync_GuildIdIsInvalid_ReturnsDataNotFoundError(
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

            var result = await uut.CreateDivisionAsync(
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

        [TestCaseSource(nameof(CreateDivisionAsync_ValidationFailure_TestCaseData))]
        public async Task CreateDivisionAsync_NameIsInUse_ReturnsNameInUseError(
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

            var result = await uut.CreateDivisionAsync(
                guildId,
                creationModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .AnyDivisionVersionsAsync(
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

        public static readonly IReadOnlyList<TestCaseData> CreateDivisionAsync_Success_TestCaseData
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

        [TestCaseSource(nameof(CreateDivisionAsync_Success_TestCaseData))]
        public async Task CreateDivisionAsync_CreationModelIsValid_PerformesCreatesAndWipesCacheAndReturnsSuccess(
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

            var result = await uut.CreateDivisionAsync(
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

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .AnyDivisionVersionsAsync(
                    guildId,
                    default,
                    name,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockAdministrationActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)CharacterManagementAdministrationActionType.DivisionCreated,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .CreateDivisionAsync(
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

        #endregion CreateDivisionAsync() Tests

        #region DeleteAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> DeleteAsync_Failure_TestCaseData
            = new[]
            {
                /*                  guildId,         actionId,       performed,                          performedById,  */
                new TestCaseData(   default(long),  default(long),  default(DateTimeOffset),            default(ulong)  ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue,  long.MinValue,  DateTimeOffset.MinValue,            ulong.MinValue  ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2L,             DateTimeOffset.Parse("2019-01-01"), 3UL             ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   4L,             5L,             DateTimeOffset.Parse("2019-01-02"), 6UL             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   7L,             8L,             DateTimeOffset.Parse("2019-01-03"), 9UL             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue,  DateTimeOffset.MaxValue,            ulong.MaxValue  ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(DeleteAsync_Failure_TestCaseData))]
        public async Task DeleteAsync_UpdateAsyncFails_ReturnsImmediatels(
            long guildId,
            long actionId,
            DateTimeOffset performed,
            ulong performedById)
        {
            using var testContext = new TestContext()
            {
                UtcNow = performed,
                NextAdministrationActionId = actionId
            };

            var mockError = new Mock<IOperationError>();
            testContext.SetGuildUpdateError(mockError.Object);

            var uut = testContext.BuildUut();

            var result = await uut.DeleteAsync(
                guildId,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockAdministrationActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)CharacterManagementAdministrationActionType.GuildDeleted,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .UpdateAsync(
                    guildId,
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
                /*                  guildId,         actionId,       performed,                          performedById,  versionId       */
                new TestCaseData(   default(long),  default(long),  default(DateTimeOffset),            default(ulong), default(long)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue,  long.MinValue,  DateTimeOffset.MinValue,            ulong.MinValue, long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2L,             DateTimeOffset.Parse("2019-01-01"), 3UL,            4L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   5L,             6L,             DateTimeOffset.Parse("2019-01-02"), 7UL,            8L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   9L,             10L,            DateTimeOffset.Parse("2019-01-03"), 11UL,           12L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue,  DateTimeOffset.MaxValue,            ulong.MaxValue, long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(DeleteAsync_Success_TestCaseData))]
        public async Task DeleteAsync_UpdateAsyncSucceeds_PerformsDeleteAndWipesCacheAndReturnsSuccess(
            long guildId,
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

            testContext.SetGuildUpdateVersionId(versionId);

            var uut = testContext.BuildUut();

            var result = await uut.DeleteAsync(
                guildId,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockAdministrationActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)CharacterManagementAdministrationActionType.GuildDeleted,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .UpdateAsync(
                    guildId,
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

        #region DeleteDivisionAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> DeleteDivisionAsync_ValidationFailure_TestCaseData
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

        [TestCaseSource(nameof(DeleteDivisionAsync_ValidationFailure_TestCaseData))]
        public async Task DeleteDivisionAsync_GuildIdIsNotValid_ReturnsDataNotFound(
            long guildId,
            long divisionId,
            ulong performedById)
        {
            using var testContext = new TestContext();

            testContext.SetIsGuildIdActive(guildId, false);

            var uut = testContext.BuildUut();

            var result = await uut.DeleteDivisionAsync(
                guildId,
                divisionId,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockAdministrationActionsRepository.ShouldNotHaveReceived(x => x
                .CreateAsync(
                    It.IsAny<int>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<ulong>(),
                    It.IsAny<CancellationToken>()));

            testContext.MockCharacterGuildsRepository.ShouldNotHaveReceived(x => x
                .UpdateDivisionAsync(
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

        public static readonly IReadOnlyList<TestCaseData> DeleteDivisionAsync_UpdateFailure_TestCaseData
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

        [TestCaseSource(nameof(DeleteDivisionAsync_UpdateFailure_TestCaseData))]
        public async Task DeleteDivisionAsync_UpdateAsyncFails_ReturnsImmediately(
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

            var mockError = new Mock<IOperationError>();
            testContext.SetDivisionUpdateError(mockError.Object);

            var uut = testContext.BuildUut();

            var result = await uut.DeleteDivisionAsync(
                guildId,
                divisionId,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockAdministrationActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)CharacterManagementAdministrationActionType.DivisionDeleted,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .UpdateDivisionAsync(
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

        public static readonly IReadOnlyList<TestCaseData> DeleteDivisionAsync_Success_TestCaseData
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

        [TestCaseSource(nameof(DeleteDivisionAsync_Success_TestCaseData))]
        public async Task DeleteDivisionAsync_UpdateAsyncSucceeds_PerformsDeleteAndWipesCacheAndReturnsSuccess(
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

            var result = await uut.DeleteDivisionAsync(
                guildId,
                divisionId,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockAdministrationActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)CharacterManagementAdministrationActionType.DivisionDeleted,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .UpdateDivisionAsync(
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

        #endregion DeleteDivisionAsync() Tests

        #region GetCurrentDivisionIdentitiesAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> GetCurrentDivisionIdentitiesAsync_TestCaseData
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

        [TestCaseSource(nameof(GetCurrentDivisionIdentitiesAsync_TestCaseData))]
        public async Task GetCurrentDivisionIdentitiesAsync_Always_ReadsIdentities(
            long guildId)
        {
            using var testContext = new TestContext();

            var identities = new CharacterGuildDivisionIdentityViewModel[]
            {
                new CharacterGuildDivisionIdentityViewModel(
                    id:     default,
                    name:   string.Empty)
            };

            testContext.MockCharacterGuildsRepository
                .Setup(x => x.AsyncEnumerateDivisionIdentities(
                    It.IsAny<Optional<long>>(),
                    It.IsAny<Optional<bool>>()))
                .Returns(identities.ToAsyncEnumerable());

            var uut = testContext.BuildUut();

            var result = await uut.GetCurrentDivisionIdentitiesAsync(
                guildId,
                testContext.CancellationToken);

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .AsyncEnumerateDivisionIdentities(guildId, false));

            result.ShouldBeSetEqualTo(identities);
        }

        #endregion GetCurrentDivisionIdentitiesAsync() Tests

        #region GetCurrentDivisionIdentityAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> GetCurrentDivisionIdentityAsync_TestCaseData
            = new[]
            {
                /*                  guildId,        divisionId      */
                new TestCaseData(   default(long),  default(long)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue,  long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   3L,             4L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   5L,             6L              ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(GetCurrentDivisionIdentityAsync_TestCaseData))]
        public async Task GetCurrentDivisionIdentityAsync_Always_ReadsIdentity(
            long guildId,
            long divisionId)
        {
            using var testContext = new TestContext();

            var identity = new CharacterGuildDivisionIdentityViewModel(
                id: default,
                name: string.Empty);

            testContext.MockCharacterGuildsRepository
                .Setup(x => x.ReadDivisionIdentityAsync(
                    It.IsAny<long>(),
                    It.IsAny<Optional<long>>(),
                    It.IsAny<Optional<bool>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(identity.ToSuccess());

            var uut = testContext.BuildUut();

            var result = await uut.GetCurrentDivisionIdentityAsync(
                guildId,
                divisionId,
                testContext.CancellationToken);

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .ReadDivisionIdentityAsync(divisionId, guildId, false, testContext.CancellationToken));

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBeSameAs(identity);
        }

        #endregion GetCurrentDivisionIdentityAsync() Tests

        #region GetCurrentIdentitiesAsync() Tests

        [Test]
        public async Task GetCurrentIdentitiesAsync_Always_ReadsIdentities()
        {
            using var testContext = new TestContext();

            var identities = new CharacterGuildIdentityViewModel[]
            {
                new CharacterGuildIdentityViewModel(
                    id:     default,
                    name:   string.Empty)
            };

            testContext.MockCharacterGuildsRepository
                .Setup(x => x.AsyncEnumerateIdentities(
                    It.IsAny<Optional<bool>>()))
                .Returns(identities.ToAsyncEnumerable());

            var uut = testContext.BuildUut();

            var result = await uut.GetCurrentIdentitiesAsync(
                testContext.CancellationToken);

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .AsyncEnumerateIdentities(false));

            result.ShouldBeSetEqualTo(identities);
        }

        #endregion GetCurrentIdentitiesAsync() Tests

        #region GetCurrentIdentityAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> GetCurrentIdentityAsync_TestCaseData
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

        [TestCaseSource(nameof(GetCurrentIdentityAsync_TestCaseData))]
        public async Task GetCurrentIdentityAsync_Always_ReadsIdentity(
            long guildId)
        {
            using var testContext = new TestContext();

            var identity = new CharacterGuildIdentityViewModel(
                id: default,
                name: string.Empty);

            testContext.MockCharacterGuildsRepository
                .Setup(x => x.ReadIdentityAsync(
                    It.IsAny<long>(),
                    It.IsAny<Optional<bool>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(identity.ToSuccess());

            var uut = testContext.BuildUut();

            var result = await uut.GetCurrentIdentityAsync(
                guildId,
                testContext.CancellationToken);

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .ReadIdentityAsync(guildId, false, testContext.CancellationToken));

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBeSameAs(identity);
        }

        #endregion GetCurrentIdentityAsync() Tests

        #region UpdateAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> UpdateAsync_NameIsInUse_TestCaseData
            = new[]
            {
                /*                  guildId,        name,           performedById   */
                new TestCaseData(   default(long),  string.Empty,   default(ulong)  ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue,  string.Empty,   ulong.MinValue  ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             "Name 2",       3UL             ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   4L,             "Name 5",       6UL             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   7L,             "Name 8",       9UL             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  "MaxValue",     ulong.MaxValue  ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(UpdateAsync_NameIsInUse_TestCaseData))]
        public async Task UpdateAsync_NameIsInUse_ReturnsNameInUseError(
            long guildId,
            string name,
            ulong performedById)
        {
            using var testContext = new TestContext();

            testContext.SetIsNameInUse(name, true);

            var uut = testContext.BuildUut();

            var updateModel = new CharacterGuildUpdateModel()
            {
                Name = name,
            };

            var result = await uut.UpdateAsync(
                guildId,
                updateModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    default,
                    It.Is<Optional<IEnumerable<long>>>(y => y.IsSpecified && (y.Value != null) && y.Value.SetEquals(guildId.ToEnumerable())),
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
                /*                  guildId,        name,           performed,                          performedById,  actionId        */
                new TestCaseData(   default(long),  string.Empty,   default(DateTimeOffset),            default(ulong), default(long)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue,  string.Empty,   DateTimeOffset.MinValue,            ulong.MinValue, long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             "Name 2",       DateTimeOffset.Parse("2003-04-05"), 6UL,            7L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   8L,             "Name 9",       DateTimeOffset.Parse("2010-11-12"), 13UL,           14L             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   15L,            "Name 16",      DateTimeOffset.Parse("2017-06-19"), 20UL,           21L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  "MaxValue",     DateTimeOffset.MaxValue,            ulong.MaxValue, long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(UpdateAsync_UpdateFailure_TestCaseData))]
        public async Task UpdateAsync_UpdateAsyncFails_ReturnsImmediately(
            long guildId,
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

            testContext.SetIsNameInUse(name, false);

            var mockError = new Mock<IOperationError>();
            testContext.SetGuildUpdateError(mockError.Object);

            var uut = testContext.BuildUut();

            var updateModel = new CharacterGuildUpdateModel()
            {
                Name = name
            };

            var result = await uut.UpdateAsync(
                guildId,
                updateModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    default,
                    It.Is<Optional<IEnumerable<long>>>(y => y.IsSpecified && (y.Value != null) && y.Value.SetEquals(guildId.ToEnumerable())),
                    name,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockAdministrationActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)CharacterManagementAdministrationActionType.GuildModified,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .UpdateAsync(
                    guildId,
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
                /*                  guildId,        name,           performed,                          performedById,  actionId,       versionId       */
                new TestCaseData(   default(long),  string.Empty,   default(DateTimeOffset),            default(ulong), default(long),  default(long)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue,  string.Empty,   DateTimeOffset.MinValue,            ulong.MinValue, long.MinValue,  long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             "Name 2",       DateTimeOffset.Parse("2003-04-05"), 6UL,            7L,             8L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   9L,             "Name 10",      DateTimeOffset.Parse("2011-12-13"), 14UL,           15L,            16L             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   17L,            "Name 18",      DateTimeOffset.Parse("2019-08-21"), 22UL,           23L,            24L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  "MaxValue",     DateTimeOffset.MaxValue,            ulong.MaxValue, long.MaxValue,  long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(UpdateAsync_Success_TestCaseData))]
        public async Task UpdateAsync_UpdateSucceeds_PerformsUpdateAndWipesCacheAndReturnsSuccess(
            long guildId,
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

            testContext.SetIsNameInUse(name, false);
            testContext.SetGuildUpdateVersionId(versionId);

            var uut = testContext.BuildUut();

            var updateModel = new CharacterGuildUpdateModel()
            {
                Name = name
            };

            var result = await uut.UpdateAsync(
                guildId,
                updateModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    default,
                    It.Is<Optional<IEnumerable<long>>>(y => y.IsSpecified && (y.Value != null) && y.Value.SetEquals(guildId.ToEnumerable())),
                    name,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockAdministrationActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)CharacterManagementAdministrationActionType.GuildModified,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .UpdateAsync(
                    guildId,
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

        #region UpdateDivisionAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> UpdateDivisionAsync_ValidationFailure_TestCaseData
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

        [TestCaseSource(nameof(UpdateDivisionAsync_ValidationFailure_TestCaseData))]
        public async Task UpdateDivisionAsync_GuildIdIsNotActive_ReturnsDataNotFoundError(
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

            var result = await uut.UpdateDivisionAsync(
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

        [TestCaseSource(nameof(UpdateDivisionAsync_ValidationFailure_TestCaseData))]
        public async Task UpdateDivisionAsync_NameIsInUse_ReturnsNameInUseError(
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

            var result = await uut.UpdateDivisionAsync(
                guildId,
                divisionId,
                updateModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .AnyDivisionVersionsAsync(
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

        public static readonly IReadOnlyList<TestCaseData> UpdateDivisionAsync_UpdateFailure_TestCaseData
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

        [TestCaseSource(nameof(UpdateDivisionAsync_UpdateFailure_TestCaseData))]
        public async Task UpdateDivisionAsync_UpdateDivisionAsyncFails_ReturnsImmediately(
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

            var mockError = new Mock<IOperationError>();
            testContext.SetDivisionUpdateError(mockError.Object);

            var uut = testContext.BuildUut();

            var updateModel = new CharacterGuildDivisionUpdateModel()
            {
                Name = name
            };

            var result = await uut.UpdateDivisionAsync(
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

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .AnyDivisionVersionsAsync(
                    guildId,
                    It.Is<Optional<IEnumerable<long>>>(y => y.IsSpecified && (y.Value != null) && y.Value.SetEquals(divisionId.ToEnumerable())),
                    name,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockAdministrationActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)CharacterManagementAdministrationActionType.DivisionModified,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .UpdateDivisionAsync(
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

        public static readonly IReadOnlyList<TestCaseData> UpdateDivisionAsync_Success_TestCaseData
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

        [TestCaseSource(nameof(UpdateDivisionAsync_Success_TestCaseData))]
        public async Task UpdateDivisionAsync_UpdateSucceeds_PerformsUpdateAndWipesCacheAndReturnsSuccess(
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

            var result = await uut.UpdateDivisionAsync(
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

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .AnyDivisionVersionsAsync(
                    guildId,
                    It.Is<Optional<IEnumerable<long>>>(y => y.IsSpecified && (y.Value != null) && y.Value.SetEquals(divisionId.ToEnumerable())),
                    name,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockAdministrationActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)CharacterManagementAdministrationActionType.DivisionModified,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockCharacterGuildsRepository.ShouldHaveReceived(x => x
                .UpdateDivisionAsync(
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

        #endregion UpdateDivisionAsync() Tests
    }
}
