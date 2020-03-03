using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;
using Moq;
using Shouldly;

using Sokan.Yastah.Business.Authentication;
using Sokan.Yastah.Business.Authorization;
using Sokan.Yastah.Business.Characters;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data.Characters;

using Sokan.Yastah.Common.Test;

namespace Sokan.Yastah.Business.Test.Characters
{
    [TestFixture]
    public class CharacterGuildsOperationsTests
    {
        #region Test Context

        internal class TestContext
            : AsyncMethodWithLoggerTestContext
        {
            public TestContext()
            {
                RequirePermissionsResult = OperationResult.Success;

                MockAuthenticationService = new Mock<IAuthenticationService>();
                MockAuthenticationService
                    .Setup(x => x.CurrentTicket)
                    .Returns(() => CurrentTicket);

                MockAuthorizationService = new Mock<IAuthorizationService>();
                MockAuthorizationService
                    .Setup(x => x.RequirePermissionsAsync(It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => RequirePermissionsResult);

                MockCharacterGuildsService = new Mock<ICharacterGuildsService>();
            }

            public AuthenticationTicket? CurrentTicket;

            public OperationResult RequirePermissionsResult;

            public readonly Mock<IAuthenticationService> MockAuthenticationService;
            public readonly Mock<IAuthorizationService> MockAuthorizationService;
            public readonly Mock<ICharacterGuildsService> MockCharacterGuildsService;

            public CharacterGuildsOperations BuildUut()
                => new CharacterGuildsOperations(
                    MockAuthenticationService.Object,
                    MockAuthorizationService.Object,
                    MockCharacterGuildsService.Object,
                    LoggerFactory.CreateLogger<CharacterGuildsOperations>());

            public void SetCurrentUserId(ulong userId)
                => CurrentTicket = new AuthenticationTicket(
                    default,
                    userId,
                    "username",
                    "discriminator",
                    "avatarHash",
                    new Dictionary<int, string>());
        }

        #endregion Test Context

        #region Test Data

        public static readonly IReadOnlyList<TestCaseData> GuildId_TestCaseData
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

        public static readonly IReadOnlyList<TestCaseData> CurrentUserId_GuildId_TestCaseData
            = new[]
            {
                /*                  currentUserId,  guildId         */
                new TestCaseData(   default(ulong), default(long)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   ulong.MinValue, long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            2L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   3UL,            4L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   5UL,            6L              ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, long.MaxValue   ).SetName("{m}(Max Values)")
            };

        #endregion Test Data

        #region CreateAsync() Tests

        [Test]
        public async Task CreateAsync_RequirePermissionsAsyncFails_ReturnsImmediately()
        {
            using var testContext = new TestContext();

            var mockError = new Mock<OperationError>("Mock Message");

            testContext.RequirePermissionsResult = OperationResult.FromError(mockError.Object);

            var uut = testContext.BuildUut();

            var creationModel = new CharacterGuildCreationModel();

            var result = await uut.CreateAsync(
                creationModel,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    It.Is<int[]>(y => (y != null) && (y.Length != 0)),
                    testContext.CancellationToken));

            testContext.MockCharacterGuildsService.Invocations.ShouldBeEmpty();
        }

        [TestCaseSource(nameof(CurrentUserId_GuildId_TestCaseData))]
        public async Task CreateAsync_RequirePermissionsAsyncSucceeds_ReturnsCreateAsync(
            ulong currentUserId,
            long guildId)
        {
            using var testContext = new TestContext();

            testContext.SetCurrentUserId(currentUserId);

            testContext.MockCharacterGuildsService
                .Setup(x => x.CreateAsync(It.IsAny<CharacterGuildCreationModel>(), It.IsAny<ulong>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(guildId);

            var uut = testContext.BuildUut();

            var creationModel = new CharacterGuildCreationModel();

            var result = await uut.CreateAsync(
                creationModel,
                testContext.CancellationToken);

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(guildId);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    It.Is<int[]>(y => (y != null) && (y.Length != 0)),
                    testContext.CancellationToken));

            testContext.MockCharacterGuildsService
                .ShouldHaveReceived(x => x.CreateAsync(creationModel, currentUserId, testContext.CancellationToken));
        }

        #endregion CreateAsync() Tests

        #region DeleteAsync() Tests

        [TestCaseSource(nameof(GuildId_TestCaseData))]
        public async Task DeleteAsync_RequirePermissionsAsyncFails_ReturnsImmediately(
            long guildId)
        {
            using var testContext = new TestContext();

            var mockError = new Mock<OperationError>("Mock Message");

            testContext.RequirePermissionsResult = OperationResult.FromError(mockError.Object);

            var uut = testContext.BuildUut();

            var result = await uut.DeleteAsync(
                guildId,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    It.Is<int[]>(y => (y != null) && (y.Length != 0)),
                    testContext.CancellationToken));

