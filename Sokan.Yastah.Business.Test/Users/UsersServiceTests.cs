using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;

using Moq;
using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Business.Authorization;
using Sokan.Yastah.Business.Permissions;
using Sokan.Yastah.Business.Roles;
using Sokan.Yastah.Business.Users;
using Sokan.Yastah.Common.Messaging;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data;
using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Permissions;
using Sokan.Yastah.Data.Users;

using Sokan.Yastah.Common.Test;

namespace Sokan.Yastah.Business.Test.Users
{
    [TestFixture]
    public class UsersServiceTests
    {
        #region Test Context

        public class TestContext
            : AsyncMethodTestContext
        {
            public TestContext()
            {
                AuthorizationConfiguration = new AuthorizationConfiguration()
                {
                    AdminUserIds = Array.Empty<ulong>(),
                    MemberGuildIds = Array.Empty<ulong>()
                };

                MockAdministrationActionsRepository = new Mock<IAdministrationActionsRepository>();
                MockAdministrationActionsRepository
                    .Setup(x => x.CreateAsync(
                        It.IsAny<int>(),
                        It.IsAny<DateTimeOffset>(),
                        It.IsAny<ulong>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => NextAdministrationActionId);

                MemoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

                MockMessenger = new Mock<IMessenger>();

                MockAuthorizationConfigurationOptions = new Mock<IOptions<AuthorizationConfiguration>>();
                MockAuthorizationConfigurationOptions
                    .Setup(x => x.Value)
                    .Returns(() => AuthorizationConfiguration);

                MockPermissionsService = new Mock<IPermissionsService>();

                MockRolesService = new Mock<IRolesService>();

                MockSystemClock = new Mock<ISystemClock>();
                MockSystemClock
                    .Setup(x => x.UtcNow)
                    .Returns(() => UtcNow);

                MockTransactionScopeFactory = new Mock<ITransactionScopeFactory>();
                MockTransactionScopeFactory
                    .Setup(x => x.CreateScope(It.IsAny<IsolationLevel?>()))
                    .Returns(() => MockTransactionScope.Object);

                MockTransactionScope = new Mock<ITransactionScope>();

                MockUsersRepository = new Mock<IUsersRepository>();
            }

            public AuthorizationConfiguration AuthorizationConfiguration;
            public long NextAdministrationActionId;
            public DateTimeOffset UtcNow;

            public readonly Mock<IAdministrationActionsRepository> MockAdministrationActionsRepository;
            public readonly Mock<IOptions<AuthorizationConfiguration>> MockAuthorizationConfigurationOptions;
            public readonly MemoryCache MemoryCache;
            public readonly Mock<IMessenger> MockMessenger;
            public readonly Mock<IPermissionsService> MockPermissionsService;
            public readonly Mock<IRolesService> MockRolesService;
            public readonly Mock<ISystemClock> MockSystemClock;
            public readonly Mock<ITransactionScopeFactory> MockTransactionScopeFactory;
            public readonly Mock<ITransactionScope> MockTransactionScope;
            public readonly Mock<IUsersRepository> MockUsersRepository;

            public UsersService BuildUut()
                => new UsersService(
                    MockAdministrationActionsRepository.Object,
                    MockAuthorizationConfigurationOptions.Object,
                    MemoryCache,
                    MockMessenger.Object,
                    MockPermissionsService.Object,
                    MockRolesService.Object,
                    MockSystemClock.Object,
                    MockTransactionScopeFactory.Object,
                    MockUsersRepository.Object);

            public void SetRoleMemberIdsCache(
                    long roleId,
                    IReadOnlyCollection<ulong>? memberIds = null)
                => MemoryCache.Set(UsersService.MakeRoleMemberIdsCacheKey(roleId), memberIds ?? Array.Empty<ulong>());

            public void SetUserPermissionMappings(
                    ulong userId,
                    IEnumerable<(long mappingId, int permissionId, bool isDenied)> mappings)
                => MockUsersRepository
                    .Setup(x => x.AsyncEnumeratePermissionMappingIdentities(
                        It.IsAny<ulong>(),
                        It.IsAny<Optional<bool>>()))
                    .Returns(mappings
                        .Select(x => new UserPermissionMappingIdentityViewModel(
                            id:             x.mappingId,
                            userId:         userId,
                            permissionId:   x.permissionId,
                            isDenied:       x.isDenied))
                        .ToAsyncEnumerable());

            public void SetUserRoleMappings(
                    ulong userId,
                    IEnumerable<(long mappingId, long roleId)> mappings)
                => MockUsersRepository
                    .Setup(x => x.AsyncEnumerateRoleMappingIdentities(
                        It.IsAny<ulong>(),
                        It.IsAny<Optional<bool>>()))
                    .Returns(mappings
                        .Select(x => new UserRoleMappingIdentityViewModel(
                            id:     x.mappingId,
                            userId: userId,
                            roleId: x.roleId))
                        .ToAsyncEnumerable());

            public void SetValidatePermissionIdsResult(IOperationError? error = null)
                => MockPermissionsService
                    .Setup(x => x.ValidateIdsAsync(
                        It.IsAny<IReadOnlyCollection<int>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync((error is null)
                        ? OperationResult.Success
                        : error.ToError());

            public void SetValidateRoleIdsResult(IOperationError? error = null)
                => MockRolesService
                    .Setup(x => x.ValidateIdsAsync(
                        It.IsAny<IReadOnlyCollection<long>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync((error is null)
                        ? OperationResult.Success
                        : error.ToError());
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

        #endregion Test Data

        #region GetGrantedPermissionsAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> GetGrantedPermissionsAsync_UserIsAdmin_TestCaseData
            = new[]
            {
                /*                  userId,         adminUserIds                */
                new TestCaseData(   ulong.MinValue, new[] { ulong.MinValue }    ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            new[] { 1UL }               ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   2UL,            new[] { 2UL, 3UL }          ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   4UL,            new[] { 3UL, 4UL, 5UL }     ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, new[] { ulong.MaxValue }    ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(GetGrantedPermissionsAsync_UserIsAdmin_TestCaseData))]
        public async Task GetGrantedPermissionsAsync_UserIsAdmin_ReturnsAllPermissions(
            ulong userId,
            IReadOnlyList<ulong> adminUserIds)
        {
            using (var testContext = new TestContext())
            {
                testContext.AuthorizationConfiguration.AdminUserIds = adminUserIds.ToArray();

                var identities = new PermissionIdentityViewModel[] { };

                testContext.MockPermissionsService
                    .Setup(x => x.GetIdentitiesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(identities);

                var uut = testContext.BuildUut();

                var result = await uut.GetGrantedPermissionsAsync(
                    userId,
                    testContext.CancellationToken);

                result.IsSuccess.ShouldBeTrue();
                result.Value.ShouldBeSameAs(identities);

                testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                    .CreateScope(default));

                testContext.MockPermissionsService.ShouldHaveReceived(x => x
                    .GetIdentitiesAsync(testContext.CancellationToken));

                testContext.MockUsersRepository.Invocations.ShouldBeEmpty();

                testContext.MockTransactionScope.ShouldNotHaveReceived(x => x
                    .Complete());
                testContext.MockTransactionScope.ShouldHaveReceived(x => x
                    .Dispose());
            }
        }

        [TestCaseSource(nameof(UserId_TestCaseData))]
        public async Task GetGrantedPermissionsAsync_UserDoesNotExist_ReturnsDataNotFoundError(
            ulong userId)
        {
            using (var testContext = new TestContext())
            {
                testContext.MockUsersRepository
                    .Setup(x => x.AnyAsync(It.IsAny<CancellationToken>(), It.IsAny<Optional<ulong>>()))
                    .ReturnsAsync(false);

                var uut = testContext.BuildUut();

                var result = await uut.GetGrantedPermissionsAsync(
                    userId,
                    testContext.CancellationToken);

                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldBeOfType<DataNotFoundError>();
                result.Error.Message.ShouldContain(userId.ToString());

                testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                    .CreateScope(default));

                testContext.MockPermissionsService.Invocations.ShouldBeEmpty();

                testContext.MockUsersRepository.ShouldHaveReceived(x => x
                    .AnyAsync(testContext.CancellationToken, userId));

                testContext.MockTransactionScope.ShouldNotHaveReceived(x => x
                    .Complete());
                testContext.MockTransactionScope.ShouldHaveReceived(x => x
                    .Dispose());
            }
        }

        public static readonly IReadOnlyList<TestCaseData> GetGrantedPermissionsAsync_UserIsNotAdmin_TestCaseData
            = new[]
            {
                /*                  userId,         adminUserIds                */
                new TestCaseData(   ulong.MinValue, Array.Empty<ulong>()        ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            new[] { 2UL }               ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   3UL,            new[] { 4UL, 5UL }          ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   6UL,            new[] { 7UL, 8UL, 9UL }     ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, Array.Empty<ulong>()        ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(GetGrantedPermissionsAsync_UserIsNotAdmin_TestCaseData))]
        public async Task GetGrantedPermissionsAsync_UserIsNotAdmin_ReadsGrantedPermissionIdentities(
            ulong userId,
            IReadOnlyList<ulong> adminUserIds)
        {
            using (var testContext = new TestContext())
            {
                testContext.AuthorizationConfiguration.AdminUserIds = adminUserIds.ToArray();

                testContext.MockUsersRepository
                    .Setup(x => x.AnyAsync(It.IsAny<CancellationToken>(), It.IsAny<Optional<ulong>>()))
                    .ReturnsAsync(true);

                var identities = new PermissionIdentityViewModel[]
                {
                    new PermissionIdentityViewModel(
                        id:     default,
                        name:   string.Empty)
                };

                testContext.MockUsersRepository
                    .Setup(x => x.AsyncEnumerateGrantedPermissionIdentities(It.IsAny<ulong>()))
                    .Returns(identities.ToAsyncEnumerable());

                var uut = testContext.BuildUut();

                var result = await uut.GetGrantedPermissionsAsync(
                    userId,
                    testContext.CancellationToken);

                result.IsSuccess.ShouldBeTrue();
                result.Value.ShouldBeSetEqualTo(identities);

                testContext.MockPermissionsService.Invocations.ShouldBeEmpty();

                testContext.MockUsersRepository.ShouldHaveReceived(x => x
                    .AnyAsync(testContext.CancellationToken, userId));

                testContext.MockUsersRepository.ShouldHaveReceived(x => x
                    .AsyncEnumerateGrantedPermissionIdentities(userId));
            }
        }

        #endregion GetGrantedPermissionsAsync() Tests

        #region GetRoleMemberIdsAsync() Tests

        [TestCaseSource(nameof(RoleId_TestCaseData))]
        public async Task GetRoleMemberIdsAsync_IdsAreNotCached_ReadsIds(
            long roleId)
        {
            using (var testContext = new TestContext())
            {
                var memberIds = new ulong[]
                {
                    1UL,
                    2UL,
                    3UL
                };

                testContext.MockUsersRepository
                    .Setup(x => x.AsyncEnumerateIds(
                        It.IsAny<Optional<long>>()))
                    .Returns(memberIds.ToAsyncEnumerable());

                var uut = testContext.BuildUut();

                var result = await uut.GetRoleMemberIdsAsync(
                    roleId,
                    testContext.CancellationToken);

                testContext.MockUsersRepository.ShouldHaveReceived(x => x
                    .AsyncEnumerateIds(
                        roleId));

                testContext.MemoryCache.TryGetValue(UsersService.MakeRoleMemberIdsCacheKey(roleId), out var cacheValue)
                    .ShouldBeTrue();
                cacheValue.ShouldBeAssignableTo<IReadOnlyCollection<ulong>>()
                    .ShouldBeSetEqualTo(memberIds);

                result.ShouldBeSameAs(cacheValue);
            }
        }

        [TestCaseSource(nameof(RoleId_TestCaseData))]
        public async Task GetRoleMemberIdsAsync_IdsAreCached_DoesNotReadsIds(
            long roleId)
        {
            using (var testContext = new TestContext())
            {
                var memberIds = new ulong[] { };

                testContext.SetRoleMemberIdsCache(roleId, memberIds);

                var uut = testContext.BuildUut();

                var result = await uut.GetRoleMemberIdsAsync(
                    roleId,
                    testContext.CancellationToken);

                testContext.MockUsersRepository.ShouldNotHaveReceived(x => x
                    .AsyncEnumerateIds(
                        It.IsAny<Optional<long>>()));

                result.ShouldBeSameAs(memberIds);
            }
        }

        #endregion GetRoleMemberIdsAsync() Tests

        #region TrackUserAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> TrackUserAsync_InitializeUser_TestCaseData
            = new[]
            {
                /*                  userId,         username,       discriminator,      avatarHash,         uctNow,                             actionID,       defaultPermissionIds,   defaultRoleIds          */
                new TestCaseData(   ulong.MinValue, string.Empty,   string.Empty,       string.Empty,       DateTimeOffset.MinValue,            long.MinValue,  Array.Empty<int>(),     Array.Empty<long>()     ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            "username 2",   "discriminator 3",  "avatarHash 4",     DateTimeOffset.Parse("2019-05-06"), 7L,             new[] { 8 },            Array.Empty<long>()     ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   9UL,            "username 10",  "discriminator 11", "avatarHash 12",    DateTimeOffset.Parse("2019-01-14"), 15L,            Array.Empty<int>(),     new[] { 16L }           ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   17UL,           "username 18",  "discriminator 19", "avatarHash 20",    DateTimeOffset.Parse("2019-09-22"), 23L,            new[] { 24, 25, 26 },   new[] { 27L, 28L, 29L } ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, "MaxValue",     "MaxValue",         "MaxValue",         DateTimeOffset.MaxValue,            long.MaxValue,  new[] { int.MaxValue }, new[] { long.MaxValue } ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(TrackUserAsync_InitializeUser_TestCaseData))]
        public async Task TrackUserAsync_MergeResultIsInsert_InitializesUser(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            DateTimeOffset utcNow,
            long actionId,
            IReadOnlyList<int> defaultPermissionIds,
            IReadOnlyList<long> defaultRoleIds)
        {
            using (var testContext = new TestContext())
            {
                testContext.UtcNow = utcNow;
                testContext.NextAdministrationActionId = actionId;

                testContext.MockUsersRepository
                    .Setup(x => x.MergeAsync(
                        It.IsAny<ulong>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<DateTimeOffset>(),
                        It.IsAny<DateTimeOffset>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(MergeResult.SingleInsert);

                testContext.MockUsersRepository
                    .Setup(x => x.AsyncEnumerateDefaultPermissionIds())
                    .Returns(defaultPermissionIds.ToAsyncEnumerable());

                testContext.MockUsersRepository
                    .Setup(x => x.AsyncEnumerateDefaultRoleIds())
                    .Returns(defaultRoleIds.ToAsyncEnumerable());

                var uut = testContext.BuildUut();

                await uut.TrackUserAsync(
                    userId,
                    username,
                    discriminator,
                    avatarHash,
                    testContext.CancellationToken);

                testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                    .CreateScope(default));

                testContext.MockUsersRepository.ShouldHaveReceived(x => x
                    .MergeAsync(
                        userId,
                        username,
                        discriminator,
                        avatarHash,
                        utcNow,
                        utcNow,
                        testContext.CancellationToken));

                if (defaultPermissionIds.Any())
                    testContext.MockUsersRepository.ShouldHaveReceived(x => x
                        .CreatePermissionMappingsAsync(
                            userId,
                            It.Is<IEnumerable<int>>(y => (y != null) && y.SetEquals(defaultPermissionIds)),
                            PermissionMappingType.Granted,
                            actionId,
                            testContext.CancellationToken));

                if (defaultRoleIds.Any())
                    testContext.MockUsersRepository.ShouldHaveReceived(x => x
                        .CreateRoleMappingsAsync(
                            userId,
                            It.Is<IEnumerable<long>>(y => (y != null) && y.SetEquals(defaultRoleIds)),
                            actionId,
                            testContext.CancellationToken));

                testContext.MockMessenger.ShouldHaveReceived(x => x
                    .PublishNotificationAsync(
                        It.Is<UserInitializingNotification>(y => (y != null) && (y.UserId == userId) && (y.ActionId == y.ActionId)),
                        testContext.CancellationToken));

                testContext.MockTransactionScope.ShouldHaveReceived(x => x
                    .Complete());
                testContext.MockTransactionScope.ShouldHaveReceived(x => x
                    .Dispose());
            }
        }

        public static readonly IReadOnlyList<TestCaseData> TrackUserAsync_UpdateUser_TestCaseData
            = new[]
            {
                /*                  userId,         username,       discriminator,      avatarHash,         uctNow,                             */
                new TestCaseData(   ulong.MinValue, string.Empty,   string.Empty,       string.Empty,       DateTimeOffset.MinValue             ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            "username 2",   "discriminator 3",  "avatarHash 4",     DateTimeOffset.Parse("2019-05-06")  ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   7UL,            "username 8",   "discriminator 9",  "avatarHash 10",    DateTimeOffset.Parse("2019-11-12")  ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   13UL,           "username 14",  "discriminator 15", "avatarHash 16",    DateTimeOffset.Parse("2019-05-18")  ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, "MaxValue",     "MaxValue",         "MaxValue",         DateTimeOffset.MaxValue             ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(TrackUserAsync_UpdateUser_TestCaseData))]
        public async Task TrackUserAsync_MergeResultIsUpdate_DoesNotInitializeUser(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            DateTimeOffset utcNow)
        {
            using (var testContext = new TestContext())
            {
                testContext.UtcNow = utcNow;

                testContext.MockUsersRepository
                    .Setup(x => x.MergeAsync(
                        It.IsAny<ulong>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<DateTimeOffset>(),
                        It.IsAny<DateTimeOffset>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(MergeResult.SingleUpdate);

                var uut = testContext.BuildUut();

                await uut.TrackUserAsync(
                    userId,
                    username,
                    discriminator,
                    avatarHash,
                    testContext.CancellationToken);

                testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                    .CreateScope(default));

                testContext.MockUsersRepository.ShouldHaveReceived(x => x
                    .MergeAsync(
                        userId,
                        username,
                        discriminator,
                        avatarHash,
                        utcNow,
                        utcNow,
                        testContext.CancellationToken));

                testContext.MockUsersRepository.ShouldNotHaveReceived(x => x
                    .CreatePermissionMappingsAsync(
                        It.IsAny<ulong>(),
                        It.IsAny<IEnumerable<int>>(),
                        It.IsAny<PermissionMappingType>(),
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()));

                testContext.MockUsersRepository.ShouldNotHaveReceived(x => x
                    .CreateRoleMappingsAsync(
                        It.IsAny<ulong>(),
                        It.IsAny<IEnumerable<long>>(),
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()));

                testContext.MockMessenger.Invocations.ShouldBeEmpty();

                testContext.MockTransactionScope.ShouldHaveReceived(x => x
                    .Complete());
                testContext.MockTransactionScope.ShouldHaveReceived(x => x
                    .Dispose());
            }
        }

        #endregion TrackUserAsync() Tests

        #region UpdateAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> UpdateAsync_ValidationFailure_TestCaseData
            = new[]
            {
                /*                  userId,         performedById,  grantedPermissionIds,   deniedPermissionIds,    assignedRoleIds         */
                new TestCaseData(   ulong.MinValue, ulong.MinValue, new[] { int.MinValue }, new[] { int.MinValue }, new[] { long.MinValue } ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            2UL,            Array.Empty<int>(),     new[] { 3 },            new[] { 4L }            ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   5UL,            6UL,            new[] { 7 },            Array.Empty<int>(),     new[] { 8L, 9L }        ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   10UL,           11UL,           new[] { 12, 13 },       new[] { 14, 15 },       Array.Empty<long>()     ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, ulong.MaxValue, new[] { int.MaxValue }, new[] { int.MaxValue }, new[] { long.MaxValue } ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(UpdateAsync_ValidationFailure_TestCaseData))]
        public async Task UpdateAsync_PermissionsServiceValidateIdsAsyncFails_ReturnsFailure(
            ulong userId,
            ulong performedById,
            IReadOnlyList<int> grantedPermissionIds,
            IReadOnlyList<int> deniedPermissionIds,
            IReadOnlyList<long> assignedRoleIds)
        {
            using (var testContext = new TestContext())
            {
                var mockError = new Mock<IOperationError>();
                testContext.SetValidatePermissionIdsResult(mockError.Object);

                var uut = testContext.BuildUut();

                var updateModel = new UserUpdateModel()
                {
                    GrantedPermissionIds = grantedPermissionIds,
                    DeniedPermissionIds = deniedPermissionIds,
                    AssignedRoleIds = assignedRoleIds,
                };

                var result = await uut.UpdateAsync(
                    userId,
                    updateModel,
                    performedById,
                    testContext.CancellationToken);

                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldBeSameAs(mockError.Object);

                testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                    .CreateScope(default));

                testContext.MockPermissionsService.ShouldHaveReceived(x => x
                    .ValidateIdsAsync(
                        It.Is<IReadOnlyCollection<int>>(y => (y != null) && y.SetEquals(grantedPermissionIds.Union(deniedPermissionIds))),
                        testContext.CancellationToken));

                testContext.MockMessenger.Invocations.ShouldBeEmpty();

                testContext.MockTransactionScope.ShouldHaveReceived(x => x
                    .Dispose());
                testContext.MockTransactionScope.ShouldNotHaveReceived(x => x
                    .Complete());
            }
        }

        [TestCaseSource(nameof(UpdateAsync_ValidationFailure_TestCaseData))]
        public async Task UpdateAsync_RolesServiceServiceValidateIdsAsyncFails_ReturnsFailure(
            ulong userId,
            ulong performedById,
            IReadOnlyList<int> grantedPermissionIds,
            IReadOnlyList<int> deniedPermissionIds,
            IReadOnlyList<long> assignedRoleIds)
        {
            using (var testContext = new TestContext())
            {
                var mockError = new Mock<IOperationError>();
                testContext.SetValidatePermissionIdsResult();
                testContext.SetValidateRoleIdsResult(mockError.Object);

                var uut = testContext.BuildUut();

                var updateModel = new UserUpdateModel()
                {
                    GrantedPermissionIds = grantedPermissionIds,
                    DeniedPermissionIds = deniedPermissionIds,
                    AssignedRoleIds = assignedRoleIds,
                };

                var result = await uut.UpdateAsync(
                    userId,
                    updateModel,
                    performedById,
                    testContext.CancellationToken);

                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldBeSameAs(mockError.Object);

                testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                    .CreateScope(default));

                testContext.MockRolesService.ShouldHaveReceived(x => x
                    .ValidateIdsAsync(
                        It.Is<IReadOnlyCollection<long>>(y => (y != null) && y.SetEquals(assignedRoleIds)),
                        testContext.CancellationToken));

                testContext.MockMessenger.Invocations.ShouldBeEmpty();

                testContext.MockTransactionScope.ShouldHaveReceived(x => x
                    .Dispose());
                testContext.MockTransactionScope.ShouldNotHaveReceived(x => x
                    .Complete());
            }
        }

        public static readonly IReadOnlyList<TestCaseData> UpdateAsync_NoChangesGiven_TestCaseData
            = new[]
            {
                /*                  userId,         performedById,  permissionMappings,                             roleMappings                                */
                new TestCaseData(   ulong.MinValue, ulong.MinValue, new[] { (long.MinValue, int.MinValue, false) }, new[] { (long.MinValue, long.MinValue) }    ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            2UL,            Array.Empty<(long, int, bool)>(),               new[] { (3L, 4L) }                          ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   5UL,            6UL,            new[] { (7L, 8, false) },                       Array.Empty<(long, long)>()                 ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   9UL,            10UL,           new[] { (11L, 12, true), (13L, 14, false) },    new[] { (15L, 16L), (17L, 18L) }            ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, ulong.MaxValue, new[] { (long.MaxValue, int.MaxValue, false) }, new[] { (long.MaxValue, long.MaxValue) }    ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(UpdateAsync_NoChangesGiven_TestCaseData))]
        public async Task UpdateAsync_UpdateModelHasNoChanges_ReturnsNoChangesGivenError(
            ulong userId,
            ulong performedById,
            IReadOnlyList<(long mappingId, int permissionId, bool isDenied)> permissionMappings,
            IReadOnlyList<(long mappingId, long roleId)> roleMappings)
        {
            using (var testContext = new TestContext())
            {
                testContext.SetValidatePermissionIdsResult();
                testContext.SetValidateRoleIdsResult();
                testContext.SetUserPermissionMappings(userId, permissionMappings);
                testContext.SetUserRoleMappings(userId, roleMappings);

                var uut = testContext.BuildUut();

                var updateModel = new UserUpdateModel()
                {
                    GrantedPermissionIds = permissionMappings
                        .Where(x => !x.isDenied)
                        .Select(x => x.permissionId)
                        .ToArray(),
                    DeniedPermissionIds = permissionMappings
                        .Where(x => x.isDenied)
                        .Select(x => x.permissionId)
                        .ToArray(),
                    AssignedRoleIds = roleMappings
                        .Select(x => x.roleId)
                        .ToArray(),
                };

                var result = await uut.UpdateAsync(
                    userId,
                    updateModel,
                    performedById,
                    testContext.CancellationToken);

                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldBeOfType<NoChangesGivenError>();

                testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                    .CreateScope(default));

                testContext.MockUsersRepository.ShouldHaveReceived(x => x
                    .AsyncEnumeratePermissionMappingIdentities(
                        userId,
                        false));

                testContext.MockUsersRepository.ShouldHaveReceived(x => x
                    .AsyncEnumerateRoleMappingIdentities(
                        userId,
                        false));

                testContext.MockUsersRepository.ShouldNotHaveReceived(x => x
                    .CreatePermissionMappingsAsync(
                        It.IsAny<ulong>(),
                        It.IsAny<IEnumerable<int>>(),
                        It.IsAny<PermissionMappingType>(),
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()));

                testContext.MockUsersRepository.ShouldNotHaveReceived(x => x
                    .CreateRoleMappingsAsync(
                        It.IsAny<ulong>(),
                        It.IsAny<IEnumerable<long>>(),
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()));

                testContext.MockUsersRepository.ShouldNotHaveReceived(x => x
                    .UpdatePermissionMappingsAsync(
                        It.IsAny<IEnumerable<long>>(),
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()));

                testContext.MockUsersRepository.ShouldNotHaveReceived(x => x
                    .UpdateRoleMappingsAsync(
                        It.IsAny<IEnumerable<long>>(),
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()));

                testContext.MockMessenger.Invocations.ShouldBeEmpty();

                testContext.MockTransactionScope.ShouldHaveReceived(x => x
                    .Dispose());
                testContext.MockTransactionScope.ShouldNotHaveReceived(x => x
                    .Complete());
            }
        }

        public static readonly IReadOnlyList<TestCaseData> UpdateAsync_Success_TestCaseData
            = new[]
            {
                /*                  userId,         performedById,  utcNow,                             actionId,       grantedPermissionIds,   deniedPermissionIds,    assignedRoleIds,            permissionMappings,                                                             roleMappings,                               addedGrantedPermissionIds,  addedDeniedPermissionIds,   deletedPermissionMappingIds,    addedAssignedRoleIds,       deletedRoleMappingIds   */
                new TestCaseData(   ulong.MinValue, ulong.MinValue, DateTimeOffset.MinValue,            long.MinValue,  new[] { int.MinValue }, Array.Empty<int>(),     new[] { long.MinValue },    new[] { (long.MinValue, int.MinValue, false) },                                 Array.Empty<(long, long)>(),                Array.Empty<int>(),         Array.Empty<int>(),         Array.Empty<long>(),            new[] { long.MinValue },    Array.Empty<long>()     ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            2UL,            DateTimeOffset.Parse("2003-04-05"), 6L,             new[] { 7 },            new[] { 8 },            new[] { 9L },               new[] { (10L, 11, false) },                                                     new[] { (12L, 13L) },                       new[] { 7 },                new[] { 8 },                new[] { 10L },                  new[] { 9L },               new[] { 12L }           ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   14UL,           15UL,           DateTimeOffset.Parse("2016-05-18"), 19L,            new[] { 20, 21 },       new[] { 22, 23 },       new[] { 24L, 25L },         new[] { (26L, 20, false), (27L, 22, true) },                                    new[] { (28L, 24L) },                       new[] { 21 },               new[] { 23 },               Array.Empty<long>(),            new[] { 25L },              Array.Empty<long>()     ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   29UL,           30UL,           DateTimeOffset.Parse("2031-08-02"), 34L,            new[] { 35, 36, 37 },   new[] { 38, 39, 40 },   new[] { 41L, 42L, 43L },    Array.Empty<(long, int, bool)>(),                                               Array.Empty<(long, long)>(),                new[] { 35, 36, 37 },       new[] { 38, 39, 40 },       Array.Empty<long>(),            new[] { 41L, 42L, 43L },    Array.Empty<long>()     ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   44UL,           45UL,           DateTimeOffset.Parse("2046-11-18"), 49L,            new[] { 50 },           new[] { 51 },           new[] { 52L },              new[] { (53L, 50, false), (54L, 55, false), (56L, 51, true), (57L, 58, true) }, new[] { (59L, 52L), (60L, 61L) },           Array.Empty<int>(),         Array.Empty<int>(),         new[] { 54L, 57L },             Array.Empty<long>(),        new[] { 60L }           ).SetName("{m}(Unique Value Set 4)"),
                new TestCaseData(   62UL,           63UL,           DateTimeOffset.Parse("2064-05-04"), 67L,            new[] { 68, 69 },       new[] { 70, 71 },       new[] { 72L },              new[] { (73L, 68, false), (74L, 69, true), (75L, 70, false), (76L, 71, true) }, new[] { (77L, 72L) },                       new[] { 69 },               new[] { 70 },               new[] { 74L, 75L },             Array.Empty<long>(),        Array.Empty<long>()     ).SetName("{m}(Unique Value Set 5)"),
                new TestCaseData(   ulong.MaxValue, ulong.MaxValue, DateTimeOffset.MaxValue,            long.MaxValue,  Array.Empty<int>(),     new[] { int.MaxValue }, Array.Empty<long>(),        Array.Empty<(long, int, bool)>(),                                               new[] { (long.MaxValue, long.MaxValue) },   Array.Empty<int>(),         new[] { int.MaxValue },     Array.Empty<long>(),            Array.Empty<long>(),        new[] { long.MaxValue } ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(UpdateAsync_Success_TestCaseData))]
        public async Task UpdateAsync_PermissionMappingsOrRoleMappingsHaveChanged_PerformsMappingsAndWipesCacheAndReturnsSuccess(
            ulong userId,
            ulong performedById,
            DateTimeOffset utcNow,
            long actionId,
            IReadOnlyList<int> grantedPermissionIds,
            IReadOnlyList<int> deniedPermissionIds,
            IReadOnlyList<long> assignedRoleIds,
            IReadOnlyList<(long mappingId, int permissionId, bool isDenied)> permissionMappings,
            IReadOnlyList<(long mappingId, long roleId)> roleMappings,
            IReadOnlyList<int> addedGrantedPermissionIds,
            IReadOnlyList<int> addedDeniedPermissionIds,
            IReadOnlyList<long> deletedPermissionMappingIds,
            IReadOnlyList<long> addedAssignedRoleIds,
            IReadOnlyList<long> deletedRoleMappingIds)
        {
            using (var testContext = new TestContext())
            {
                testContext.UtcNow = utcNow;
                testContext.NextAdministrationActionId = actionId;

                testContext.SetValidatePermissionIdsResult();
                testContext.SetValidateRoleIdsResult();
                testContext.SetUserPermissionMappings(userId, permissionMappings);
                testContext.SetUserRoleMappings(userId, roleMappings);

                var allRoleIds = addedAssignedRoleIds
                    .Concat(roleMappings
                        .Select(x => x.roleId))
                    .ToArray();
                foreach(var roleId in allRoleIds)
                    testContext.MemoryCache.Set(UsersService.MakeRoleMemberIdsCacheKey(roleId), new object());

                var uut = testContext.BuildUut();

                var updateModel = new UserUpdateModel()
                {
                    GrantedPermissionIds = grantedPermissionIds,
                    DeniedPermissionIds = deniedPermissionIds,
                    AssignedRoleIds = assignedRoleIds,
                };

                var result = await uut.UpdateAsync(
                    userId,
                    updateModel,
                    performedById,
                    testContext.CancellationToken);

                result.IsSuccess.ShouldBeTrue();

                testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                    .CreateScope(default));

                testContext.MockAdministrationActionsRepository.ShouldHaveReceived(x => x
                    .CreateAsync(
                        (int)UserManagementAdministrationActionType.UserModified,
                        utcNow,
                        performedById,
                        testContext.CancellationToken));

                testContext.MockUsersRepository.ShouldHaveReceived(x => x
                    .AsyncEnumeratePermissionMappingIdentities(
                        userId,
                        false));

                testContext.MockUsersRepository.ShouldHaveReceived(x => x
                    .AsyncEnumerateRoleMappingIdentities(
                        userId,
                        false));

                if(addedGrantedPermissionIds.Any())
                    testContext.MockUsersRepository.ShouldHaveReceived(x => x
                        .CreatePermissionMappingsAsync(
                            userId,
                            It.Is<IEnumerable<int>>(y => (y != null) && y.SetEquals(addedGrantedPermissionIds)),
                            PermissionMappingType.Granted,
                            actionId,
                            testContext.CancellationToken));
                else
                    testContext.MockUsersRepository.ShouldNotHaveReceived(x => x
                        .CreatePermissionMappingsAsync(
                            It.IsAny<ulong>(),
                            It.IsAny<IEnumerable<int>>(),
                            PermissionMappingType.Granted,
                            It.IsAny<long>(),
                            It.IsAny<CancellationToken>()));

                if(addedDeniedPermissionIds.Any())
                    testContext.MockUsersRepository.ShouldHaveReceived(x => x
                        .CreatePermissionMappingsAsync(
                            userId,
                            It.Is<IEnumerable<int>>(y => (y != null) && y.SetEquals(addedDeniedPermissionIds)),
                            PermissionMappingType.Denied,
                            actionId,
                            testContext.CancellationToken));
                else
                    testContext.MockUsersRepository.ShouldNotHaveReceived(x => x
                        .CreatePermissionMappingsAsync(
                            It.IsAny<ulong>(),
                            It.IsAny<IEnumerable<int>>(),
                            PermissionMappingType.Denied,
                            It.IsAny<long>(),
                            It.IsAny<CancellationToken>()));

                if(addedAssignedRoleIds.Any())
                    testContext.MockUsersRepository.ShouldHaveReceived(x => x
                        .CreateRoleMappingsAsync(
                            userId,
                            It.Is<IEnumerable<long>>(y => (y != null) && y.SetEquals(addedAssignedRoleIds)),
                            actionId,
                            testContext.CancellationToken));
                else
                    testContext.MockUsersRepository.ShouldNotHaveReceived(x => x
                    .CreateRoleMappingsAsync(
                        It.IsAny<ulong>(),
                        It.IsAny<IEnumerable<long>>(),
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()));

                if(deletedPermissionMappingIds.Any())
                    testContext.MockUsersRepository.ShouldHaveReceived(x => x
                        .UpdatePermissionMappingsAsync(
                            It.Is<IEnumerable<long>>(y => (y != null) && y.SetEquals(deletedPermissionMappingIds)),
                            actionId,
                            testContext.CancellationToken));
                else
                    testContext.MockUsersRepository.ShouldNotHaveReceived(x => x
                    .UpdatePermissionMappingsAsync(
                        It.IsAny<IEnumerable<long>>(),
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()));

                if(deletedRoleMappingIds.Any())
                    testContext.MockUsersRepository.ShouldHaveReceived(x => x
                        .UpdateRoleMappingsAsync(
                            It.Is<IEnumerable<long>>(y => (y != null) && y.SetEquals(deletedRoleMappingIds)),
                            actionId,
                            testContext.CancellationToken));
                else
                    testContext.MockUsersRepository.ShouldNotHaveReceived(x => x
                    .UpdateRoleMappingsAsync(
                        It.IsAny<IEnumerable<long>>(),
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()));

                var modifiedRoleIds = addedAssignedRoleIds
                    .Concat(roleMappings
                        .Where(x => deletedRoleMappingIds.Contains(x.mappingId))
                        .Select(x => x.roleId))
                    .ToArray();
                foreach(var roleId in modifiedRoleIds)
                    testContext.MemoryCache.TryGetValue(UsersService.MakeRoleMemberIdsCacheKey(roleId), out _)
                        .ShouldBeFalse();

                var unmodifiedRoleIds = allRoleIds
                    .Except(modifiedRoleIds)
                    .ToArray();
                foreach(var roleId in unmodifiedRoleIds)
                    testContext.MemoryCache.TryGetValue(UsersService.MakeRoleMemberIdsCacheKey(roleId), out _)
                        .ShouldBeTrue();

                testContext.MockMessenger.ShouldHaveReceived(x => x
                    .PublishNotificationAsync(
                        It.Is<UserUpdatingNotification>(y => (y != null) && (y.UserId == userId) && (y.ActionId == actionId)),
                        testContext.CancellationToken));

                testContext.MockTransactionScope.ShouldHaveReceived(x => x
                    .Dispose());
                testContext.MockTransactionScope.ShouldHaveReceived(x => x
                    .Complete());
            }
        }

        #endregion UpdateAsync() Tests
    }
}
