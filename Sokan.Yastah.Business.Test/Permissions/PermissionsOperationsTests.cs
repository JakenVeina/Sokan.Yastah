﻿using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;
using Moq;
using Shouldly;

using Sokan.Yastah.Business.Authorization;
using Sokan.Yastah.Business.Permissions;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data.Permissions;

using Sokan.Yastah.Common.Test;

namespace Sokan.Yastah.Business.Test.Permissions
{
    [TestFixture]
    public class PermissionsOperationsTests
    {
        #region Test Context

        internal class TestContext
            : AsyncMethodTestContext
        {
            public TestContext()
            {
                MockAuthorizationService = new Mock<IAuthorizationService>();
                
                MockPermissionsService = new Mock<IPermissionsService>();
            }

            public readonly Mock<IAuthorizationService> MockAuthorizationService;
            public readonly Mock<IPermissionsService> MockPermissionsService;

            public PermissionsOperations BuildUut()
                => new PermissionsOperations(
                    MockAuthorizationService.Object,
                    MockPermissionsService.Object);
        }

        #endregion Test Context

        #region GetDescriptionsAsync() Tests

        [Test]
        public async Task GetDescriptionsAsync_RequirePermissionsAsyncFails_ReturnsImmediately()
        {
            using var testContext = new TestContext();
            
            var mockError = new Mock<IOperationError>();

            testContext.MockAuthorizationService
                .Setup(x => x.RequirePermissionsAsync(It.IsAny<CancellationToken>(), It.IsAny<int[]>()))
                .ReturnsAsync(OperationResult.FromError(mockError.Object));

            var uut = testContext.BuildUut();

            var result = await uut.GetDescriptionsAsync(
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    testContext.CancellationToken,
                    It.Is<int[]>(y => (y != null) && (y.Length != 0))));

            testContext.MockPermissionsService.Invocations.ShouldBeEmpty();
        }

        [Test]
        public async Task GetDescriptionsAsync_RequirePermissionsAsyncSucceeds_ReturnsGetDescriptionsAsync()
        {
            using var testContext = new TestContext();
            
            testContext.MockAuthorizationService
                .Setup(x => x.RequirePermissionsAsync(It.IsAny<CancellationToken>(), It.IsAny<int[]>()))
                .ReturnsAsync(OperationResult.Success);

            var descriptions = new PermissionCategoryDescriptionViewModel[0];

            testContext.MockPermissionsService
                .Setup(x => x.GetDescriptionsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(descriptions);

            var uut = testContext.BuildUut();

            var result = await uut.GetDescriptionsAsync(
                testContext.CancellationToken);

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBeSameAs(descriptions);

            testContext.MockAuthorizationService.ShouldHaveReceived(x => x
                .RequirePermissionsAsync(
                    testContext.CancellationToken,
                    It.Is<int[]>(y => (y != null) && (y.Length != 0))));

            testContext.MockPermissionsService.ShouldHaveReceived(x => x
                .GetDescriptionsAsync(testContext.CancellationToken));
        }

        #endregion GetDescriptionsAsync() Tests
    }
}
