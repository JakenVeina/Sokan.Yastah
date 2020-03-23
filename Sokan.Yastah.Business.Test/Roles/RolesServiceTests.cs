using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using NUnit.Framework;
using Moq;
using Shouldly;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;

using Sokan.Yastah.Business.Permissions;
using Sokan.Yastah.Business.Roles;
using Sokan.Yastah.Common.Messaging;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data;
using Sokan.Yastah.Data.Auditing;
using Sokan.Yastah.Data.Roles;

using Sokan.Yastah.Common.Test;

namespace Sokan.Yastah.Business.Test.Roles
{
    [TestFixture]
    public class RolesServiceTests
    {
        #region Test Context

        internal class TestContext
            : AsyncMethodWithLoggerFactoryAndMemoryCacheTestContext
        {
            public TestContext()
            {
                MockAuditableActionsRepository = new Mock<IAuditableActionsRepository>();
                MockAuditableActionsRepository
                    .Setup(x => x.CreateAsync(
                        It.IsAny<int>(),
                        It.IsAny<DateTimeOffset>(),
                        It.IsAny<ulong?>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => NextAdministrationActionId);

                MockMessenger = new Mock<IMessenger>();
                
                MockPermissionsService = new Mock<IPermissionsService>();
                
                MockRolesRepository = new Mock<IRolesRepository>();
                MockRolesRepository
                    .Setup(x => x.CreateAsync(
                        It.IsAny<string>(),
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => NextRoleId);

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
            public long NextRoleId;

            public readonly Mock<IAuditableActionsRepository> MockAuditableActionsRepository;
            public readonly Mock<IMessenger> MockMessenger;
            public readonly Mock<IPermissionsService> MockPermissionsService;
            public readonly Mock<IRolesRepository> MockRolesRepository;
            public readonly Mock<ISystemClock> MockSystemClock;
            public readonly Mock<ITransactionScopeFactory> MockTransactionScopeFactory;
            public readonly Mock<ITransactionScope> MockTransactionScope;

            public RolesService BuildUut()
                => new RolesService(
                    MockAuditableActionsRepository.Object,
                    LoggerFactory.CreateLogger<RolesService>(),
                    MemoryCache,
                    MockMessenger.Object,
                    MockPermissionsService.Object,
                    MockRolesRepository.Object,
                    MockSystemClock.Object,
                    MockTransactionScopeFactory.Object);

            public void SetIsNameInUse(bool isNameInUse)
                => MockRolesRepository
                    .Setup(x => x.AnyVersionsAsync(
                        It.IsAny<Optional<IEnumerable<long>>>(),
                        It.IsAny<Optional<string>>(),
                        It.IsAny<Optional<bool>>(),
                        It.IsAny<Optional<bool>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(isNameInUse);

            public void SetRoleUpdateError(OperationError error)
                => MockRolesRepository
                    .Setup(x => x.UpdateAsync(
                        It.IsAny<long>(),
                        It.IsAny<long>(),
                        It.IsAny<Optional<string>>(),
                        It.IsAny<Optional<bool>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(error);

            public void SetRoleUpdateVersionId(long versionId)
                => MockRolesRepository
                    .Setup(x => x.UpdateAsync(
                        It.IsAny<long>(),
                        It.IsAny<long>(),
                        It.IsAny<Optional<string>>(),
                        It.IsAny<Optional<bool>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(versionId);

            public void SetPermissionMappingIdentities(long roleId, IEnumerable<(
                    long mappingId,
                    int permissionId)> permissionMappings)
                => MockRolesRepository
                    .Setup(x => x.AsyncEnumeratePermissionMappingIdentities(
                        It.IsAny<long>(),
                        It.IsAny<Optional<bool>>()))
                    .Returns(permissionMappings.Select(x =>
                        new RolePermissionMappingIdentityViewModel(
                            id:             x.mappingId,
                            roleId:         roleId,
                            permissionId:   x.permissionId))
                        .ToAsyncEnumerable());

            public void SetCurrentIdentitiesCache(IReadOnlyCollection<RoleIdentityViewModel>? identities = null)
                => MemoryCache.Set(RolesService._getCurrentIdentitiesCacheKey, identities ?? Array.Empty<RoleIdentityViewModel>());

            public void SetCurrentIdentitiesCache(IReadOnlyCollection<long> roleIds)
                => SetCurrentIdentitiesCache(roleIds
                    .Select(x => new RoleIdentityViewModel(
                        id:     x,
                        name:   $"Role {x}"))
                    .ToArray());

            public void SetValidatePermissionIdsResult(OperationError? error = null)
                => MockPermissionsService
                    .Setup(x => x.ValidateIdsAsync(
                        It.IsAny<IReadOnlyCollection<int>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync((error is null)
                        ? OperationResult.Success
                        : error);
        }

        #endregion Test Context

        #region CreateAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> CreateAsync_NameIsInUse_TestCaseData
            = new[]
            {
                /*                  performedById,  name            */
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
            
            testContext.SetIsNameInUse(true);
            testContext.SetCurrentIdentitiesCache();

            var uut = testContext.BuildUut();

            var creationModel = new RoleCreationModel()
            {
                Name = name
            };

            var result = await uut.CreateAsync(
                creationModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockRolesRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    default,
                    name,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockMessenger.Invocations.ShouldBeEmpty();

            testContext.MemoryCache.TryGetValue(RolesService._getCurrentIdentitiesCacheKey, out _)
                .ShouldBeTrue();

            testContext.MockTransactionScope.ShouldNotHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            result.IsFailure.ShouldBeTrue();
            var error = result.Error.ShouldBeOfType<NameInUseError>();
            error.Message.Contains(name);
        }

        public static readonly IReadOnlyList<TestCaseData> CreateAsync_InvalidPermissionIds_TestCaseData
            = new[]
            {
                /*                  performedById,  name,           grantedPermissionIds    */
                new TestCaseData(   ulong.MinValue, string.Empty,   new[] { int.MinValue }  ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            "Name 2",       new[] { 3, 4 }          ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   5UL,            "Name 6",       new[] { 7, 8, 9 }       ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   10UL,           "Name 11",      new[] { 12, 13, 14 }    ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, "Max Value",    new[] { int.MaxValue }  ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(CreateAsync_InvalidPermissionIds_TestCaseData))]
        public async Task CreateAsync_PermissionsServiceValidateIdsAsyncFails_ReturnsFailure(
            ulong performedById,
            string name,
            IReadOnlyList<int> grantedPermissionIds)
        {
            using var testContext = new TestContext();
            
            testContext.SetIsNameInUse(false);
            testContext.SetCurrentIdentitiesCache();

            var mockError = new Mock<OperationError>("Mock Message");
            testContext.SetValidatePermissionIdsResult(mockError.Object);

            var uut = testContext.BuildUut();

            var creationModel = new RoleCreationModel()
            {
                Name = name,
                GrantedPermissionIds = grantedPermissionIds
            };

            var result = await uut.CreateAsync(
                creationModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockPermissionsService.ShouldHaveReceived(x => x
                .ValidateIdsAsync(
                    It.Is<IReadOnlyCollection<int>>(y => (y != null) && y.SetEquals(grantedPermissionIds)),
                    testContext.CancellationToken));

            testContext.MockMessenger.Invocations.ShouldBeEmpty();

            testContext.MemoryCache.TryGetValue(RolesService._getCurrentIdentitiesCacheKey, out _)
                .ShouldBeTrue();

            testContext.MockTransactionScope.ShouldNotHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);
        }

        public static readonly IReadOnlyList<TestCaseData> CreateAsync_Success_TestCaseData
            = new[]
            {
                /*                  actionId,       performed,                          performedById,  name,           grantedPermissionIds,   roleId          */
                new TestCaseData(   long.MinValue,  DateTimeOffset.MinValue,            ulong.MinValue, string.Empty,   new[] { int.MinValue }, long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             DateTimeOffset.Parse("2019-01-01"), 2UL,            "Name 3",       Array.Empty<int>(),     4L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   5L,             DateTimeOffset.Parse("2019-01-02"), 6UL,            "Name 7",       new[] { 8, },           9L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   10L,            DateTimeOffset.Parse("2019-01-03"), 11UL,           "Name 12",      new[] { 13, 14, 15 },   16L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  DateTimeOffset.MaxValue,            ulong.MaxValue, "Max Value",    new[] { int.MaxValue }, long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(CreateAsync_Success_TestCaseData))]
        public async Task CreateAsync_CreationModelIsValid_PerformesCreatesAndWipesCacheAndReturnsSuccess(
            long actionId,
            DateTimeOffset performed,
            ulong performedById,
            string name,
            IReadOnlyList<int> grantedPermissionIds,
            long roleId)
        {
            using var testContext = new TestContext()
            {
                UtcNow = performed,
                NextAdministrationActionId = actionId,
                NextRoleId = roleId
            };

            testContext.SetIsNameInUse(false);
            testContext.SetCurrentIdentitiesCache();
            testContext.SetValidatePermissionIdsResult();

            var uut = testContext.BuildUut();

            var creationModel = new RoleCreationModel()
            {
                Name = name,
                GrantedPermissionIds = grantedPermissionIds
            };

            var result = await uut.CreateAsync(
                creationModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockRolesRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    default,
                    name,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockAuditableActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)RoleManagementAdministrationActionType.RoleCreated,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockRolesRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    name,
                    actionId,
                    testContext.CancellationToken));

            foreach (var permissionId in grantedPermissionIds)
                testContext.MockRolesRepository.ShouldHaveReceived(x => x
                    .CreatePermissionMappingsAsync(
                        roleId,
                        It.Is<IEnumerable<int>>(y => (y != null) && y.SetEquals(grantedPermissionIds)),
                        actionId,
                        testContext.CancellationToken));

            testContext.MockMessenger.Invocations.ShouldBeEmpty();

            testContext.MemoryCache.TryGetValue(RolesService._getCurrentIdentitiesCacheKey, out _)
                .ShouldBeFalse();

            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(roleId);
        }

        #endregion CreateAsync() Tests

        #region DeleteAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> DeleteAsync_Failure_TestCaseData
            = new[]
            {
                /*                  roleId,         actionId,       performed,                          performedById,  */
                new TestCaseData(   long.MinValue,  long.MinValue,  DateTimeOffset.MinValue,            ulong.MinValue  ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2L,             DateTimeOffset.Parse("2019-01-01"), 3UL             ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   4L,             5L,             DateTimeOffset.Parse("2019-01-02"), 6UL             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   7L,             8L,             DateTimeOffset.Parse("2019-01-03"), 9UL             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue,  DateTimeOffset.MaxValue,            ulong.MaxValue  ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(DeleteAsync_Failure_TestCaseData))]
        public async Task DeleteAsync_UpdateAsyncFails_ReturnsImmediatels(
            long roleId,
            long actionId,
            DateTimeOffset performed,
            ulong performedById)
        {
            using var testContext = new TestContext()
            {
                UtcNow = performed,
                NextAdministrationActionId = actionId
            };

            testContext.SetCurrentIdentitiesCache();

            var mockError = new Mock<OperationError>("Mock Message");
            testContext.SetRoleUpdateError(mockError.Object);

            var uut = testContext.BuildUut();

            var result = await uut.DeleteAsync(
                roleId,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockAuditableActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)RoleManagementAdministrationActionType.RoleDeleted,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockRolesRepository.ShouldHaveReceived(x => x
                .UpdateAsync(
                    roleId,
                    actionId,
                    Optional<string>.Unspecified,
                    true,
                    testContext.CancellationToken));

            testContext.MockMessenger.Invocations.ShouldBeEmpty();

            testContext.MemoryCache.TryGetValue(RolesService._getCurrentIdentitiesCacheKey, out _)
                .ShouldBeTrue();

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
                /*                  roleId,         actionId,       performed,                          performedById,  versionId       */
                new TestCaseData(   long.MinValue,  long.MinValue,  DateTimeOffset.MinValue,            ulong.MinValue, long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2L,             DateTimeOffset.Parse("2019-01-01"), 3UL,            4L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   5L,             6L,             DateTimeOffset.Parse("2019-01-02"), 7UL,            8L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   9L,             10L,            DateTimeOffset.Parse("2019-01-03"), 11UL,           12L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue,  DateTimeOffset.MaxValue,            ulong.MaxValue, long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(DeleteAsync_Success_TestCaseData))]
        public async Task DeleteAsync_UpdateAsyncSucceeds_PerformsDeleteAndWipesCacheAndReturnsSuccess(
            long roleId,
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

            testContext.SetRoleUpdateVersionId(versionId);
            testContext.SetCurrentIdentitiesCache();

            var uut = testContext.BuildUut();

            var result = await uut.DeleteAsync(
                roleId,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockAuditableActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)RoleManagementAdministrationActionType.RoleDeleted,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockRolesRepository.ShouldHaveReceived(x => x
                .UpdateAsync(
                    roleId,
                    actionId,
                    Optional<string>.Unspecified,
                    true,
                    testContext.CancellationToken));

            testContext.MockMessenger.ShouldHaveReceived(x => x
                .PublishNotificationAsync(
                    It.Is<RoleDeletingNotification>(y => (y != null) && (y.RoleId == roleId) && (y.ActionId == actionId)),
                    testContext.CancellationToken));

            testContext.MemoryCache.TryGetValue(RolesService._getCurrentIdentitiesCacheKey, out _)
                .ShouldBeFalse();

            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            result.IsSuccess.ShouldBeTrue();
        }

        #endregion DeleteAsync() Tests

        #region GetCurrentIdentitiesAsync() Tests

        [Test]
        public async Task GetCurrentIdentitiesAsync_IdentitiesAreNotCached_ReadsIdentities()
        {
            using var testContext = new TestContext();
            
            var identities = new RoleIdentityViewModel[]
            {
                new RoleIdentityViewModel(
                    id:     default,
                    name:   string.Empty)
            };

            testContext.MockRolesRepository
                .Setup(x => x.AsyncEnumerateIdentities(
                    It.IsAny<Optional<bool>>()))
                .Returns(identities.ToAsyncEnumerable());

            var uut = testContext.BuildUut();

            var result = await uut.GetCurrentIdentitiesAsync(
                testContext.CancellationToken);

            testContext.MockRolesRepository.ShouldHaveReceived(x => x
                .AsyncEnumerateIdentities(false));

            testContext.MemoryCache.TryGetValue(RolesService._getCurrentIdentitiesCacheKey, out var cacheValue)
                .ShouldBeTrue();
            cacheValue.ShouldBeAssignableTo<IReadOnlyCollection<RoleIdentityViewModel>>()
                .ShouldBeSetEqualTo(identities);

            result.ShouldBeSameAs(cacheValue);
        }

        [Test]
        public async Task GetCurrentIdentitiesAsync_IdentitiesAreCached_DoesNotReadIdentities()
        {
            using var testContext = new TestContext();
            
            var identities = TestArray.Unique<RoleIdentityViewModel>();

            testContext.SetCurrentIdentitiesCache(identities);

            var uut = testContext.BuildUut();

            var result = await uut.GetCurrentIdentitiesAsync(
                testContext.CancellationToken);

            testContext.MockRolesRepository.ShouldNotHaveReceived(x => x
                .AsyncEnumerateIdentities(It.IsAny<Optional<bool>>()));

            result.ShouldBeSameAs(identities);
        }

        #endregion GetCurrentIdentitiesAsync() Tests

        #region UpdateAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> UpdateAsync_NameIsInUse_TestCaseData
            = new[]
            {
                /*                  roleId,         name,           grantedPermissionIds,   performedById   */
                new TestCaseData(   long.MinValue,  string.Empty,   Array.Empty<int>(),     ulong.MinValue  ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             "Name 2",       new[] { 3 },            4UL             ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   5L,             "Name 6",       new[] { 7, 8 },         9UL             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   10L,            "Name 11",      new[] { 12, 13, 14 },   15UL            ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  "Max Values",   new[] { int.MaxValue }, ulong.MaxValue  ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(UpdateAsync_NameIsInUse_TestCaseData))]
        public async Task UpdateAsync_NameIsInUse_ReturnsNameInUseError(
            long roleId,
            string name,
            IReadOnlyList<int> grantedPermissionIds,
            ulong performedById)
        {
            using var testContext = new TestContext();
            
            testContext.SetIsNameInUse(true);
            testContext.SetCurrentIdentitiesCache();

            var uut = testContext.BuildUut();

            var updateModel = new RoleUpdateModel()
            {
                Name = name,
                GrantedPermissionIds = grantedPermissionIds
            };

            var result = await uut.UpdateAsync(
                roleId,
                updateModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockRolesRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    It.Is<Optional<IEnumerable<long>>>(y => y.IsSpecified && (y.Value != null) && y.Value.SetEquals(roleId.ToEnumerable())),
                    name,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockMessenger.Invocations.ShouldBeEmpty();

            testContext.MemoryCache.TryGetValue(RolesService._getCurrentIdentitiesCacheKey, out _)
                .ShouldBeTrue();

            testContext.MockTransactionScope.ShouldNotHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeOfType<NameInUseError>();
            result.Error.Message.ShouldContain(name);
        }

        public static readonly IReadOnlyList<TestCaseData> UpdateAsync_InvalidPermissionIds_TestCaseData
            = new[]
            {
                /*                  roleId,         name,           grantedPermissionIds,   performedById   */
                new TestCaseData(   long.MinValue,  string.Empty,   new[] { int.MinValue }, ulong.MinValue  ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             "Name 2",       new[] { 3, 4 },         5UL             ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   6L,             "Name 7",       new[] { 8, 9, 10 },     11UL            ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   12L,            "Name 13",      new[] { 14, 15, 16 },   17UL            ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  "MaxValue",     new[] { int.MaxValue }, ulong.MaxValue  ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(UpdateAsync_InvalidPermissionIds_TestCaseData))]
        public async Task UpdateAsync_GrantedPermissionIdsHasInvalidIds_ReturnsDataNotFoundError(
            long roleId,
            string name,
            IReadOnlyList<int> grantedPermissionIds,
            ulong performedById)
        {
            using var testContext = new TestContext();
            
            testContext.SetIsNameInUse(false);
            testContext.SetCurrentIdentitiesCache();

            var mockError = new Mock<OperationError>("Mock Message");
            testContext.SetValidatePermissionIdsResult(mockError.Object);

            var uut = testContext.BuildUut();

            var updateModel = new RoleUpdateModel()
            {
                Name = name,
                GrantedPermissionIds = grantedPermissionIds
            };

            var result = await uut.UpdateAsync(
                roleId,
                updateModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockRolesRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    It.Is<Optional<IEnumerable<long>>>(y => y.IsSpecified && (y.Value.Count() == 1) && (y.Value.First() == roleId)),
                    name,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockPermissionsService.ShouldHaveReceived(x => x
                .ValidateIdsAsync(
                    It.Is<IReadOnlyCollection<int>>(y => (y != null) && y.SetEquals(grantedPermissionIds)),
                    testContext.CancellationToken));

            testContext.MockMessenger.Invocations.ShouldBeEmpty();

            testContext.MemoryCache.TryGetValue(RolesService._getCurrentIdentitiesCacheKey, out _)
                .ShouldBeTrue();

            testContext.MockTransactionScope.ShouldNotHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);
        }

        public static readonly IReadOnlyList<TestCaseData> UpdateAsync_UpdateFailure_TestCaseData
            = new[]
            {
                /*                  roleId,         name,           grantedPermissionIds,   performed,                          performedById,  actionId        */
                new TestCaseData(   long.MinValue,  string.Empty,   new[] { int.MinValue }, DateTimeOffset.MinValue,            ulong.MinValue, long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             "Name 2",       Array.Empty<int>(),     DateTimeOffset.Parse("2019-01-01"), 3UL,            4L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   5L,             "Name 6",       new[] { 7 },            DateTimeOffset.Parse("2019-01-02"), 8UL,            9L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   10L,            "Name 11",      new[] { 12, 13, 14 },   DateTimeOffset.Parse("2019-01-03"), 15UL,           16L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  "MaxValue",     new[] { int.MaxValue }, DateTimeOffset.MaxValue,            ulong.MaxValue, long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(UpdateAsync_UpdateFailure_TestCaseData))]
        public async Task UpdateAsync_UpdateAsyncFails_ReturnsImmediately(
            long roleId,
            string name,
            IReadOnlyList<int> grantedPermissionIds,
            DateTimeOffset performed,
            ulong performedById,
            long actionId)
        {
            using var testContext = new TestContext()
            {
                UtcNow = performed,
                NextAdministrationActionId = actionId
            };

            testContext.SetIsNameInUse(false);
            testContext.SetValidatePermissionIdsResult();
            testContext.SetCurrentIdentitiesCache();

            var mockError = new Mock<OperationError>("Mock Message");
            testContext.SetRoleUpdateError(mockError.Object);

            var uut = testContext.BuildUut();

            var updateModel = new RoleUpdateModel()
            {
                Name = name,
                GrantedPermissionIds = grantedPermissionIds
            };

            var result = await uut.UpdateAsync(
                roleId,
                updateModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockRolesRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    It.Is<Optional<IEnumerable<long>>>(y => y.IsSpecified && (y.Value != null) && y.Value.SetEquals(roleId.ToEnumerable())),
                    name,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockPermissionsService.ShouldHaveReceived(x => x
                .ValidateIdsAsync(
                    It.Is<IReadOnlyCollection<int>>(y => (y != null) && y.SetEquals(grantedPermissionIds)),
                    testContext.CancellationToken));

            testContext.MockAuditableActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)RoleManagementAdministrationActionType.RoleModified,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockRolesRepository.ShouldHaveReceived(x => x
                .UpdateAsync(
                    roleId,
                    actionId,
                    name,
                    default,
                    testContext.CancellationToken));

            testContext.MockMessenger.Invocations.ShouldBeEmpty();

            testContext.MemoryCache.TryGetValue(RolesService._getCurrentIdentitiesCacheKey, out _)
                .ShouldBeTrue();

            testContext.MockTransactionScope.ShouldNotHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(mockError.Object);
        }

        public static readonly IReadOnlyList<TestCaseData> UpdateAsync_NoChanges_TestCaseData
            = new[]
            {
                /*                  roleId,         name,           performed,                          performedById,  actionId,       permissionMappings                      */
                new TestCaseData(   long.MinValue,  string.Empty,   DateTimeOffset.MinValue,            ulong.MinValue, long.MinValue,  new[] { (long.MinValue, int.MinValue) } ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             "Name 2",       DateTimeOffset.Parse("2019-01-01"), 3UL,            4L,             Array.Empty<ValueTuple<long, int>>()    ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   5L,             "Name 6",       DateTimeOffset.Parse("2019-01-02"), 7UL,            8L,             new[] { (9L, 10) }                      ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   11L,            "Name 12",      DateTimeOffset.Parse("2019-01-03"), 13UL,           14L,            new[] { (15L, 16), (17L, 18) }          ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  "MaxValue",     DateTimeOffset.MaxValue,            ulong.MaxValue, long.MaxValue,  new[] { (long.MaxValue, int.MaxValue) } ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(UpdateAsync_NoChanges_TestCaseData))]
        public async Task UpdateAsync_UpdateModelHasNoChanges_ReturnsNoChangesGivenError(
            long roleId,
            string name,
            DateTimeOffset performed,
            ulong performedById,
            long actionId,
            IReadOnlyList<(long mappingId, int permissionId)> permissionMappings)
        {
            using var testContext = new TestContext()
            {
                UtcNow = performed,
                NextAdministrationActionId = actionId
            };

            testContext.SetIsNameInUse(false);
            testContext.SetValidatePermissionIdsResult();
            testContext.SetRoleUpdateError(new NoChangesGivenError(string.Empty));
            testContext.SetPermissionMappingIdentities(roleId, permissionMappings);
            testContext.SetCurrentIdentitiesCache();

            var uut = testContext.BuildUut();

            var updateModel = new RoleUpdateModel()
            {
                Name = name,
                GrantedPermissionIds = permissionMappings.Select(x => x.permissionId).ToArray()
            };

            var result = await uut.UpdateAsync(
                roleId,
                updateModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockRolesRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    It.Is<Optional<IEnumerable<long>>>(y => y.IsSpecified && (y.Value != null) && y.Value.SetEquals(roleId.ToEnumerable())),
                    name,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockPermissionsService.ShouldHaveReceived(x => x
                .ValidateIdsAsync(
                    It.Is<IReadOnlyCollection<int>>(y => (y != null) && y.SetEquals(updateModel.GrantedPermissionIds)),
                    testContext.CancellationToken));

            testContext.MockAuditableActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)RoleManagementAdministrationActionType.RoleModified,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockRolesRepository.ShouldHaveReceived(x => x
                .UpdateAsync(
                    roleId,
                    actionId,
                    name,
                    default,
                    testContext.CancellationToken));

            testContext.MockRolesRepository.ShouldHaveReceived(x => x
                .AsyncEnumeratePermissionMappingIdentities(
                    roleId,
                    false));

            testContext.MockMessenger.Invocations.ShouldBeEmpty();

            testContext.MemoryCache.TryGetValue(RolesService._getCurrentIdentitiesCacheKey, out _)
                .ShouldBeTrue();

            testContext.MockTransactionScope.ShouldNotHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeOfType<NoChangesGivenError>();
            result.Error.Message.ShouldContain(roleId.ToString());
        }

        public static readonly IReadOnlyList<TestCaseData> UpdateAsync_NameHasChanged_TestCaseData
            = new[]
            {
                /*                  roleId,         name,           performed,                          performedById,  actionId,       versionId,      permissionMappings                      */
                new TestCaseData(   long.MinValue,  string.Empty,   DateTimeOffset.MinValue,            ulong.MinValue, long.MinValue,  long.MinValue,  new[] { (long.MinValue, int.MinValue) } ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             "Name 2",       DateTimeOffset.Parse("2019-01-01"), 3UL,            4L,             5L,             Array.Empty<ValueTuple<long, int>>()    ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   6L,             "Name 7",       DateTimeOffset.Parse("2019-01-02"), 8UL,            9L,             10L,            new[] { (11L, 12) }                     ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   13L,            "Name 14",      DateTimeOffset.Parse("2019-01-03"), 15UL,           16L,            17L,            new[] { (18L, 19), (20L, 21) }          ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  "MaxValue",     DateTimeOffset.MaxValue,            ulong.MaxValue, long.MaxValue,  long.MaxValue,  new[] { (long.MaxValue, int.MaxValue) } ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(UpdateAsync_NameHasChanged_TestCaseData))]
        public async Task UpdateAsync_NameHasChanged_PerformsUpdateAndWipesCacheAndReturnsSuccess(
            long roleId,
            string name,
            DateTimeOffset performed,
            ulong performedById,
            long actionId,
            long versionId,
            IReadOnlyList<(long mappingId, int permissionId)> permissionMappings)
        {
            using var testContext = new TestContext()
            {
                UtcNow = performed,
                NextAdministrationActionId = actionId
            };

            testContext.SetIsNameInUse(false);
            testContext.SetValidatePermissionIdsResult();
            testContext.SetRoleUpdateVersionId(versionId);
            testContext.SetPermissionMappingIdentities(roleId, permissionMappings);
            testContext.SetCurrentIdentitiesCache();

            var uut = testContext.BuildUut();

            var updateModel = new RoleUpdateModel()
            {
                Name = name,
                GrantedPermissionIds = permissionMappings.Select(x => x.permissionId).ToArray()
            };

            var result = await uut.UpdateAsync(
                roleId,
                updateModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockRolesRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    It.Is<Optional<IEnumerable<long>>>(y => y.IsSpecified && (y.Value != null) && y.Value.SetEquals(roleId.ToEnumerable())),
                    name,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockPermissionsService.ShouldHaveReceived(x => x
                .ValidateIdsAsync(
                    It.Is<IReadOnlyCollection<int>>(y => (y != null) && y.SetEquals(updateModel.GrantedPermissionIds)),
                    testContext.CancellationToken));

            testContext.MockAuditableActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)RoleManagementAdministrationActionType.RoleModified,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockRolesRepository.ShouldHaveReceived(x => x
                .UpdateAsync(
                    roleId,
                    actionId,
                    name,
                    default,
                    testContext.CancellationToken));

            testContext.MockRolesRepository.ShouldHaveReceived(x => x
                .AsyncEnumeratePermissionMappingIdentities(
                    roleId,
                    false));

            testContext.MockRolesRepository.ShouldNotHaveReceived(x => x
                .CreatePermissionMappingsAsync(
                    It.IsAny<long>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()));

            testContext.MockRolesRepository.ShouldNotHaveReceived(x => x
                .UpdatePermissionMappingsAsync(
                    It.IsAny<IEnumerable<long>>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()));

            testContext.MockMessenger.ShouldHaveReceived(x => x
                .PublishNotificationAsync(
                    It.Is<RoleUpdatingNotification>(y => (y != null) && (y.RoleId == roleId) && (y.ActionId == actionId)),
                    testContext.CancellationToken));

            testContext.MemoryCache.TryGetValue(RolesService._getCurrentIdentitiesCacheKey, out _)
                .ShouldBeFalse();

            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            result.IsSuccess.ShouldBeTrue();
        }

        public static readonly IReadOnlyList<TestCaseData> UpdateAsync_PermissionMappingsHaveChanged_TestCaseData
            = new[]
            {
                /*                  roleId,         name,           grantedPErmissionIds,   performed,                          performedById,  actionId,       permissionMappings,                         addedPermissionIds,     deletedMappingIds       */
                new TestCaseData(   long.MinValue,  string.Empty,   new[] { int.MinValue }, DateTimeOffset.MinValue,            ulong.MinValue, long.MinValue,  Array.Empty<ValueTuple<long, int>>(),       new[] { int.MinValue }, Array.Empty<long>()     ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             "Name 2",       new[] { 3 },            DateTimeOffset.Parse("2019-01-01"), 4UL,            5L,             new[] { (6L, 7) },                          new[] { 3 },            new[] { 6L }            ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   8L,             "Name 9",       new[] { 10, 11 },       DateTimeOffset.Parse("2019-01-02"), 12UL,           13L,            new[] { (14L, 10) },                        new[] { 11 },           Array.Empty<long>()     ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   15L,            "Name 16",      new[] { 17, 18, 19 },   DateTimeOffset.Parse("2019-01-03"), 20UL,           21L,            new[] { (22L, 17), (23L, 24) },             new[] { 18, 19 },       new[] { 23L }           ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   25L,            "Name 26",      new[] { 27, 28 },       DateTimeOffset.Parse("2019-01-04"), 30UL,           31L,            new[] { (32L, 28), (33L, 34), (35L, 36) },  new[] { 27 },           new[] { 33L, 35L }      ).SetName("{m}(Unique Value Set 4)"),
                new TestCaseData(   37L,            "Name 38",      new[] { 39, 40 },       DateTimeOffset.Parse("2019-01-05"), 41UL,           42L,            new[] { (43L, 39), (44L, 40), (45L, 46) },  Array.Empty<int>(),     new[] { 45L }           ).SetName("{m}(Unique Value Set 5)"),
                new TestCaseData(   long.MaxValue,  "MaxValue",     Array.Empty<int>(),     DateTimeOffset.MaxValue,            ulong.MaxValue, long.MaxValue,  new[] { (long.MaxValue, int.MaxValue) },    Array.Empty<int>(),     new[] { long.MaxValue } ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(UpdateAsync_PermissionMappingsHaveChanged_TestCaseData))]
        public async Task UpdateAsync_PermissionMappingsHaveChanged_PerformsMappingsAndWipesCacheAndReturnsSuccess(
            long roleId,
            string name,
            IReadOnlyList<int> grantedPermissionIds,
            DateTimeOffset performed,
            ulong performedById,
            long actionId,
            IReadOnlyList<(long mappingId, int permissionId)> permissionMappings,
            IReadOnlyList<int> addedPermissionIds,
            IReadOnlyList<long> deletedMappingIds)
        {
            using var testContext = new TestContext()
            {
                UtcNow = performed,
                NextAdministrationActionId = actionId
            };

            testContext.SetIsNameInUse(false);
            testContext.SetValidatePermissionIdsResult();
            testContext.SetRoleUpdateError(new NoChangesGivenError(string.Empty));
            testContext.SetPermissionMappingIdentities(roleId, permissionMappings);
            testContext.SetCurrentIdentitiesCache();

            var uut = testContext.BuildUut();

            var updateModel = new RoleUpdateModel()
            {
                Name = name,
                GrantedPermissionIds = grantedPermissionIds
            };

            var result = await uut.UpdateAsync(
                roleId,
                updateModel,
                performedById,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockRolesRepository.ShouldHaveReceived(x => x
                .AnyVersionsAsync(
                    It.Is<Optional<IEnumerable<long>>>(y => y.IsSpecified && (y.Value != null) && y.Value.SetEquals(roleId.ToEnumerable())),
                    name,
                    false,
                    true,
                    testContext.CancellationToken));

            testContext.MockPermissionsService.ShouldHaveReceived(x => x
                .ValidateIdsAsync(
                    It.Is<IReadOnlyCollection<int>>(y => (y != null) && y.SetEquals(updateModel.GrantedPermissionIds)),
                    testContext.CancellationToken));

            testContext.MockAuditableActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)RoleManagementAdministrationActionType.RoleModified,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockRolesRepository.ShouldHaveReceived(x => x
                .UpdateAsync(
                    roleId,
                    actionId,
                    name,
                    default,
                    testContext.CancellationToken));

            testContext.MockRolesRepository.ShouldHaveReceived(x => x
                .AsyncEnumeratePermissionMappingIdentities(
                    roleId,
                    false));

            if (addedPermissionIds.Any())
                testContext.MockRolesRepository.ShouldHaveReceived(x => x
                    .CreatePermissionMappingsAsync(
                        roleId,
                        It.Is<IEnumerable<int>>(y => (y != null) && y.SetEquals(addedPermissionIds)),
                        actionId,
                        testContext.CancellationToken));
            else
                testContext.MockRolesRepository.ShouldNotHaveReceived(x => x
                    .CreatePermissionMappingsAsync(
                        It.IsAny<long>(),
                        It.IsAny<IEnumerable<int>>(),
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()));

            if (deletedMappingIds.Any())
                testContext.MockRolesRepository.ShouldHaveReceived(x => x
                    .UpdatePermissionMappingsAsync(
                        It.Is<IEnumerable<long>>(y => (y != null) && y.SetEquals(deletedMappingIds)),
                        actionId,
                        testContext.CancellationToken));
            else
                testContext.MockRolesRepository.ShouldNotHaveReceived(x => x
                    .UpdatePermissionMappingsAsync(
                        It.IsAny<IEnumerable<long>>(),
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()));

            testContext.MockMessenger.ShouldHaveReceived(x => x
                .PublishNotificationAsync(
                    It.Is<RoleUpdatingNotification>(y => (y != null) && (y.RoleId == roleId) && (y.ActionId == actionId)),
                    testContext.CancellationToken));

            testContext.MemoryCache.TryGetValue(RolesService._getCurrentIdentitiesCacheKey, out _)
                .ShouldBeFalse();

            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            result.IsSuccess.ShouldBeTrue();
        }

        #endregion UpdateAsync() Tests

        #region ValidateIdsAsync() Tests

        [Test]
        public async Task ValidateIdsAsync_RoleIdsIsEmpty_ReturnsSuccess()
        {
            using var testContext = new TestContext();
            
            var uut = testContext.BuildUut();

            var result = await uut.ValidateIdsAsync(
                Array.Empty<long>(),
                testContext.CancellationToken);

            result.IsSuccess.ShouldBeTrue();
        }

        public static readonly IReadOnlyList<TestCaseData> ValidateIdsAsync_RoleIdsHasInvalidIds_TestCaseData
            = new[]
            {
                /*                  roleIds,                    validRoleIds,           invalidRoleIds          */
                new TestCaseData(   new[] { long.MinValue },    Array.Empty<long>(),    new[] { long.MinValue } ).SetName("{m}(Min Values)"),
                new TestCaseData(   new[] { 1L },               new[] { 2L },           new[] { 1L }            ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   new[] { 3L, 4L },           new[] { 4L, 5L },       new[] { 3L }            ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   new[] { 6L, 7L, 8L },       new[] { 9L, 10L, 11L }, new[] { 6L, 7L, 8L }    ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   new[] { long.MaxValue },    Array.Empty<long>(),    new[] { long.MaxValue } ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(ValidateIdsAsync_RoleIdsHasInvalidIds_TestCaseData))]
        public async Task ValidateIdsAsync_RoleIdsHasInvalidIds_ReturnsDataNotFound(
            IReadOnlyList<long> roleIds,
            IReadOnlyList<long> validRoleIds,
            IReadOnlyList<long> invalidRoleIds)
        {
            using var testContext = new TestContext();
            
            testContext.SetCurrentIdentitiesCache(validRoleIds);

            var uut = testContext.BuildUut();

            var result = await uut.ValidateIdsAsync(
                roleIds,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeOfType<DataNotFoundError>();
            foreach (var invalidRoleId in invalidRoleIds)
                result.Error.Message.ShouldContain(invalidRoleId.ToString());
        }

        public static readonly IReadOnlyList<TestCaseData> ValidateIdsAsync_RoleIds_TestCaseData
            = new[]
            {
                /*                  roleIds                 */
                new TestCaseData(   new[] { long.MinValue } ).SetName("{m}(Min Values)"),
                new TestCaseData(   new[] { 1L }            ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   new[] { 2L, 3L }        ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   new[] { 4L, 5L, 6L }    ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   new[] { long.MaxValue } ).SetName("{m}(Max Values)")
            };

        public static readonly IReadOnlyList<TestCaseData> ValidateIdsAsync_RoleIdsDoesNotHaveInvalidIds_TestCaseData
            = new[]
            {
                /*                  roleIds,                    validRoleIds            */
                new TestCaseData(   new[] { long.MinValue },    new[] { long.MinValue } ).SetName("{m}(Min Values)"),
                new TestCaseData(   new[] { 1L },               new[] { 1L, 2L }        ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   new[] { 3L, 4L },           new[] { 2L, 3L, 4L }    ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   new[] { 5L, 6L, 7L },       new[] { 5L, 6L, 7L }    ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   new[] { long.MaxValue },    new[] { long.MaxValue } ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(ValidateIdsAsync_RoleIdsDoesNotHaveInvalidIds_TestCaseData))]
        public async Task ValidateIdsAsync_RoleIdsDoesNotHaveInvalidIds_ReturnsSuccess(
            IReadOnlyList<long> roleIds,
            IReadOnlyList<long> validRoleIds)
        {
            using var testContext = new TestContext();
            
            testContext.SetCurrentIdentitiesCache(validRoleIds);

            var uut = testContext.BuildUut();

            var result = await uut.ValidateIdsAsync(
                roleIds,
                testContext.CancellationToken);

            result.IsSuccess.ShouldBeTrue();
        }

        [TestCaseSource(nameof(ValidateIdsAsync_RoleIds_TestCaseData))]
        public async Task ValidateIdsAsync_CurrentIdentitiesAreNotCached_ReadsIdentities(
            IReadOnlyList<long> roleIds)
        {
            using var testContext = new TestContext();

            var identities = roleIds
                .Select(x => new RoleIdentityViewModel(
                    id: x,
                    name: $"Role {x}"))
                .ToArray();

            testContext.MockRolesRepository
                .Setup(x => x.AsyncEnumerateIdentities(
                    It.IsAny<Optional<bool>>()))
                .Returns(identities.ToAsyncEnumerable());

            var uut = testContext.BuildUut();

            var result = await uut.ValidateIdsAsync(
                roleIds,
                testContext.CancellationToken);

            result.IsSuccess.ShouldBeTrue();

            testContext.MockRolesRepository.ShouldHaveReceived(x => x
                .AsyncEnumerateIdentities(
                    false));
        }

        [TestCaseSource(nameof(ValidateIdsAsync_RoleIds_TestCaseData))]
        public async Task ValidateIdsAsync_CurrentIdentitiesAreCached_DoesNotReadIdentities(
            IReadOnlyList<long> roleIds)
        {
            using var testContext = new TestContext();
            
            testContext.SetCurrentIdentitiesCache(roleIds);

            var uut = testContext.BuildUut();

            var result = await uut.ValidateIdsAsync(
                roleIds,
                testContext.CancellationToken);

            result.IsSuccess.ShouldBeTrue();

            testContext.MockRolesRepository.ShouldNotHaveReceived(x => x
                .AsyncEnumerateIdentities(
                    It.IsAny<Optional<bool>>()));
        }

        #endregion ValidateIdsAsync() Tests
    }
}
