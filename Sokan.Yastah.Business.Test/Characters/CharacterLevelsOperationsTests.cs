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
    public class CharacterLevelsOperationsTests
    {
        #region Test Context

        internal class TestContext
            : AsyncMethodTestContext
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
                    .Setup(x => x.RequirePermissionsAsync(It.IsAny<CancellationToken>(), It.IsAny<int[]>()))
                    .ReturnsAsync(() => RequirePermissionsResult);

                MockCharacterLevelsService = new Mock<ICharacterLevelsService>();
            }

            public AuthenticationTicket? CurrentTicket;

            public OperationResult RequirePermissionsResult;

            public readonly Mock<IAuthenticationService> MockAuthenticationService;
            public readonly Mock<IAuthorizationService> MockAuthorizationService;
            public readonly Mock<ICharacterLevelsService> MockCharacterLevelsService;

            public CharacterLevelsOperations BuildUut()
                => new CharacterLevelsOperations(
                    MockAuthenticationService.Object,
                    MockAuthorizationService.Object,
                    MockCharacterLevelsService.Object);

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

        public static readonly IReadOnlyList<TestCaseData> CurrentUserId_TestCaseData
            = new[]
            {
                /*                  currentUserId   */
                new TestCaseData(   default(ulong)  ).SetName("{m}(Default Values)"),
                new TestCaseData(   ulong.MinValue  ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL             ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   2UL             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   3UL             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue  ).SetName("{m}(Max Values)")
            };

        #endregion Test Data

        #region GetDefinitionsAsync() Tests

        [Test]
        public async Task GetDefinitionsAsync_RequirePermissionsAsyncFails_ReturnsImmediately()
        {
            using var testContext = new TestContext();
            
            var mockError = new Mock<IOperationError>();

            testContext.RequirePermissionsResult = OperationResult.FromError(mockError.Object);

            var uut = testContext.BuildUut();

            var result = await uut.GetDefinitionsAsync(
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    testContext.CancellationToken,
                    It.Is<int[]>(y => (y != null) && (y.Length != 0))));

            testContext.MockCharacterLevelsService.Invocations.ShouldBeEmpty();
        }

        [Test]
        public async Task GetDefinitionsAsync_RequirePermissionsAsyncSucceeds_ReturnsGetCurrentDefinitionsAsync()
        {
            using var testContext = new TestContext();
            
            var identities = TestArray.Unique<CharacterLevelDefinitionViewModel>();

            testContext.MockCharacterLevelsService
                .Setup(x => x.GetCurrentDefinitionsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(identities);

            var uut = testContext.BuildUut();

            var result = await uut.GetDefinitionsAsync(
                testContext.CancellationToken);

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBeSameAs(identities);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    testContext.CancellationToken,
                    It.Is<int[]>(y => (y != null) && (y.Length != 0))));

            testContext.MockCharacterLevelsService
                .ShouldHaveReceived(x => x.GetCurrentDefinitionsAsync(testContext.CancellationToken));
        }

        #endregion GetDefinitionsAsync() Tests

        #region UpdateExperienceDiffsAsync() Tests

        [Test]
        public async Task UpdateExperienceDiffsAsync_RequirePermissionsAsyncFails_ReturnsImmediately()
        {
            using var testContext = new TestContext();

            var mockError = new Mock<IOperationError>();

            testContext.RequirePermissionsResult = OperationResult.FromError(mockError.Object);

            var uut = testContext.BuildUut();

            var experienceThresholds = TestArray.Unique<int>();

            var result = await uut.UpdateExperienceDiffsAsync(
                experienceThresholds,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    testContext.CancellationToken,
                    It.Is<int[]>(y => (y != null) && (y.Length != 0))));

            testContext.MockCharacterLevelsService.Invocations.ShouldBeEmpty();
        }

        [TestCaseSource(nameof(CurrentUserId_TestCaseData))]
        public async Task UpdateExperienceDiffsAsync_RequirePermissionsAsyncSucceeds_ReturnsUpdateExperienceDiffsAsync(
            ulong currentUserId)
        {
            using var testContext = new TestContext();

            testContext.SetCurrentUserId(currentUserId);

            var mockError = new Mock<IOperationError>();

            testContext.MockCharacterLevelsService
                .Setup(x => x.UpdateExperienceDiffsAsync(
                    It.IsAny<IReadOnlyList<int>>(),
                    It.IsAny<ulong>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(OperationResult.FromError(mockError.Object));

            var uut = testContext.BuildUut();

            var experienceThresholds = TestArray.Unique<int>();

            var result = await uut.UpdateExperienceDiffsAsync(
                experienceThresholds,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    testContext.CancellationToken,
                    It.Is<int[]>(y => (y != null) && (y.Length != 0))));

            testContext.MockCharacterLevelsService.ShouldHaveReceived(x => x
                .UpdateExperienceDiffsAsync(
                    experienceThresholds,
                    currentUserId,
                    testContext.CancellationToken));
        }

        #endregion UpdateExperienceDiffsAsync() Tests
    }
}
