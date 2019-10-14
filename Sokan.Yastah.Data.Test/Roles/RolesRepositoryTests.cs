using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using NUnit.Framework;
using Moq;
using Shouldly;

using Microsoft.EntityFrameworkCore;

using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data.Roles;


namespace Sokan.Yastah.Data.Test.Roles
{
    [TestFixture]
    public class RolesRepositoryTests
    {
        #region Test Context

        internal class TestContext
            : MockYastahDbTestContext
        {
            public TestContext(bool isReadOnly = true)
                : base(isReadOnly)
            {
                MockTransactionScopeFactory = new Mock<ITransactionScopeFactory>();

                MockTransactionScope = new Mock<ITransactionScope>();

                MockTransactionScopeFactory
                    .Setup(x => x.CreateScope(It.IsAny<IsolationLevel?>()))
                    .Returns(() => MockTransactionScope.Object);
            }

            public RolesRepository BuildUut()
                => new RolesRepository(
                    MockContext.Object,
                    MockTransactionScopeFactory.Object);

            public readonly Mock<ITransactionScopeFactory> MockTransactionScopeFactory;

            public readonly Mock<ITransactionScope> MockTransactionScope;
        }

        #endregion Test Context

        #region AnyVersionsAsync() Tests

        internal static IReadOnlyList<TestCaseData> AnyVersionsAsync_Always_TestCaseData
            => new[]
            {
                /*                  excludedRoleIds,                                                        name,                                   isDeleted,                          isLatestVersion                     expectedResult  */
                new TestCaseData(   Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.Unspecified,           Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}()"),
                new TestCaseData(   Optional<IEnumerable<long>>.FromValue(Array.Empty<long>()),             Optional<string>.Unspecified,           Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: Empty)"),
                new TestCaseData(   Optional<IEnumerable<long>>.FromValue(new[] {     1L, 2L, 3L     }),    Optional<string>.Unspecified,           Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         false           ).SetName("{m}(excludedRoleIds: 1, 2, 3)"),
                new TestCaseData(   Optional<IEnumerable<long>>.FromValue(new[] { 0L, 1L, 2L, 3L, 4L }),    Optional<string>.Unspecified,           Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         false           ).SetName("{m}(excludedRoleIds: 0, 1, 2, 3, 4)"),
                new TestCaseData(   Optional<IEnumerable<long>>.FromValue(new[] { 0L                 }),    Optional<string>.Unspecified,           Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: 0)"),
                new TestCaseData(   Optional<IEnumerable<long>>.FromValue(new[] {     1L             }),    Optional<string>.Unspecified,           Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: 1)"),
                new TestCaseData(   Optional<IEnumerable<long>>.FromValue(new[] {         2L         }),    Optional<string>.Unspecified,           Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: 2)"),
                new TestCaseData(   Optional<IEnumerable<long>>.FromValue(new[] {             3L     }),    Optional<string>.Unspecified,           Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: 3)"),
                new TestCaseData(   Optional<IEnumerable<long>>.FromValue(new[] {                 4L }),    Optional<string>.Unspecified,           Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: 4)"),
                new TestCaseData(   Optional<IEnumerable<long>>.FromValue(new[] {     1L,     3L,    }),    Optional<string>.Unspecified,           Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: 1, 3)"),
                new TestCaseData(   Optional<IEnumerable<long>>.FromValue(new[] { 0L,     2L,     4L }),    Optional<string>.Unspecified,           Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(excludedRoleIds: 0, 2, 4)"),
                new TestCaseData(   Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Role 1"),   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(name: Role 1)"),
                new TestCaseData(   Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Role 2"),   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(name: Role 2)"),
                new TestCaseData(   Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Role 2a"),  Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(name: Role 2a)"),
                new TestCaseData(   Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Role 2b"),  Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         false           ).SetName("{m}(name: Role 2b)"),
                new TestCaseData(   Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Role 3"),   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(name: Role 3)"),
                new TestCaseData(   Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Role 3a"),  Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         true            ).SetName("{m}(name: Role 3a)"),
                new TestCaseData(   Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Role 3b"),  Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         false           ).SetName("{m}(name: Role 3b)"),
                new TestCaseData(   Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Role 4"),   Optional<bool>.Unspecified,         Optional<bool>.Unspecified,         false           ).SetName("{m}(name: Role 4)"),
                new TestCaseData(   Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.Unspecified,           Optional<bool>.FromValue(true),     Optional<bool>.Unspecified,         true            ).SetName("{m}(isDeleted: true)"),
                new TestCaseData(   Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.Unspecified,           Optional<bool>.FromValue(false),    Optional<bool>.Unspecified,         true            ).SetName("{m}(isDeleted: false)"),
                new TestCaseData(   Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.Unspecified,           Optional<bool>.Unspecified,         Optional<bool>.FromValue(true),     true            ).SetName("{m}(isLatestVersion: true)"),
                new TestCaseData(   Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.Unspecified,           Optional<bool>.Unspecified,         Optional<bool>.FromValue(false),    true            ).SetName("{m}(isLatestVersion: false)"),
                new TestCaseData(   Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Role 2a"),  Optional<bool>.FromValue(true),     Optional<bool>.Unspecified,         true            ).SetName("{m}(Role has deleted version)"),
                new TestCaseData(   Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Role 1"),   Optional<bool>.FromValue(true),     Optional<bool>.Unspecified,         false           ).SetName("{m}(Role does not have deleted version)"),
                new TestCaseData(   Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Role 1"),   Optional<bool>.FromValue(false),    Optional<bool>.Unspecified,         true            ).SetName("{m}(Role has undeleted version)"),
                new TestCaseData(   Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Role 3a"),  Optional<bool>.FromValue(false),    Optional<bool>.Unspecified,         false           ).SetName("{m}(Role does not have undeleted version)"),
                new TestCaseData(   Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Role 1"),   Optional<bool>.Unspecified,         Optional<bool>.FromValue(true),     true            ).SetName("{m}(Role has current version)"),
                new TestCaseData(   Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Role 2"),   Optional<bool>.Unspecified,         Optional<bool>.FromValue(true),     false           ).SetName("{m}(Role does not have current version)"),
                new TestCaseData(   Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Role 2a"),  Optional<bool>.Unspecified,         Optional<bool>.FromValue(false),    true            ).SetName("{m}(Role has prior versions)"),
                new TestCaseData(   Optional<IEnumerable<long>>.Unspecified,                                Optional<string>.FromValue("Role 1"),   Optional<bool>.Unspecified,         Optional<bool>.FromValue(false),    false           ).SetName("{m}(Role does not have prior versions)"),
                new TestCaseData(   Optional<IEnumerable<long>>.FromValue(new[] { 0L }),                    Optional<string>.FromValue("name"),     Optional<bool>.FromValue(true),     Optional<bool>.FromValue(false),    false           ).SetName("{m}(All criteria specified)")
            };

        [TestCaseSource(nameof(AnyVersionsAsync_Always_TestCaseData))]
        public async Task AnyVersionsAsync_Always_ResultIsExpected(
            Optional<IEnumerable<long>> excludedRoleIds,
            Optional<string> name,
            Optional<bool> isDeleted,
            Optional<bool> isLatestVersion,
            bool expectedResult)
        {
            using (var testContext = new TestContext())
            {
                var uut = testContext.BuildUut();

                var result = await uut.AnyVersionsAsync(
                    testContext.CancellationToken,
                    excludedRoleIds,
                    name,
                    isDeleted,
                    isLatestVersion);

                result.ShouldBe(expectedResult);

                testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
            }
        }

        #endregion AnyVersionsAsync() Tests

        #region CreateAsync() Tests

        public static IReadOnlyList<TestCaseData> CreateAsync_TestCaseData
            => new[]
            {
                /*                  name            actionId,       roleId          */
                new TestCaseData(   string.Empty,   long.MinValue,  long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   "Test Role 1",  4L,             7L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   "Test Role 2",  5L,             8L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   "Test Role 3",  6L,             9L              ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   "Test Role 4",  long.MaxValue,  long.MaxValue   ).SetName("{m}(Max Values)"),
            };

        [TestCaseSource(nameof(CreateAsync_TestCaseData))]
        public async Task CreateAsync_Always_AddsNewEntities(
            string name,
            long actionId,
            long roleId)
        {
            using (var testContext = new TestContext(isReadOnly: false))
            {
                testContext.MockContext
                    .Setup(x => x.AddAsync(It.IsAny<RoleVersionEntity>(), It.IsAny<CancellationToken>()))
                    .Callback<RoleVersionEntity, CancellationToken>((x, y) =>
                    {
                        if(x?.Role != null)
                        {
                            x.Role.Id = roleId;
                            x.RoleId = roleId;
                        }
                    });

                var uut = testContext.BuildUut();

                var result = await uut.CreateAsync(
                    name,
                    actionId,
                    testContext.CancellationToken);

                testContext.MockContext
                    .ShouldHaveReceived(x => x.AddAsync(It.IsNotNull<RoleVersionEntity>(), testContext.CancellationToken));
                testContext.MockContext
                    .ShouldHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));

                var entity = testContext.MockContext
                    .Invocations
                    .Where(x => x.Method.Name == nameof(YastahDbContext.AddAsync))
                    .Select(x => (RoleVersionEntity)x.Arguments[0])
                    .First();

                entity.Name.ShouldBe(name);
                entity.ActionId.ShouldBe(actionId);
                entity.PreviousVersionId.ShouldBeNull();
                entity.NextVersionId.ShouldBeNull();
                entity.RoleId.ShouldBe(roleId);
                entity.Role.ShouldNotBeNull();
                entity.Role.Id.ShouldBe(roleId);

                result.ShouldBe(roleId);
            }
        }

        #endregion CreateAsync() Tests

        #region CreatePermissionMappingsAsync() Tests

        public static IReadOnlyList<TestCaseData> CreatePermissionMappingsAsync_TestCaseData
            => new[]
            {
                /*                  roleId,         permissionIds,          actionId,       mappingIds              */
                new TestCaseData(   long.MinValue,  new[] { int.MinValue }, long.MinValue,  new[] { long.MinValue } ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             Array.Empty<int>(),     11L,            Array.Empty<long>()     ).SetName("{m}(Empty Value Set)"),
                new TestCaseData(   2L,             new[] { 5 },            12L,            new[] { 15L }           ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   3L,             new[] { 6, 7 },         13L,            new[] { 16L, 17L }      ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   4L,             new[] { 8, 9, 10 },     14L,            new[] { 18L, 19L, 20L } ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  new[] { int.MaxValue }, long.MaxValue,  new[] { long.MaxValue } ).SetName("{m}(Max Values)"),
            };

        [TestCaseSource(nameof(CreatePermissionMappingsAsync_TestCaseData))]
        public async Task CreatePermissionMappingsAsync_Always_AddsNewEntities(
            long roleId,
            IEnumerable<int> permissionIds,
            long actionId,
            IReadOnlyList<long> mappingIds)
        {
            using (var testContext = new TestContext(isReadOnly: false))
            {
                testContext.MockContext
                    .Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<object>>(), It.IsAny<CancellationToken>()))
                    .Callback<IEnumerable<object>, CancellationToken>((x, y) =>
                    {
                        if (x?.Any() == true)
                            foreach ((var entity, var i) in x.Select((entity, i) => (entity, i)))
                                if (!(entity is null) && (entity is RolePermissionMappingEntity mapping) && (i < mappingIds.Count))
                                    mapping.Id = mappingIds[i];
                    });

                var uut = testContext.BuildUut();

                var result = await uut.CreatePermissionMappingsAsync(
                    roleId,
                    permissionIds,
                    actionId,
                    testContext.CancellationToken);

                testContext.MockContext
                    .ShouldHaveReceived(x => x.AddRangeAsync(It.IsNotNull<IEnumerable<object>>(), testContext.CancellationToken));
                testContext.MockContext
                    .ShouldHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));

                var entities = testContext.MockContext
                    .Invocations
                    .Where(x => x.Method.Name == nameof(YastahDbContext.AddRangeAsync))
                    .Select(x => (IEnumerable<object>)x.Arguments[0])
                    .First()
                    .ToArray();

                entities.Length.ShouldBe(mappingIds.Count);
                foreach (var entity in entities)
                {
                    entity.ShouldBeOfType<RolePermissionMappingEntity>();
                    var mapping = (RolePermissionMappingEntity)entity;

                    mapping.RoleId.ShouldBe(roleId);
                    mapping.CreationId.ShouldBe(actionId);
                    mapping.DeletionId.ShouldBeNull();
                }
                var mappings = entities.Cast<RolePermissionMappingEntity>();
                mappings.Select(x => x.PermissionId).ShouldBeSetEqualTo(permissionIds);

                result.ShouldBeSetEqualTo(mappingIds);
            }
        }

        #endregion CreatePermissionMappingsAsync() Tests

        #region ReadDetailAsync() Tests

        public static IReadOnlyList<TestCaseData> ReadDetailAsync_NoMatch_TestCaseData
            => new[]
            {
                /*                  roleId  isDeleted                       */
                new TestCaseData(   4L,     Optional<bool>.Unspecified      ).SetName("{m}(Role does not exist)"),
                new TestCaseData(   1L,     Optional<bool>.FromValue(true)  ).SetName("{m}(Role does not have deleted current version)"),
                new TestCaseData(   2L,     Optional<bool>.FromValue(false) ).SetName("{m}(Role does not have undeleted current version)")
            };

        [TestCaseSource(nameof(ReadDetailAsync_NoMatch_TestCaseData))]
        public async Task ReadDetailAsync_MatchingRoleDoesNotExist_ResultIsDataNotFound(
            long roleId,
            Optional<bool> isDeleted)
        {
            using (var testContext = new TestContext())
            {
                var uut = testContext.BuildUut();

                var result = await uut.ReadDetailAsync(
                    testContext.CancellationToken,
                    roleId,
                    isDeleted);

                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldBeOfType<DataNotFoundError>();
                result.Error.Message.ShouldContain(roleId.ToString());

                testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
            }
        }

        public static IReadOnlyList<TestCaseData> ReadDetailAsync_Match_TestCaseData
            => new[]
            {
                /*                  roleId  isDeleted                           versionId   permissionIds   */
                new TestCaseData(   3L,     Optional<bool>.Unspecified,         7L,         new[] { 1, 3 }  ).SetName("{m}(Role has prior versions)"),
                new TestCaseData(   1L,     Optional<bool>.Unspecified,         1L,         new[] { 1 }     ).SetName("{m}(Role has no prior versions)"),
                new TestCaseData(   2L,     Optional<bool>.FromValue(true),     5L,         new[] { 3 }     ).SetName("{m}(Role has deleted current version)"),
                new TestCaseData(   1L,     Optional<bool>.FromValue(false),    1L,         new[] { 1 }     ).SetName("{m}(Role has undeleted current version)")
            };

        [TestCaseSource(nameof(ReadDetailAsync_Match_TestCaseData))]
        public async Task ReadDetailAsync_MatchingRoleExists_ResultIsSuccess(
            long roleId,
            Optional<bool> isDeleted,
            long versionId,
            IEnumerable<int> permissionIds)
        {
            using (var testContext = new TestContext())
            {
                var uut = testContext.BuildUut();

                var result = await uut.ReadDetailAsync(
                    testContext.CancellationToken,
                    roleId,
                    isDeleted);

                result.IsSuccess.ShouldBeTrue();
                result.Value.ShouldNotBeNull();
                result.Value.Id.ShouldBe(roleId);

                var entity = testContext.Entities.RoleVersions.First(x => x.Id == versionId);

                result.Value.Name.ShouldBe(entity.Name);
                result.Value.GrantedPermissionIds.ShouldBeSetEqualTo(permissionIds);

                testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
            }
        }

        #endregion ReadDetailAsync() Tests

        #region ReadIdentitiesAsync() Tests

        public static IReadOnlyList<TestCaseData> ReadIdentitiesAsync_TestCaseData
            => new[]
            {
                /*                  isDeleted                           versionIds              */
                new TestCaseData(   Optional<bool>.Unspecified,         new[] { 1L, 5L, 7L }    ).SetName("{m}(All current versions)"),
                new TestCaseData(   Optional<bool>.FromValue(true),     new[] { 5L }            ).SetName("{m}(Deleted current versions)"),
                new TestCaseData(   Optional<bool>.FromValue(false),    new[] { 1L, 7L }        ).SetName("{m}(Undeleted current versions)")
            };

        [TestCaseSource(nameof(ReadIdentitiesAsync_TestCaseData))]
        public async Task ReadIdentitiesAsync_Always_ReturnsMatchingIdentities(
            Optional<bool> isDeleted,
            IReadOnlyList<long> versionIds)
        {
            using (var testContext = new TestContext())
            {
                var uut = testContext.BuildUut();

                var results = await uut.ReadIdentitiesAsync(
                    testContext.CancellationToken,
                    isDeleted);

                results.ShouldNotBeNull();
                results.ForEach(result => result.ShouldNotBeNull());
                results.Select(x => x?.Id).ShouldBeSetEqualTo(versionIds.Select(x => (long?)testContext.Entities.RoleVersions.First(y => y.Id == x).RoleId));
                foreach (var result in results)
                {
                    var entity = testContext.Entities.RoleVersions.First(y => (y.RoleId == result.Id) && versionIds.Contains(y.Id));

                    result.Name.ShouldBe(result.Name);
                }

                testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
            }
        }

        #endregion ReadIdentitiesAsync() Tests

        #region ReadPermissionMappingIdentitiesAsync() Tests

        public static IReadOnlyList<TestCaseData> ReadPermissionMappingIdentitiesAsync_TestCaseData
            => new[]
            {
                /*                  roleId  isDeleted                           mappingIds              */
                new TestCaseData(   1L,     Optional<bool>.FromValue(true),     Array.Empty<long>()     ).SetName("{m}(Role has no deleted mappings)"),
                new TestCaseData(   1L,     Optional<bool>.FromValue(false),    new[] { 1L }            ).SetName("{m}(Role has only undeleted mappings)"),
                new TestCaseData(   2L,     Optional<bool>.FromValue(true),     new[] { 2L }            ).SetName("{m}(Role has deleted mappings)"),
                new TestCaseData(   2L,     Optional<bool>.FromValue(false),    new[] { 3L }            ).SetName("{m}(Role has undeleted mappings)"),
                new TestCaseData(   3L,     Optional<bool>.Unspecified,         new[] { 4L, 5L, 6L }    ).SetName("{m}(Role has deleted and undeleted mappings)"),
                new TestCaseData(   4L,     Optional<bool>.Unspecified,         Array.Empty<long>()     ).SetName("{m}(Role does not exist)")
            };

        [TestCaseSource(nameof(ReadPermissionMappingIdentitiesAsync_TestCaseData))]
        public async Task ReadPermissionMappingIdentitiesAsync_Always_ReturnsMatchingIdentities(
            long roleId,
            Optional<bool> isDeleted,
            IReadOnlyList<long> mappingIds)
        {
            using (var testContext = new TestContext())
            {
                var uut = testContext.BuildUut();

                var results = await uut.ReadPermissionMappingIdentitiesAsync(
                    testContext.CancellationToken,
                    roleId,
                    isDeleted);

                results.ShouldNotBeNull();
                results.ForEach(result => result.ShouldNotBeNull());
                results.Select(x => x?.Id).ShouldBeSetEqualTo(mappingIds.Select(x => (long?)x));
                foreach (var result in results)
                {
                    result.RoleId.ShouldBe(roleId);
                    
                    var entity = testContext.Entities.RolePermissionMappings.First(x => x.Id == result.Id);

                    result.PermissionId.ShouldBe(entity.PermissionId);
                }

                testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
            }
        }

        #endregion ReadPermissionMappingIdentitiesAsync() Tests

        #region UpdateAsync() Tests

        public static IReadOnlyList<TestCaseData> UpdateAsync_DataNotFound_TestCaseData
            => new[]
            {
                /*                  roleId  actionId        name                                        isDeleted                       */
                new TestCaseData(   4L,     long.MinValue,  Optional<string>.FromValue(string.Empty),   Optional<bool>.FromValue(false) ).SetName("{m}(Min values)"),
                new TestCaseData(   4L,     1L,             Optional<string>.FromValue("Bogus Role 1"), Optional<bool>.FromValue(false) ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   4L,     2L,             Optional<string>.FromValue("Bogus Role 2"), Optional<bool>.FromValue(true)  ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   4L,     3L,             Optional<string>.FromValue("Bogus Role 3"), Optional<bool>.FromValue(false) ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   4L,     long.MaxValue,  Optional<string>.FromValue("Bogus Role 4"), Optional<bool>.FromValue(true)  ).SetName("{m}(Max values)"),
            };

        [TestCaseSource(nameof(UpdateAsync_DataNotFound_TestCaseData))]
        public async Task UpdateAsync_RoleIdDoesNotExist_ResultIsNoDataFound(
            long roleId,
            long actionId,
            Optional<string> name,
            Optional<bool> isDeleted)
        {
            using (var testContext = new TestContext(isReadOnly: false))
            {
                var uut = testContext.BuildUut();

                var result = await uut.UpdateAsync(
                    testContext.CancellationToken,
                    roleId,
                    actionId,
                    name,
                    isDeleted);

                testContext.MockTransactionScopeFactory
                    .ShouldHaveReceived(x => x.CreateScope(default));

                testContext.MockContext
                    .ShouldNotHaveReceived(x => x.AddAsync(It.IsAny<RoleVersionEntity>(), It.IsAny<CancellationToken>()));
                testContext.MockContext
                    .ShouldNotHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));

                testContext.MockTransactionScope
                    .ShouldNotHaveReceived(x => x.Complete());
                testContext.MockTransactionScope
                    .ShouldHaveReceived(x => x.Dispose());

                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldBeOfType<DataNotFoundError>();
                result.Error.Message.ShouldContain(roleId.ToString());
            }
        }

        public static IReadOnlyList<TestCaseData> UpdateAsync_NoChangesGiven_TestCaseData
            => new[]
            {
                /*                  roleId  actionId    name                                    isDeleted                       */
                new TestCaseData(   1L,     20L,        Optional<string>.Unspecified,           Optional<bool>.Unspecified      ).SetName("{m}(Role 1, no changes)"),
                new TestCaseData(   1L,     21L,        Optional<string>.FromValue("Role 1"),   Optional<bool>.FromValue(false) ).SetName("{m}(Role 1, no differences)"),
                new TestCaseData(   2L,     22L,        Optional<string>.Unspecified,           Optional<bool>.Unspecified      ).SetName("{m}(Role 2, no changes)"),
                new TestCaseData(   2L,     23L,        Optional<string>.FromValue("Role 2a"),  Optional<bool>.FromValue(true)  ).SetName("{m}(Role 2, no differences)"),
                new TestCaseData(   3L,     24L,        Optional<string>.Unspecified,           Optional<bool>.Unspecified      ).SetName("{m}(Role 3, no changes)"),
                new TestCaseData(   3L,     25L,        Optional<string>.FromValue("Role 3"),   Optional<bool>.FromValue(false) ).SetName("{m}(Role 3, no differences)")
            };

        [TestCaseSource(nameof(UpdateAsync_NoChangesGiven_TestCaseData))]
        public async Task UpdateAsync_NoChangesAreNeeded_ResultIsNoChangesGiven(
            long roleId,
            long actionId,
            Optional<string> name,
            Optional<bool> isDeleted)
        {
            using (var testContext = new TestContext(isReadOnly: false))
            {
                var uut = testContext.BuildUut();

                var result = await uut.UpdateAsync(
                    testContext.CancellationToken,
                    roleId,
                    actionId,
                    name,
                    isDeleted);

                testContext.MockTransactionScopeFactory
                    .ShouldHaveReceived(x => x.CreateScope(default));

                testContext.MockContext
                    .ShouldNotHaveReceived(x => x.AddAsync(It.IsAny<RoleVersionEntity>(), It.IsAny<CancellationToken>()));
                testContext.MockContext
                    .ShouldNotHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));

                testContext.MockTransactionScope
                    .ShouldHaveReceived(x => x.Complete());
                testContext.MockTransactionScope
                    .ShouldHaveReceived(x => x.Dispose());

                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldBeOfType<NoChangesGivenError>();
                result.Error.Message.ShouldContain(roleId.ToString());
            }
        }

        public static IReadOnlyList<TestCaseData> UpdateAsync_ChangesGiven_TestCaseData
            => new[]
            {
                /*                  roleId  actionId    name                                    isDeleted                           versionId   */
                new TestCaseData(   1L,     26L,        Optional<string>.FromValue("Role 1a"),  Optional<bool>.Unspecified,         8L          ).SetName("{m}(Role 1, Name changed)"),
                new TestCaseData(   1L,     27L,        Optional<string>.Unspecified,           Optional<bool>.FromValue(true),     9L          ).SetName("{m}(Role 1, IsDeleted changed)"),
                new TestCaseData(   1L,     28L,        Optional<string>.FromValue("Role 1b"),  Optional<bool>.FromValue(true),     10L         ).SetName("{m}(Role 1, all properties changed)"),
                new TestCaseData(   2L,     29L,        Optional<string>.FromValue("Role 2"),   Optional<bool>.Unspecified,         11L         ).SetName("{m}(Role 2, Name changed)"),
                new TestCaseData(   2L,     30L,        Optional<string>.Unspecified,           Optional<bool>.FromValue(false),    12L         ).SetName("{m}(Role 2, IsDeleted changed)"),
                new TestCaseData(   2L,     31L,        Optional<string>.FromValue("Role 2b"),  Optional<bool>.FromValue(false),    13L         ).SetName("{m}(Role 2, all properties changed)"),
                new TestCaseData(   3L,     32L,        Optional<string>.FromValue("Role 3b"),  Optional<bool>.Unspecified,         14L         ).SetName("{m}(Role 3, Name changed)"),
                new TestCaseData(   3L,     33L,        Optional<string>.Unspecified,           Optional<bool>.FromValue(true),     15L         ).SetName("{m}(Role 3, IsDeleted changed)"),
                new TestCaseData(   3L,     34L,        Optional<string>.FromValue("Role 3c"),  Optional<bool>.FromValue(true),     16L         ).SetName("{m}(Role 3, all properties changed)"),
            };

        [TestCaseSource(nameof(UpdateAsync_ChangesGiven_TestCaseData))]
        public async Task UpdateAsync_Otherwise_ResultIsSuccess(
            long roleId,
            long actionId,
            Optional<string> name,
            Optional<bool> isDeleted,
            long versionId)
        {
            using (var testContext = new TestContext(isReadOnly: false))
            {
                testContext.MockContext
                    .Setup(x => x.AddAsync(It.IsNotNull<RoleVersionEntity>(), It.IsAny<CancellationToken>()))
                    .Callback<RoleVersionEntity, CancellationToken>((x, y) => x.Id = versionId);

                var previousVersionEntity = testContext.Entities.RoleVersions.First(x => (x.RoleId == roleId) && (x.NextVersionId is null));

                var uut = testContext.BuildUut();

                var result = await uut.UpdateAsync(
                    testContext.CancellationToken,
                    roleId,
                    actionId,
                    name,
                    isDeleted);

                testContext.MockTransactionScopeFactory
                    .ShouldHaveReceived(x => x.CreateScope(default));

                testContext.MockContext
                    .ShouldHaveReceived(x => x.AddAsync(It.IsAny<RoleVersionEntity>(), testContext.CancellationToken));
                testContext.MockContext
                    .ShouldHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));

                testContext.MockTransactionScope
                    .ShouldHaveReceived(x => x.Complete());
                testContext.MockTransactionScope
                    .ShouldHaveReceived(x => x.Dispose());

                var entity = testContext.MockContext
                    .Invocations
                    .Where(x => x.Method.Name == nameof(YastahDbContext.AddAsync))
                    .Select(x => (RoleVersionEntity)x.Arguments[0])
                    .First();

                entity.ShouldNotBeNull();
                entity.RoleId.ShouldBe(roleId);
                entity.ActionId.ShouldBe(actionId);
                entity.NextVersionId.ShouldBeNull();
                entity.PreviousVersionId.ShouldBe(previousVersionEntity.Id);
                entity.Name.ShouldBe(name.IsSpecified ? name.Value : previousVersionEntity.Name);
                entity.IsDeleted.ShouldBe(isDeleted.IsSpecified ? isDeleted.Value : previousVersionEntity.IsDeleted);

                previousVersionEntity.NextVersion.ShouldBeSameAs(entity);

                result.IsSuccess.ShouldBeTrue();
                result.Value.ShouldBe(versionId);
            }
        }

