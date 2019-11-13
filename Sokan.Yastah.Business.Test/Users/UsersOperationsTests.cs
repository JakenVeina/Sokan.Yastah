using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Moq;
using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Business.Authentication;
using Sokan.Yastah.Business.Authorization;
using Sokan.Yastah.Business.Users;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data.Users;

using Sokan.Yastah.Common.Test;

namespace Sokan.Yastah.Business.Test.Users
{
    [TestFixture]
    public class UsersOperationsTests
    {
        #region Test Context

        public class TestContext
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

                MockUsersRepository = new Mock<IUsersRepository>();

                MockUsersService = new Mock<IUsersService>();
            }

            public AuthenticationTicket CurrentTicket;

            public OperationResult RequirePermissionsResult;

            public readonly Mock<IAuthenticationService> MockAuthenticationService;
            public readonly Mock<IAuthorizationService> MockAuthorizationService;
            public readonly Mock<IUsersRepository> MockUsersRepository;
            public readonly Mock<IUsersService> MockUsersService;

            public UsersOperations BuildUut()
                => new UsersOperations(
                    MockAuthenticationService.Object,
                    MockAuthorizationService.Object,
                    MockUsersRepository.Object,
                    MockUsersService.Object);

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

        public static readonly IReadOnlyList<TestCaseData> UserId_TestCaseData
            = new[]
            {
                /*                  userId          */
                new TestCaseData(   ulong.MinValue  ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL             ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   2UL             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   3UL             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue  ).SetName("{m}(Max Values)")
            };

        public static readonly IReadOnlyList<TestCaseData> CurrentUserId_UserId_TestCaseData
            = new[]
            {
                /*                  currentUserId,  userId          */
                new TestCaseData(   ulong.MinValue, ulong.MinValue  ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            2UL             ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   3UL,            4UL             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   5UL,            6UL             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, ulong.MaxValue  ).SetName("{m}(Max Values)")
            };

        #endregion Test Data

        #region GetDetailAsync() Tests