            testContext.MockCharacterGuildsService.Invocations.ShouldBeEmpty();
        }

        [TestCaseSource(nameof(CurrentUserId_GuildId_TestCaseData))]
        public async Task DeleteAsync_RequirePermissionsAsyncSucceeds_ReturnsDeleteAsync(
            ulong currentUserId,
            long guildId)
        {
            using var testContext = new TestContext();

            testContext.SetCurrentUserId(currentUserId);

            var mockError = new Mock<OperationError>("Mock Message");

            testContext.MockCharacterGuildsService
                .Setup(x => x.DeleteAsync(It.IsAny<long>(), It.IsAny<ulong>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(OperationResult.FromError(mockError.Object));

            var uut = testContext.BuildUut();

            var result = await uut.DeleteAsync(
                guildId,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    It.Is<int[]>(y => (y != null) && (y.Length != 0)),
                    testContext.CancellationToken));

            testContext.MockCharacterGuildsService.ShouldHaveReceived(x => x
                .DeleteAsync(guildId, currentUserId, testContext.CancellationToken));
        }

        #endregion DeleteAsync() Tests

        #region GetIdentitiesAsync() Tests

        [Test]
        public async Task GetIdentitiesAsync_RequirePermissionsAsyncFails_ReturnsImmediately()
        {
            using var testContext = new TestContext();
            
            var mockError = new Mock<OperationError>("Mock Message");

            testContext.RequirePermissionsResult = OperationResult.FromError(mockError.Object);

            var uut = testContext.BuildUut();

            var result = await uut.GetIdentitiesAsync(
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    It.Is<int[]>(y => (y != null) && (y.Length != 0)),
                    testContext.CancellationToken));

            testContext.MockCharacterGuildsService.Invocations.ShouldBeEmpty();
        }

        [Test]
        public async Task GetIdentitiesAsync_RequirePermissionsAsyncSucceeds_ReturnsGetCurrentIdentitiesAsync()
        {
            using var testContext = new TestContext();
            
            var identities = TestArray.Unique<CharacterGuildIdentityViewModel>();

            testContext.MockCharacterGuildsService
                .Setup(x => x.GetCurrentIdentitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(identities);

            var uut = testContext.BuildUut();

            var result = await uut.GetIdentitiesAsync(
                testContext.CancellationToken);

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBeSameAs(identities);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    It.Is<int[]>(y => (y != null) && (y.Length != 0)),
                    testContext.CancellationToken));

            testContext.MockCharacterGuildsService
                .ShouldHaveReceived(x => x.GetCurrentIdentitiesAsync(testContext.CancellationToken));
        }

        #endregion GetIdentitiesAsync() Tests

        #region UpdateAsync() Tests

        [TestCaseSource(nameof(GuildId_TestCaseData))]
        public async Task UpdateAsync_RequirePermissionsAsyncFails_ReturnsImmediately(
            long guildId)
        {
            using var testContext = new TestContext();

            var mockError = new Mock<OperationError>("Mock Message");

            testContext.RequirePermissionsResult = OperationResult.FromError(mockError.Object);

            var uut = testContext.BuildUut();

            var updateModel = new CharacterGuildUpdateModel();

            var result = await uut.UpdateAsync(
                guildId,
                updateModel,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    It.Is<int[]>(y => (y != null) && (y.Length != 0)),
                    testContext.CancellationToken));

            testContext.MockCharacterGuildsService.Invocations.ShouldBeEmpty();
        }

        [TestCaseSource(nameof(CurrentUserId_GuildId_TestCaseData))]
        public async Task UpdateAsync_RequirePermissionsAsyncSucceeds_ReturnsUpdateAsync(
            ulong currentUserId,
            long guildId)
        {
            using var testContext = new TestContext();

            testContext.SetCurrentUserId(currentUserId);

            var mockError = new Mock<OperationError>("Mock Message");

            testContext.MockCharacterGuildsService
                .Setup(x => x.UpdateAsync(It.IsAny<long>(), It.IsAny<CharacterGuildUpdateModel>(), It.IsAny<ulong>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(OperationResult.FromError(mockError.Object));

            var uut = testContext.BuildUut();

            var updateModel = new CharacterGuildUpdateModel();

            var result = await uut.UpdateAsync(
                guildId,
                updateModel,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    It.Is<int[]>(y => (y != null) && (y.Length != 0)),
                    testContext.CancellationToken));

            testContext.MockCharacterGuildsService.ShouldHaveReceived(x => x
                .UpdateAsync(guildId, updateModel, currentUserId, testContext.CancellationToken));
        }

        #endregion UpdateAsync() Tests
    }
}
