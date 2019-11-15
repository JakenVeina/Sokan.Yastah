using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;
using Moq;
using Shouldly;

using Sokan.Yastah.Business.Authentication;
using Sokan.Yastah.Business.Authorization;
using Sokan.Yastah.Business.Permissions;
using Sokan.Yastah.Data.Permissions;

using Sokan.Yastah.Common.Test;

namespace Sokan.Yastah.Business.Test.Authorization
{
    [TestFixture]
    public class AuthorizationServiceTests
    {
        #region Test Context

        public class TestContext
            : AsyncMethodTestContext
        {
            public TestContext()
            {
                CurrentTicket = new AuthenticationTicket(
                    id: 1,
                    userId: 2,
                    username: "username",
                    discriminator: "discriminator",
                    avatarHash: "avatarHash",
                    new Dictionary<int, string>()
                    {
                        { 3, "grantedPermission" }
                    });

                PermissionIdentities = Array.Empty<PermissionIdentityViewModel>();

                MockAuthenticationService = new Mock<IAuthenticationService>();
                MockAuthenticationService
                    .Setup(x => x.CurrentTicket)
                    .Returns(() => CurrentTicket);

                MockPermissionsService = new Mock<IPermissionsService>();
                MockPermissionsService
                    .Setup(x => x.GetIdentitiesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => PermissionIdentities);
            }

            public AuthenticationTicket? CurrentTicket;

            public IReadOnlyList<PermissionIdentityViewModel> PermissionIdentities;
            public readonly Mock<IAuthenticationService> MockAuthenticationService;
            public readonly Mock<IPermissionsService> MockPermissionsService;

            public AuthorizationService BuildUut()
                => new AuthorizationService(
                    MockAuthenticationService.Object,
                    MockPermissionsService.Object);

            public void SetCurrentTicket(IEnumerable<int> grantedPermissionIds)
                => CurrentTicket = new AuthenticationTicket(
                    CurrentTicket!.Id,
                    CurrentTicket.UserId,
                    CurrentTicket.Username,
                    CurrentTicket.Discriminator,
                    CurrentTicket.AvatarHash,
                    grantedPermissionIds.ToDictionary(
                        id => id,
                        id => PermissionIdentities.First(x => x.Id == id).Name));

            public void SetPermissionIdentities(IEnumerable<int> permissionIds)
                => PermissionIdentities = permissionIds
                    .Select(id => new PermissionIdentityViewModel(
                        id:     id,
                        name:   $"Permission {id}"))
                    .ToArray();
        }

        #endregion Test Context

        #region RequireAuthentication() Tests

        [Test]
        public void RequireAuthentication_CurrentTicketIsNull_ReturnsUnauthenticatedUserError()
        {
            using (var testContext = new TestContext())
            {
                testContext.CurrentTicket = null;

                var uut = testContext.BuildUut();

                var result = uut.RequireAuthentication();

                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldBeOfType<UnauthenticatedUserError>();
            }
        }

        [Test]
        public void RequireAuthentication_CurrentTicketIsNotNull_ReturnsSuccess()
        {
            using (var testContext = new TestContext())
            {
                var uut = testContext.BuildUut();

                var result = uut.RequireAuthentication();

                result.IsSuccess.ShouldBeTrue();
            }
        }

        #endregion RequireAuthentication() Tests

        #region RequirePermissionsAsync() Tests

        internal static readonly IReadOnlyList<TestCaseData> RequirePermissionsAsync_CurrentTicketIsNull_TestCaseData
            = new[]
            {
                /*                  permissionIds       */
                new TestCaseData(   Array.Empty<int>()  ),
                new TestCaseData(   new[] { 0 }         ),
                new TestCaseData(   new[] { 1 }         ),
                new TestCaseData(   new[] { 2, 3 }      ),
                new TestCaseData(   new[] { 4, 5, 6 }   )
            };

        [TestCaseSource(nameof(RequirePermissionsAsync_CurrentTicketIsNull_TestCaseData))]
        public async Task RequirePermissionsAsync_CurrentTicketIsNull_ReturnsUnauthenticatedUserError(
            int[] permissionIds)
        {
            using (var testContext = new TestContext())
            {
                testContext.CurrentTicket = null;

                var uut = testContext.BuildUut();

                var result = await uut.RequirePermissionsAsync(
                    testContext.CancellationToken,
                    permissionIds);

                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldBeOfType<UnauthenticatedUserError>();

                testContext.MockPermissionsService.Invocations.ShouldBeEmpty();
            }
        }

