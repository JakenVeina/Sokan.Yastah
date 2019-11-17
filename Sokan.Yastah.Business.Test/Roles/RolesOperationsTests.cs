using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;
using Moq;
using Shouldly;

using Sokan.Yastah.Business.Authentication;
using Sokan.Yastah.Business.Authorization;
using Sokan.Yastah.Business.Roles;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data.Roles;

using Sokan.Yastah.Common.Test;

namespace Sokan.Yastah.Business.Test.Roles
{
    [TestFixture]
    public class RolesOperationsTests
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

                MockRolesRepository = new Mock<IRolesRepository>();

                MockRolesService = new Mock<IRolesService>();
            }

            public AuthenticationTicket? CurrentTicket;

            public OperationResult RequirePermissionsResult;

            public readonly Mock<IAuthenticationService> MockAuthenticationService;
            public readonly Mock<IAuthorizationService> MockAuthorizationService;
            public readonly Mock<IRolesRepository> MockRolesRepository;
            public readonly Mock<IRolesService> MockRolesService;

            public RolesOperations BuildUut()
                => new RolesOperations(
                    MockAuthenticationService.Object,
                    MockAuthorizationService.Object,
                    MockRolesRepository.Object,
                    MockRolesService.Object);

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

        public static readonly IReadOnlyList<TestCaseData> RoleId_TestCaseData
            = new[]
            {
                /*                  roleId          */
                new TestCaseData(   long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   2L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   3L              ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue   ).SetName("{m}(Max Values)")
            };

        public static readonly IReadOnlyList<TestCaseData> CurrentUserId_RoleId_TestCaseData
            = new[]
            {
                /*                  currentUserId,  roleId          */
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
            
            var mockError = new Mock<IOperationError>();

            testContext.RequirePermissionsResult = OperationResult.FromError(mockError.Object);

            var uut = testContext.BuildUut();

            var creationModel = new RoleCreationModel();

            var result = await uut.CreateAsync(
                creationModel,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    testContext.CancellationToken,
                    It.Is<int[]>(y => (y != null) && (y.Length != 0))));

            testContext.MockRolesService.Invocations.ShouldBeEmpty();
            testContext.MockRolesRepository.Invocations.ShouldBeEmpty();
        }

        [TestCaseSource(nameof(CurrentUserId_RoleId_TestCaseData))]
        public async Task CreateAsync_RequirePermissionsAsyncSucceeds_ReturnsCreateAsync(
            ulong currentUserId,
            long roleId)
        {
            using var testContext = new TestContext();
            
            testContext.SetCurrentUserId(currentUserId);

            testContext.MockRolesService
                .Setup(x => x.CreateAsync(It.IsAny<RoleCreationModel>(), It.IsAny<ulong>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(roleId);

            var uut = testContext.BuildUut();

            var creationModel = new RoleCreationModel();

            var result = await uut.CreateAsync(
                creationModel,
                testContext.CancellationToken);

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(roleId);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    testContext.CancellationToken,
                    It.Is<int[]>(y => (y != null) && (y.Length != 0))));

            testContext.MockRolesService
                .ShouldHaveReceived(x => x.CreateAsync(creationModel, currentUserId, testContext.CancellationToken));

            testContext.MockRolesRepository.Invocations.ShouldBeEmpty();
        }

        #endregion CreateAsync() Tests

        #region DeleteAsync() Tests

        [TestCaseSource(nameof(RoleId_TestCaseData))]
        public async Task DeleteAsync_RequirePermissionsAsyncFails_ReturnsImmediately(
            long roleId)
        {
            using var testContext = new TestContext();
            
            var mockError = new Mock<IOperationError>();

            testContext.RequirePermissionsResult = OperationResult.FromError(mockError.Object);

            var uut = testContext.BuildUut();

            var result = await uut.DeleteAsync(
                roleId,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    testContext.CancellationToken,
                    It.Is<int[]>(y => (y != null) && (y.Length != 0))));

            testContext.MockRolesService.Invocations.ShouldBeEmpty();
            testContext.MockRolesRepository.Invocations.ShouldBeEmpty();
        }

        [TestCaseSource(nameof(CurrentUserId_RoleId_TestCaseData))]
        public async Task DeleteAsync_RequirePermissionsAsyncSucceeds_ReturnsDeleteAsync(
            ulong currentUserId,
            long roleId)
        {
            using var testContext = new TestContext();
            
            testContext.SetCurrentUserId(currentUserId);

            var mockError = new Mock<IOperationError>();

            testContext.MockRolesService
                .Setup(x => x.DeleteAsync(It.IsAny<long>(), It.IsAny<ulong>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(OperationResult.FromError(mockError.Object));

            var uut = testContext.BuildUut();

            var result = await uut.DeleteAsync(
                roleId,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    testContext.CancellationToken,
                    It.Is<int[]>(y => (y != null) && (y.Length != 0))));

            testContext.MockRolesService.ShouldHaveReceived(x => x
                .DeleteAsync(roleId, currentUserId, testContext.CancellationToken));

            testContext.MockRolesRepository.Invocations.ShouldBeEmpty();
        }

        #endregion DeleteAsync() Tests

        #region GetDetailAsync() Tests

        [TestCaseSource(nameof(RoleId_TestCaseData))]
        public async Task GetDetailAsync_RequirePermissionsAsyncFails_ReturnsImmediately(
            long roleId)
        {
            using var testContext = new TestContext();
            
            var mockError = new Mock<IOperationError>();

            testContext.RequirePermissionsResult = OperationResult.FromError(mockError.Object);

            var uut = testContext.BuildUut();

            var result = await uut.GetDetailAsync(
                roleId,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    testContext.CancellationToken,
                    It.Is<int[]>(y => (y != null) && (y.Length != 0))));

            testContext.MockRolesService.Invocations.ShouldBeEmpty();
            testContext.MockRolesRepository.Invocations.ShouldBeEmpty();
        }

        [TestCaseSource(nameof(RoleId_TestCaseData))]
        public async Task GetDetailAsync_RequirePermissionsAsyncSucceeds_ReturnsReadDetailAsync(
            long roleId)
        {
            using var testContext = new TestContext();
            
            var detail = new RoleDetailViewModel(
                id: default,
                name: string.Empty,
                grantedPermissionIds: Array.Empty<int>());

            testContext.MockRolesRepository
                .Setup(x => x.ReadDetailAsync(
                    It.IsAny<long>(),
                    It.IsAny<Optional<bool>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(detail);

            var uut = testContext.BuildUut();

            var result = await uut.GetDetailAsync(
                roleId,
                testContext.CancellationToken);

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBeSameAs(detail);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    testContext.CancellationToken,
                    It.Is<int[]>(y => (y != null) && (y.Length != 0))));

            testContext.MockRolesRepository.ShouldHaveReceived(x => x
                .ReadDetailAsync(roleId, false, testContext.CancellationToken));

            testContext.MockRolesService.Invocations.ShouldBeEmpty();
        }

        #endregion GetIdentitiesAsync() Tests

        #region GetIdentitiesAsync() Tests

        [Test]
        public async Task GetIdentitiesAsync_RequirePermissionsAsyncFails_ReturnsImmediately()
        {
            using var testContext = new TestContext();
            
            var mockError = new Mock<IOperationError>();

            testContext.RequirePermissionsResult = OperationResult.FromError(mockError.Object);

            var uut = testContext.BuildUut();

            var result = await uut.GetIdentitiesAsync(
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    testContext.CancellationToken,
                    It.Is<int[]>(y => (y != null) && (y.Length != 0))));

            testContext.MockRolesService.Invocations.ShouldBeEmpty();
            testContext.MockRolesRepository.Invocations.ShouldBeEmpty();
        }

        [Test]
        public async Task GetIdentitiesAsync_RequirePermissionsAsyncSucceeds_ReturnsGetCurrentIdentitiesAsync()
        {
            using var testContext = new TestContext();
            
            var identities = TestArray.Unique<RoleIdentityViewModel>();

            testContext.MockRolesService
                .Setup(x => x.GetCurrentIdentitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(identities);

            var uut = testContext.BuildUut();

            var result = await uut.GetIdentitiesAsync(
                testContext.CancellationToken);

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBeSameAs(identities);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    testContext.CancellationToken,
                    It.Is<int[]>(y => (y != null) && (y.Length != 0))));

            testContext.MockRolesService
                .ShouldHaveReceived(x => x.GetCurrentIdentitiesAsync(testContext.CancellationToken));

            testContext.MockRolesRepository.Invocations.ShouldBeEmpty();
        }

        #endregion GetIdentitiesAsync() Tests

        #region UpdateAsync() Tests

        [TestCaseSource(nameof(RoleId_TestCaseData))]
        public async Task UpdateAsync_RequirePermissionsAsyncFails_ReturnsImmediately(
            long roleId)
        {
            using var testContext = new TestContext();
            
            var mockError = new Mock<IOperationError>();

            testContext.RequirePermissionsResult = OperationResult.FromError(mockError.Object);

            var uut = testContext.BuildUut();

            var updateModel = new RoleUpdateModel();

            var result = await uut.UpdateAsync(
                roleId,
                updateModel,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    testContext.CancellationToken,
                    It.Is<int[]>(y => (y != null) && (y.Length != 0))));

            testContext.MockRolesService.Invocations.ShouldBeEmpty();
            testContext.MockRolesRepository.Invocations.ShouldBeEmpty();
        }

        [TestCaseSource(nameof(CurrentUserId_RoleId_TestCaseData))]
        public async Task UpdateAsync_RequirePermissionsAsyncSucceeds_ReturnsUpdateAsync(
            ulong currentUserId,
            long roleId)
        {
            using var testContext = new TestContext();
            
            testContext.SetCurrentUserId(currentUserId);

            var mockError = new Mock<IOperationError>();

            testContext.MockRolesService
                .Setup(x => x.UpdateAsync(It.IsAny<long>(), It.IsAny<RoleUpdateModel>(), It.IsAny<ulong>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(OperationResult.FromError(mockError.Object));

            var uut = testContext.BuildUut();

            var updateModel = new RoleUpdateModel();

            var result = await uut.UpdateAsync(
                roleId,
                updateModel,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    testContext.CancellationToken,
                    It.Is<int[]>(y => (y != null) && (y.Length != 0))));

            testContext.MockRolesService.ShouldHaveReceived(x => x
                .UpdateAsync(roleId, updateModel, currentUserId, testContext.CancellationToken));

            testContext.MockRolesRepository.Invocations.ShouldBeEmpty();
        }

        #endregion UpdateAsync() Tests
    }
}