        #endregion UpdateAsync() Tests

        #region UpdatePermissionMappingsAsync() Tests

        internal static readonly IReadOnlyList<TestCaseData> UpdatePermissionMappingsAsync_TestCaseData
            = new[]
            {
                /*                  mappingIds,                         deletionId      */
                new TestCaseData(   new[] { 1L },                       long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   new[] { 2L },                       20L             ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   new[] { 3L, 4L },                   21L             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   new[] { 5L, 6L },                   22L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   new[] { 1L, 2L, 3L, 4L, 5L, 6L },   long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(UpdatePermissionMappingsAsync_TestCaseData))]
        public async Task UpdatePermissionMappingsAsync_Always_UpdatesMatchingEntities(
            IReadOnlyList<long> mappingIds,
            long deletionId)
        {
            using (var testContext = new TestContext(isReadOnly: false))
            {
                var entities = mappingIds
                    .Select(x => testContext.Entities.RolePermissionMappings.First(rpm => rpm.Id == x))
                    .ToArray();

                var uut = testContext.BuildUut();

                await uut.UpdatePermissionMappingsAsync(
                    mappingIds,
                    deletionId,
                    testContext.CancellationToken);

                entities.ForEach(entity => entity.DeletionId.ShouldBe(deletionId));

                testContext.MockContext.ShouldHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
            }
        }

        #endregion UpdatePermissionMappingsAsync() Tests
    }
}