        internal static readonly IReadOnlyList<TestCaseData> RequirePermissionsAsync_CurrentTicketDoesNotHavePermissions_TestCaseData
            = new[]
            {
                /*                  permissionIds           grantedPermissionIds    */
                new TestCaseData(   new[] { 1 },            Array.Empty<int>()      ).SetName("{m}(missing: 1)"),
                new TestCaseData(   new[] { 2 },            new[] { 3 }             ).SetName("{m}(missing: 2)"),
                new TestCaseData(   new[] { 4, 5 },         new[] { 6, 7 }          ).SetName("{m}(missing: 4, 5)"),
                new TestCaseData(   new[] { 8, 9 },         new[] { 8, 10 }         ).SetName("{m}(missing: 9)"),
                new TestCaseData(   new[] { 11, 12, 13 },   new[] { 14, 15, 16 }    ).SetName("{m}(missing: 11, 12, 13)")
            };

        [TestCaseSource(nameof(RequirePermissionsAsync_CurrentTicketDoesNotHavePermissions_TestCaseData))]
        public async Task RequirePermissionsAsync_CurrentTicketDoesNotHavePermissions_ReturnsInsufficientPermissionsError(
            int[] permissionIds,
            IReadOnlyList<int> grantedPermissionIds)
        {
            using (var testContext = new TestContext())
            {
                testContext.SetPermissionIdentities(grantedPermissionIds.Union(permissionIds));
                testContext.SetCurrentTicket(grantedPermissionIds);

                var uut = testContext.BuildUut();

                var result = await uut.RequirePermissionsAsync(
                    testContext.CancellationToken,
                    permissionIds);

                result.IsFailure.ShouldBeTrue();
                var error = result.Error.ShouldBeOfType<InsufficientPermissionsError>();
                error.MissingPermissions.Select(x => x.Key)
                    .ShouldBeSetEqualTo(permissionIds.Except(grantedPermissionIds));
                error.MissingPermissions.ForEach(permission =>
                {
                    var identity = testContext.PermissionIdentities.First(x => x.Id == permission.Key);

                    permission.Value.ShouldBe(identity.Name);
                });

                testContext.MockPermissionsService.ShouldHaveReceived(x => x
                    .GetIdentitiesAsync(testContext.CancellationToken));
            }
        }

        internal static readonly IReadOnlyList<TestCaseData> RequirePermissionsAsync_CurrentTicketHasPermissions_TestCaseData
            = new[]
            {
                /*                  permissionIds           grantedPermissionIds    */
                new TestCaseData(   Array.Empty<int>(),     Array.Empty<int>()      ).SetName("{m}(No grants)"),
                new TestCaseData(   Array.Empty<int>(),     new[] { 0 }             ).SetName("{m}(No required permissions)"),
                new TestCaseData(   new[] { 1 },            new[] { 1 }             ).SetName("{m}(Perfect match)"),
                new TestCaseData(   new[] { 2 },            new[] { 2, 3 }          ).SetName("{m}(One extra permission)"),
                new TestCaseData(   new[] { 5, 6 },         new[] { 5, 6 }          ).SetName("{m}(Perfect match, multiple required)"),
                new TestCaseData(   new[] { 5, 6 },         new[] { 5, 6, 7 }       ).SetName("{m}(One extra permission, multiple required)")
            };

        [TestCaseSource(nameof(RequirePermissionsAsync_CurrentTicketHasPermissions_TestCaseData))]
        public async Task RequirePermissionsAsync_CurrentTicketHasPermissions_ReturnsSuccess(
            int[] permissionIds,
            IReadOnlyList<int> grantedPermissionIds)
        {
            using (var testContext = new TestContext())
            {
                testContext.SetPermissionIdentities(grantedPermissionIds.Union(permissionIds));
                testContext.SetCurrentTicket(grantedPermissionIds);

                var uut = testContext.BuildUut();

                var result = await uut.RequirePermissionsAsync(
                    testContext.CancellationToken,
                    permissionIds);

                result.IsSuccess.ShouldBeTrue();

                testContext.MockPermissionsService.Invocations.ShouldBeEmpty();
            }
        }


        #endregion RequirePermissionsAsync() Tests
    }
}