        [TestCaseSource(nameof(UserId_TestCaseData))]
        public async Task GetDetailAsync_RequirePermissionsAsyncFails_ReturnsImmediately(
            ulong userId)
        {
            using (var testContext = new TestContext())
            {
                var mockError = new Mock<IOperationError>();

                testContext.RequirePermissionsResult = OperationResult.FromError(mockError.Object);

                var uut = testContext.BuildUut();

                var result = await uut.GetDetailAsync(
                    userId,
                    testContext.CancellationToken);

                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldBeSameAs(mockError.Object);

                testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                    .RequirePermissionsAsync(
                        testContext.CancellationToken,
                        It.Is<int[]>(y => (y != null) && (y.Length != 0))));

                testContext.MockUsersService.Invocations.ShouldBeEmpty();
                testContext.MockUsersRepository.Invocations.ShouldBeEmpty();
            }
        }

        [TestCaseSource(nameof(UserId_TestCaseData))]
        public async Task GetDetailAsync_RequirePermissionsAsyncSucceeds_ReturnsReadDetailAsync(
            ulong userId)
        {
            using (var testContext = new TestContext())
            {
                var detail = new UserDetailViewModel();

                testContext.MockUsersRepository
                    .Setup(x => x.ReadDetailAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(detail);

                var uut = testContext.BuildUut();

                var result = await uut.GetDetailAsync(
                    userId,
                    testContext.CancellationToken);

                result.IsSuccess.ShouldBeTrue();
                result.Value.ShouldBeSameAs(detail);

                testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                    .RequirePermissionsAsync(
                        testContext.CancellationToken,
                        It.Is<int[]>(y => (y != null) && (y.Length != 0))));

                testContext.MockUsersRepository.ShouldHaveReceived(x => x
                    .ReadDetailAsync(userId, testContext.CancellationToken));

                testContext.MockUsersService.Invocations.ShouldBeEmpty();
            }
        }

        #endregion GetDetailAsync() Tests

        #region GetOverviewsAsync() Tests

        [Test]
        public async Task GetOverviewsAsync_RequirePermissionsAsyncFails_ReturnsImmediately()
        {
            using (var testContext = new TestContext())
            {
                var mockError = new Mock<IOperationError>();

                testContext.RequirePermissionsResult = OperationResult.FromError(mockError.Object);

                var uut = testContext.BuildUut();

                var result = await uut.GetOverviewsAsync(
                    testContext.CancellationToken);

                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldBeSameAs(mockError.Object);

                testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                    .RequirePermissionsAsync(
                        testContext.CancellationToken,
                        It.Is<int[]>(y => (y != null) && (y.Length != 0))));

                testContext.MockUsersService.Invocations.ShouldBeEmpty();
                testContext.MockUsersRepository.Invocations.ShouldBeEmpty();
            }
        }

        [Test]
        public async Task GetOverviewsAsync_RequirePermissionsAsyncSucceeds_ReturnsGetCurrentOverviewsAsync()
        {
            using (var testContext = new TestContext())
            {
                var overviews = new UserOverviewViewModel[]
                {
                    new UserOverviewViewModel()
                };

                testContext.MockUsersRepository
                    .Setup(x => x.AsyncEnumerateOverviews())
                    .Returns(overviews.ToAsyncEnumerable());

                var uut = testContext.BuildUut();

                var result = await uut.GetOverviewsAsync(
                    testContext.CancellationToken);

                result.IsSuccess.ShouldBeTrue();
                result.Value.ShouldBeSetEqualTo(overviews);

                testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                    .RequirePermissionsAsync(
                        testContext.CancellationToken,
                        It.Is<int[]>(y => (y != null) && (y.Length != 0))));

                testContext.MockUsersService.Invocations.ShouldBeEmpty();

                testContext.MockUsersRepository
                    .ShouldHaveReceived(x => x.AsyncEnumerateOverviews());
            }
        }

        #endregion GetOverviewsAsync() Tests

        #region UpdateAsync() Tests

        [TestCaseSource(nameof(UserId_TestCaseData))]
        public async Task UpdateAsync_RequirePermissionsAsyncFails_ReturnsImmediately(
            ulong userId)
        {
            using (var testContext = new TestContext())
            {
                var mockError = new Mock<IOperationError>();

                testContext.RequirePermissionsResult = OperationResult.FromError(mockError.Object);

                var uut = testContext.BuildUut();

                var updateModel = new UserUpdateModel();

                var result = await uut.UpdateAsync(
                    userId,
                    updateModel,
                    testContext.CancellationToken);

                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldBeSameAs(mockError.Object);

                testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                    .RequirePermissionsAsync(
                        testContext.CancellationToken,
                        It.Is<int[]>(y => (y != null) && (y.Length != 0))));

                testContext.MockUsersService.Invocations.ShouldBeEmpty();
                testContext.MockUsersRepository.Invocations.ShouldBeEmpty();
            }
        }

        [TestCaseSource(nameof(CurrentUserId_UserId_TestCaseData))]
        public async Task UpdateAsync_RequirePermissionsAsyncSucceeds_ReturnsUpdateAsync(
            ulong currentUserId,
            ulong userId)
        {
            using (var testContext = new TestContext())
            {
                testContext.SetCurrentUserId(currentUserId);

                var mockError = new Mock<IOperationError>();

                testContext.MockUsersService
                    .Setup(x => x.UpdateAsync(It.IsAny<ulong>(), It.IsAny<UserUpdateModel>(), It.IsAny<ulong>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(OperationResult.FromError(mockError.Object));

                var uut = testContext.BuildUut();

                var updateModel = new UserUpdateModel();

                var result = await uut.UpdateAsync(
                    userId,
                    updateModel,
                    testContext.CancellationToken);

                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldBeSameAs(mockError.Object);

                testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                    .RequirePermissionsAsync(
                        testContext.CancellationToken,
                        It.Is<int[]>(y => (y != null) && (y.Length != 0))));

                testContext.MockUsersService.ShouldHaveReceived(x => x
                    .UpdateAsync(userId, updateModel, currentUserId, testContext.CancellationToken));

                testContext.MockUsersRepository.Invocations.ShouldBeEmpty();
            }
        }

        #endregion UpdateAsync() Tests
    }
}
