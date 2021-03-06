﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using NUnit.Framework;
using Moq;
using Shouldly;

using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data.Permissions;
using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Data.Test.Users
{
    [TestFixture]
    public class UsersRepositoryTests
    {
        #region Test Context

        internal class TestContext
            : MockYastahDbTestContext
        {
            public static TestContext CreateReadOnly()
                => new TestContext(
                    UsersTestEntitySetBuilder.SharedSet);

            private TestContext(
                    YastahTestEntitySet entities)
                : base(
                    entities)
            {
                MockTransactionScopeFactory = new Mock<ITransactionScopeFactory>();

                MockTransactionScope = new Mock<ITransactionScope>();

                MockTransactionScopeFactory
                    .Setup(x => x.CreateScope(It.IsAny<IsolationLevel?>()))
                    .Returns(() => MockTransactionScope.Object);
            }

            public UsersRepository BuildUut()
                => new UsersRepository(
                    MockContext.Object,
                    LoggerFactory.CreateLogger<UsersRepository>(),
                    MockTransactionScopeFactory.Object);

            public readonly Mock<ITransactionScopeFactory> MockTransactionScopeFactory;

            public readonly Mock<ITransactionScope> MockTransactionScope;
        }

        #endregion Test Context

        #region AnyAsync() Tests

        internal static IReadOnlyList<TestCaseData> AnyAsync_TestCaseData
            => new[]
            {
                /*                  userId,                         expectedResult  */
                new TestCaseData(   Optional<ulong>.Unspecified,    true            ).SetName("{m}()"),
                new TestCaseData(   Optional<ulong>.FromValue(0),   false           ).SetName("{m}(userId: 0)"),
                new TestCaseData(   Optional<ulong>.FromValue(1),   true            ).SetName("{m}(userId: 1)"),
                new TestCaseData(   Optional<ulong>.FromValue(2),   true            ).SetName("{m}(userId: 2)"),
                new TestCaseData(   Optional<ulong>.FromValue(3),   true            ).SetName("{m}(userId: 3)"),
                new TestCaseData(   Optional<ulong>.FromValue(4),   false           ).SetName("{m}(userId: 4)")
            };

        [TestCaseSource(nameof(AnyAsync_TestCaseData))]
        public async Task AnyAsync_Always_ResultIsExpected(
            Optional<ulong> userId,
            bool expectedResult)
        {
            using var testContext = TestContext.CreateReadOnly();
            
            var uut = testContext.BuildUut();

            var result = await uut.AnyAsync(
                userId,
                testContext.CancellationToken);

            result.ShouldBe(expectedResult);

            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        #endregion AnyAsync() Tests

        #region AsyncEnumerateDefaultRoleIds() Tests

        internal static IReadOnlyList<TestCaseData> AsyncEnumerateDefaultRoleIds_TestCaseData
            => new[]
            {
                /*                  expectedResult      */
                new TestCaseData(   new[] { 1L, 2L }    ).SetName("{m}()"),
            };

        [TestCaseSource(nameof(AsyncEnumerateDefaultRoleIds_TestCaseData))]
        public async Task AsyncEnumerateDefaultRoleIds_Always_ReturnsUndeletedMappingRoleIds(
            IReadOnlyList<long> expectedResult)
        {
            using var testContext = TestContext.CreateReadOnly();

            var uut = testContext.BuildUut();

            var result = await uut.AsyncEnumerateDefaultRoleIds()
                .ToArrayAsync();

            result.ShouldBeSetEqualTo(expectedResult);

            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        #endregion AsyncEnumerateDefaultRoleIds() Tests

        #region AsyncEnumerateDefaultPermissionIds() Tests

        internal static IReadOnlyList<TestCaseData> AsyncEnumerateDefaultPermissionIds_TestCaseData
            => new[]
            {
                /*                  expectedResult  */
                new TestCaseData(   new[] { 1, 2 }  ).SetName("{m}()"),
            };

        [TestCaseSource(nameof(AsyncEnumerateDefaultPermissionIds_TestCaseData))]
        public async Task AsyncEnumerateDefaultPermissionIds_Always_ReturnsUndeletedMappingPermissionIds(
            IReadOnlyList<int> expectedResult)
        {
            using var testContext = TestContext.CreateReadOnly();

            var uut = testContext.BuildUut();

            var result = await uut.AsyncEnumerateDefaultPermissionIds()
                .ToArrayAsync();

            result.ShouldBeSetEqualTo(expectedResult);

            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        #endregion AsyncEnumerateDefaultPermissionIds() Tests

        #region AsyncEnumerateGrantedPermissionIdentities() Tests

        internal static IReadOnlyList<TestCaseData> AsyncEnumerateGrantedPermissionIdentities_TestCaseData
            => new[]
            {
                /*                  userId  grantedPermissionIds    */
                new TestCaseData(   1UL,    new[] { 1, 2, 3 }       ).SetName("{m}(User 1)"),
                new TestCaseData(   2UL,    new[] { 3 }             ).SetName("{m}(User 2)"),
                new TestCaseData(   3UL,    Array.Empty<int>()      ).SetName("{m}(User 3)")
            };

        [TestCaseSource(nameof(AsyncEnumerateGrantedPermissionIdentities_TestCaseData))]
        public async Task AsyncEnumerateGrantedPermissionIdentities_Always_ReturnsExpected(
            ulong userId,
            IReadOnlyList<int> permissionIds)
        {
            using var testContext = TestContext.CreateReadOnly();

            var uut = testContext.BuildUut();

            var results = await uut.AsyncEnumerateGrantedPermissionIdentities(
                    userId)
                .ToArrayAsync();

            results.ShouldNotBeNull();
            results.ForEach(result => result.ShouldNotBeNull());
            results.Select(result => result.Id).ShouldBeSetEqualTo(permissionIds);
            results.ForEach(result =>
            {
                var entity = testContext.Entities.Permissions.First(p => p.PermissionId == result.Id);

                result.Name.ShouldContain(entity.Name);
                result.Name.ShouldContain(entity.Category.Name);
            });

            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        #endregion AsyncEnumerateGrantedPermissionIdentities() Tests

        #region AsyncEnumerateIds() Tests

        internal static readonly IReadOnlyList<TestCaseData> AsyncEnumerateIds_TestCaseData
            = new[]
            {
                /*                  roleId                          userIds                 */
                new TestCaseData(   Optional<long>.Unspecified,     new[] { 1UL, 2UL, 3UL } ).SetName("{m}()"),
                new TestCaseData(   Optional<long>.FromValue(0),    Array.Empty<ulong>()    ).SetName("{m}(userId: 0)"),
                new TestCaseData(   Optional<long>.FromValue(1),    new[] { 2UL }           ).SetName("{m}(userId: 1)"),
                new TestCaseData(   Optional<long>.FromValue(2),    new[] { 1UL }           ).SetName("{m}(userId: 2)"),
                new TestCaseData(   Optional<long>.FromValue(3),    new[] { 2UL }           ).SetName("{m}(userId: 3)"),
                new TestCaseData(   Optional<long>.FromValue(4),    Array.Empty<ulong>()    ).SetName("{m}(userId: 4)")
            };

        [TestCaseSource(nameof(AsyncEnumerateIds_TestCaseData))]
        public async Task AsyncEnumerateIds_Tests(
            Optional<long> roleId,
            IReadOnlyList<ulong> userIds)
        {
            using var testContext = TestContext.CreateReadOnly();

            var uut = testContext.BuildUut();

            var result = await uut.AsyncEnumerateIds(
                    roleId)
                .ToArrayAsync();

            result.ShouldBeSetEqualTo(userIds);

            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        #endregion AsyncEnumerateIds() Tests

        #region AsyncEnumerateOverviews() Tests

        [Test]
        public async Task AsyncEnumerateOverviews_Always_ReturnsAllUserOverviews()
        {
            using var testContext = TestContext.CreateReadOnly();

            var uut = testContext.BuildUut();

            var results = await uut.AsyncEnumerateOverviews()
                .ToArrayAsync();

            var userEntities = testContext.Entities.Users;

            results.ShouldNotBeNull();
            results.ForEach(result => result.ShouldNotBeNull());
            results.Select(result => result.Id).ShouldBeSetEqualTo(userEntities.Select(u => u.Id));
            results.ForEach(result =>
            {
                var entity = userEntities.First(u => u.Id == result.Id);

                result.Username.ShouldBe(entity.Username);
                result.Discriminator.ShouldBe(entity.Discriminator);
                result.FirstSeen.ShouldBe(entity.FirstSeen);
                result.LastSeen.ShouldBe(entity.LastSeen);
            });

            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        #endregion AsyncEnumerateOverviews() Tests

        #region AsyncEnumeratePermissionMappingIdentities() Tests

        internal static readonly IReadOnlyList<TestCaseData> AsyncEnumeratePermissionMappingIdentities_TestCaseData
            = new[]
            {
                /*                  userId, isDeleted,                          mappingIds              */
                new TestCaseData(   0UL,    Optional<bool>.Unspecified,         Array.Empty<long>()     ).SetName("{m}(userId: 0)"),
                new TestCaseData(   1UL,    Optional<bool>.Unspecified,         new[] { 1L, 7L, 8L }    ).SetName("{m}(userId: 1)"),
                new TestCaseData(   1UL,    Optional<bool>.FromValue(true),     new[] { 1L }            ).SetName("{m}(userId: 1, isDeleted: true)"),
                new TestCaseData(   1UL,    Optional<bool>.FromValue(false),    new[] { 7L, 8L }        ).SetName("{m}(userId: 1, isDeleted: false)"),
                new TestCaseData(   2UL,    Optional<bool>.Unspecified,         new[] { 4L, 5L, 6L }    ).SetName("{m}(userId: 2)"),
                new TestCaseData(   2UL,    Optional<bool>.FromValue(true),     new[] { 5L }            ).SetName("{m}(userId: 2, isDeleted: true)"),
                new TestCaseData(   2UL,    Optional<bool>.FromValue(false),    new[] { 4L, 6L }        ).SetName("{m}(userId: 2, isDeleted: false)"),
                new TestCaseData(   3UL,    Optional<bool>.Unspecified,         new[] { 2L, 3L }        ).SetName("{m}(userId: 3)"),
                new TestCaseData(   3UL,    Optional<bool>.FromValue(true),     Array.Empty<long>()     ).SetName("{m}(userId: 3, isDeleted: true)"),
                new TestCaseData(   3UL,    Optional<bool>.FromValue(false),    new[] { 2L, 3L }        ).SetName("{m}(userId: 3, isDeleted: false)")
           };

        [TestCaseSource(nameof(AsyncEnumeratePermissionMappingIdentities_TestCaseData))]
        public async Task AsyncEnumeratePermissionMappingIdentities_Always_ReturnsMatchingPermissionMappingIdentities(
            ulong userId,
            Optional<bool> isDeleted,
            IReadOnlyList<long> mappingIds)
        {
            using var testContext = TestContext.CreateReadOnly();

            var uut = testContext.BuildUut();

            var results = await uut.AsyncEnumeratePermissionMappingIdentities(
                    userId,
                    isDeleted)
                .ToArrayAsync();

            results.ShouldNotBeNull();
            results.ForEach(result => result.ShouldNotBeNull());
            results.Select(x => x.Id).ShouldBeSetEqualTo(mappingIds);
            results.ForEach(result =>
            {
                var entity = testContext.Entities.UserPermissionMappings.First(upm => upm.Id == result.Id);

                result.UserId.ShouldBe(entity.UserId);
                result.PermissionId.ShouldBe(entity.PermissionId);
                result.IsDenied.ShouldBe(entity.IsDenied);
            });

            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        #endregion AsyncEnumeratePermissionMappingIdentities() Tests

        #region AsyncEnumerateRoleMappingIdentities() Tests

        internal static readonly IReadOnlyList<TestCaseData> AsyncEnumerateRoleMappingIdentities_TestCaseData
            = new[]
            {
                /*                  userId, isDeleted,                          mappingIds              */
                new TestCaseData(   0UL,    Optional<bool>.Unspecified,         Array.Empty<long>()     ).SetName("{m}(userId: 0)"),
                new TestCaseData(   1UL,    Optional<bool>.Unspecified,         new[] { 5L, 6L }        ).SetName("{m}(userId: 1)"),
                new TestCaseData(   1UL,    Optional<bool>.FromValue(true),     new[] { 6L }            ).SetName("{m}(userId: 1, isDeleted: true)"),
                new TestCaseData(   1UL,    Optional<bool>.FromValue(false),    new[] { 5L }            ).SetName("{m}(userId: 1, isDeleted: false)"),
                new TestCaseData(   2UL,    Optional<bool>.Unspecified,         new[] { 3L, 4L }        ).SetName("{m}(userId: 2)"),
                new TestCaseData(   2UL,    Optional<bool>.FromValue(true),     Array.Empty<long>()     ).SetName("{m}(userId: 2, isDeleted: true)"),
                new TestCaseData(   2UL,    Optional<bool>.FromValue(false),    new[] { 3L, 4L }        ).SetName("{m}(userId: 2, isDeleted: false)"),
                new TestCaseData(   3UL,    Optional<bool>.Unspecified,         new[] { 1L, 2L }        ).SetName("{m}(userId: 3)"),
                new TestCaseData(   3UL,    Optional<bool>.FromValue(true),     new[] { 1L, 2L }        ).SetName("{m}(userId: 3, isDeleted: true)"),
                new TestCaseData(   3UL,    Optional<bool>.FromValue(false),    Array.Empty<long>()     ).SetName("{m}(userId: 3, isDeleted: false)")
           };

        [TestCaseSource(nameof(AsyncEnumerateRoleMappingIdentities_TestCaseData))]
        public async Task AsyncEnumerateRoleMappingIdentities_Always_ReturnsMatchingRoleMappingIdentities(
            ulong userId,
            Optional<bool> isDeleted,
            IReadOnlyList<long> mappingIds)
        {
            using var testContext = TestContext.CreateReadOnly();

            var uut = testContext.BuildUut();

            var results = await uut.AsyncEnumerateRoleMappingIdentities(
                    userId,
                    isDeleted)
                .ToArrayAsync();

            results.ShouldNotBeNull();
            results.ForEach(result => result.ShouldNotBeNull());
            results.Select(x => x.Id).ShouldBeSetEqualTo(mappingIds);
            results.ForEach(result =>
            {
                var entity = testContext.Entities.UserRoleMappings.First(upm => upm.Id == result.Id);

                result.UserId.ShouldBe(entity.UserId);
                result.RoleId.ShouldBe(entity.RoleId);
            });

            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        #endregion AsyncEnumerateRoleMappingIdentities() Tests

        #region CreatePermissionMappingsAsync() Tests

        public static IReadOnlyList<TestCaseData> CreatePermissionMappingsAsync_TestCaseData
            => new[]
            {
                /*                  userId,         permissionIds,          type,                           actionId,       mappingIds              */
                new TestCaseData(   ulong.MinValue, new[] { int.MinValue }, PermissionMappingType.Denied,   long.MinValue,  new[] { long.MinValue } ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            Array.Empty<int>(),     PermissionMappingType.Granted,  11L,            Array.Empty<long>()     ).SetName("{m}(Empty Value Set)"),
                new TestCaseData(   2UL,            new[] { 5 },            PermissionMappingType.Denied,   12L,            new[] { 15L }           ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   3UL,            new[] { 6, 7 },         PermissionMappingType.Granted,  13L,            new[] { 16L, 17L }      ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   4UL,            new[] { 8, 9, 10 },     PermissionMappingType.Denied,   14L,            new[] { 18L, 19L, 20L } ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, new[] { int.MaxValue }, PermissionMappingType.Granted,  long.MaxValue,  new[] { long.MaxValue } ).SetName("{m}(Max Values)"),
            };

        [TestCaseSource(nameof(CreatePermissionMappingsAsync_TestCaseData))]
        public async Task CreatePermissionMappingsAsync_Always_AddsNewEntities(
            ulong userId,
            IReadOnlyList<int> permissionIds,
            PermissionMappingType type,
            long actionId,
            IReadOnlyList<long> mappingIds)
        {
            using var testContext = TestContext.CreateReadOnly();

            testContext.MockContext
                .Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<object>>(), It.IsAny<CancellationToken>()))
                .Callback<IEnumerable<object>, CancellationToken>((x, y) =>
                {
                    if (x?.Any() == true)
                        foreach ((var entity, var i) in x.Select((entity, i) => (entity, i)))
                            if (!(entity is null) && (entity is UserPermissionMappingEntity mapping) && (i < mappingIds.Count))
                                mapping.Id = mappingIds[i];
                });

            var uut = testContext.BuildUut();

            var result = await uut.CreatePermissionMappingsAsync(
                userId,
                permissionIds,
                type,
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
                entity.ShouldBeOfType<UserPermissionMappingEntity>();
                var mapping = (UserPermissionMappingEntity)entity;

                mapping.UserId.ShouldBe(userId);
                mapping.IsDenied.ShouldBe(type == PermissionMappingType.Denied);
                mapping.CreationId.ShouldBe(actionId);
                mapping.DeletionId.ShouldBeNull();
            }
            var mappings = entities.Cast<UserPermissionMappingEntity>();
            mappings.Select(x => x.PermissionId).ShouldBeSetEqualTo(permissionIds);

            result.ShouldBeSetEqualTo(mappingIds);
        }

        #endregion CreatePermissionMappingsAsync() Tests

        #region CreateRoleMappingsAsync() Tests

        public static IReadOnlyList<TestCaseData> CreateRoleMappingsAsync_TestCaseData
            => new[]
            {
                /*                  userId,         roleIds,                    actionId,       mappingIds              */
                new TestCaseData(   ulong.MinValue, new[] { long.MinValue },    long.MinValue,  new[] { long.MinValue } ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            Array.Empty<long>(),        11L,            Array.Empty<long>()     ).SetName("{m}(Empty Value Set)"),
                new TestCaseData(   2UL,            new[] { 5L },               12L,            new[] { 15L }           ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   3UL,            new[] { 6L, 7L },           13L,            new[] { 16L, 17L }      ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   4UL,            new[] { 8L, 9L, 10L },      14L,            new[] { 18L, 19L, 20L } ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, new[] { long.MaxValue },    long.MaxValue,  new[] { long.MaxValue } ).SetName("{m}(Max Values)"),
            };

        [TestCaseSource(nameof(CreateRoleMappingsAsync_TestCaseData))]
        public async Task CreateRoleMappingsAsync_Always_AddsNewEntities(
            ulong userId,
            IReadOnlyList<long> roleIds,
            long actionId,
            IReadOnlyList<long> mappingIds)
        {
            using var testContext = TestContext.CreateReadOnly();

            testContext.MockContext
                .Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<object>>(), It.IsAny<CancellationToken>()))
                .Callback<IEnumerable<object>, CancellationToken>((x, y) =>
                {
                    if (x?.Any() == true)
                        foreach ((var entity, var i) in x.Select((entity, i) => (entity, i)))
                            if (!(entity is null) && (entity is UserRoleMappingEntity mapping) && (i < mappingIds.Count))
                                mapping.Id = mappingIds[i];
                });

            var uut = testContext.BuildUut();

            var result = await uut.CreateRoleMappingsAsync(
                userId,
                roleIds,
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
                entity.ShouldBeOfType<UserRoleMappingEntity>();
                var mapping = (UserRoleMappingEntity)entity;

                mapping.UserId.ShouldBe(userId);
                mapping.CreationId.ShouldBe(actionId);
                mapping.DeletionId.ShouldBeNull();
            }
            var mappings = entities.Cast<UserRoleMappingEntity>();
            mappings.Select(x => x.RoleId).ShouldBeSetEqualTo(roleIds);

            result.ShouldBeSetEqualTo(mappingIds);
        }

        #endregion CreateRoleMappingsAsync() Tests

        #region MergeAsync() Tests

        public static IReadOnlyList<TestCaseData> MergeAsync_UserDoesNotExist_TestCaseData
            => new[]
            {
                /*                  id,             username,           discriminator,  avatarHash,     firstSeen,                          lastSeen                            */
                new TestCaseData(   default(ulong), string.Empty,       string.Empty,   null,           default(DateTimeOffset),            default(DateTimeOffset)             ).SetName("{m}(Default Values)"),
                new TestCaseData(   ulong.MinValue, string.Empty,       string.Empty,   string.Empty,   DateTimeOffset.MinValue,            DateTimeOffset.MinValue             ).SetName("{m}(Min Values)"),
                new TestCaseData(   ulong.MaxValue, string.Empty,       string.Empty,   string.Empty,   DateTimeOffset.MaxValue,            DateTimeOffset.MaxValue             ).SetName("{m}(Max Values)"),
                new TestCaseData(   1UL,            "2",                "0003",         "00004",        DateTimeOffset.Parse("2005-06-07"), DateTimeOffset.Parse("2008-09-10")  ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   11UL,           "12",               "0013",         "00014",        DateTimeOffset.Parse("2015-04-17"), DateTimeOffset.Parse("2018-07-20")  ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   21UL,           "22",               "0023",         "00024",        DateTimeOffset.Parse("2025-02-27"), DateTimeOffset.Parse("2028-05-30")  ).SetName("{m}(Unique Value Set 3)")
            };

        [TestCaseSource(nameof(MergeAsync_UserDoesNotExist_TestCaseData))]
        public async Task MergeAsync_UserDoesNotExist_InsertsUserAndReturnsSingleInsert(
            ulong id,
            string username,
            string discriminator,
            string avatarHash,
            DateTimeOffset firstSeen,
            DateTimeOffset lastSeen)
        {
            using var testContext = TestContext.CreateReadOnly();

            testContext.MockContext
                .Setup(x => x.FindAsync<UserEntity?>(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as UserEntity);

            var uut = testContext.BuildUut();

            var result = await uut.MergeAsync(
                id,
                username,
                discriminator,
                avatarHash,
                firstSeen,
                lastSeen,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x.CreateScope(default));

            testContext.MockContext.ShouldHaveReceived(x => x.FindAsync<UserEntity>(
                It.Is<object[]>(y => (y.Length == 1) && (y[0] is ulong) && ((ulong)y[0] == id)),
                testContext.CancellationToken));
            testContext.MockContext.ShouldHaveReceived(x => x.AddAsync(It.IsNotNull<UserEntity>(), testContext.CancellationToken));
            testContext.MockContext.ShouldHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));

            testContext.MockTransactionScope.ShouldHaveReceived(x => x.Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x.Dispose());

            var entity = testContext.MockContext
                .Invocations
                .Where(x => x.Method.Name == nameof(YastahDbContext.AddAsync))
                .Select(x => (UserEntity)x.Arguments[0])
                .First();

            entity.Id.ShouldBe(id);
            entity.Username.ShouldBe(username);
            entity.Discriminator.ShouldBe(discriminator);
            entity.AvatarHash.ShouldBe(avatarHash);
            entity.FirstSeen.ShouldBe(firstSeen);
            entity.LastSeen.ShouldBe(lastSeen);

            result.RowsInserted.ShouldBe(1);
            result.RowsUpdated.ShouldBe(0);
        }

        public static IReadOnlyList<TestCaseData> MergeAsync_UserExists_TestCaseData
            => new[]
            {
                /*                  id,             username,           discriminator,  avatarHash,     firstSeen,                          lastSeen,                           priorFirstSeen                      */
                new TestCaseData(   default(ulong), string.Empty,       string.Empty,   null,           default(DateTimeOffset),            default(DateTimeOffset),            default(DateTimeOffset)             ).SetName("{m}(Default Values)"),
                new TestCaseData(   ulong.MinValue, string.Empty,       string.Empty,   string.Empty,   DateTimeOffset.MinValue,            DateTimeOffset.MinValue,            DateTimeOffset.MinValue             ).SetName("{m}(Min Values)"),
                new TestCaseData(   ulong.MaxValue, string.Empty,       string.Empty,   string.Empty,   DateTimeOffset.MaxValue,            DateTimeOffset.MaxValue,            DateTimeOffset.MaxValue             ).SetName("{m}(Max Values)"),
                new TestCaseData(   1UL,            "2",                "0003",         "00004",        DateTimeOffset.Parse("2005-06-07"), DateTimeOffset.Parse("2008-09-10"), DateTimeOffset.Parse("2011-12-13")  ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   14UL,           "15",               "0016",         "00017",        DateTimeOffset.Parse("2018-07-20"), DateTimeOffset.Parse("2021-10-23"), DateTimeOffset.Parse("2024-01-26")  ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   27UL,           "28",               "0029",         "00030",        DateTimeOffset.Parse("2031-08-03"), DateTimeOffset.Parse("2034-11-06"), DateTimeOffset.Parse("2037-02-09")  ).SetName("{m}(Unique Value Set 3)")
            };

        [TestCaseSource(nameof(MergeAsync_UserExists_TestCaseData))]
        public async Task MergeAsync_UserExists_UpdatesUserAndReturnsSingleUpdate(
            ulong id,
            string username,
            string discriminator,
            string? avatarHash,
            DateTimeOffset firstSeen,
            DateTimeOffset lastSeen,
            DateTimeOffset priorFirstSeen)
        {
            using var testContext = TestContext.CreateReadOnly();

            var entity = new UserEntity(
                id,
                username,
                discriminator,
                avatarHash,
                priorFirstSeen,
                lastSeen);

            testContext.MockContext
                .Setup(x => x.FindAsync<UserEntity?>(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            var uut = testContext.BuildUut();

            var result = await uut.MergeAsync(
                id,
                username,
                discriminator,
                avatarHash,
                firstSeen,
                lastSeen,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x.CreateScope(default));

            testContext.MockContext.ShouldHaveReceived(x => x.FindAsync<UserEntity>(
                It.Is<object[]>(y => (y.Length == 1) && (y[0] is ulong) && ((ulong)y[0] == id)),
                testContext.CancellationToken));
            testContext.MockContext.ShouldNotHaveReceived(x => x.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()));
            testContext.MockContext.ShouldHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));

            testContext.MockTransactionScope.ShouldHaveReceived(x => x.Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x.Dispose());

            entity.Id.ShouldBe(id);
            entity.Username.ShouldBe(username);
            entity.Discriminator.ShouldBe(discriminator);
            entity.AvatarHash.ShouldBe(avatarHash);
            entity.FirstSeen.ShouldBe(priorFirstSeen);
            entity.LastSeen.ShouldBe(lastSeen);

            result.RowsInserted.ShouldBe(0);
            result.RowsUpdated.ShouldBe(1);
        }

        #endregion MergeAsync() Tests

        #region ReadDetailAsync() Tests

        internal static IReadOnlyList<TestCaseData> ReadDetailAsync_UserDoesNotExist_TestCaseData
            => new[]
            {
                /*                  userId  */
                new TestCaseData(   ulong.MinValue  ).SetName("{m}(Min Values)"),
                new TestCaseData(   4UL             ).SetName("{m}(Unique Value Set)"),
                new TestCaseData(   ulong.MaxValue  ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(ReadDetailAsync_UserDoesNotExist_TestCaseData))]
        public async Task ReadDetailAsync_UserDoesNotExist_ResultIsDataNotFound(
            ulong userId)
        {
            using var testContext = TestContext.CreateReadOnly();

            var uut = testContext.BuildUut();

            var result = await uut.ReadDetailAsync(
                userId,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeOfType<DataNotFoundError>();
            result.Error.Message.ShouldContain(userId.ToString());

            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        internal static IReadOnlyList<TestCaseData> ReadDetailAsync_UserExists_TestCaseData
            => new[]
            {
                /*                  userId  grantedPermissionIds,   deniedPermissionIds,    assignedRoleIds         */
                new TestCaseData(   1UL,    new[] { 1, 2 },         Array.Empty<int>(),     new[] { 2L }            ).SetName("{m}(User 1)"),
                new TestCaseData(   2UL,    new[] { 3 },            new[] { 1 },            new[] { 1L, 3L }        ).SetName("{m}(User 2)"),
                new TestCaseData(   3UL,    Array.Empty<int>(),     new[] { 1, 2 },         Array.Empty<long>()     ).SetName("{m}(User 3)")
            };

        [TestCaseSource(nameof(ReadDetailAsync_UserExists_TestCaseData))]
        public async Task ReadDetailAsync_UserExists_ResultIsSuccess(
            ulong userId,
            IReadOnlyList<int> grantedPermissionIds,
            IReadOnlyList<int> deniedPermissionIds,
            IReadOnlyList<long> assignedRoleIds)
        {
            using var testContext = TestContext.CreateReadOnly();

            var entity = testContext.Entities.Users.First(x => x.Id == userId);

            var uut = testContext.BuildUut();

            var result = await uut.ReadDetailAsync(
                userId,
                testContext.CancellationToken);

            result.IsSuccess.ShouldBeTrue();
            result.Value.Id.ShouldBe(userId);
            result.Value.Username.ShouldBe(entity.Username);
            result.Value.Discriminator.ShouldBe(entity.Discriminator);
            result.Value.FirstSeen.ShouldBe(entity.FirstSeen);
            result.Value.LastSeen.ShouldBe(entity.LastSeen);
            result.Value.GrantedPermissionIds.ShouldBeSetEqualTo(grantedPermissionIds);
            result.Value.DeniedPermissionIds.ShouldBeSetEqualTo(deniedPermissionIds);
            result.Value.AssignedRoleIds.ShouldBeSetEqualTo(assignedRoleIds);

            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        #endregion ReadDetailAsync() Tests

        #region UpdatePermissionMappingsAsync() Tests

        internal static readonly IReadOnlyList<TestCaseData> UpdatePermissionMappingsAsync_TestCaseData
            = new[]
            {
                /*                  mappingIds,                 deletionId      */
                new TestCaseData(   new[] { default(long) },    default(long)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   new[] { long.MinValue },    long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   new[] { long.MaxValue },    long.MaxValue   ).SetName("{m}(Max Values)"),
                new TestCaseData(   new[] { 1L },               2L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   new[] { 3L, 4L },           5L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   new[] { 6L, 7L, 8L },       9L              ).SetName("{m}(Unique Value Set 3)"),
            };

        [TestCaseSource(nameof(UpdatePermissionMappingsAsync_TestCaseData))]
        public async Task UpdatePermissionMappingsAsync_Always_UpdatesMatchingEntities(
            IReadOnlyList<long> mappingIds,
            long deletionId)
        {
            using var testContext = TestContext.CreateReadOnly();

            var entities = new List<UserPermissionMappingEntity>();
            var foundIds = new List<long>();

            testContext.MockContext
                .Setup(x => x.FindAsync<UserPermissionMappingEntity?>(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns<object[], CancellationToken>((keys, _) =>
                {
                    if ((keys.Length == 1) && (keys[0] is long id))
                        foundIds.Add(id);

                    var entity = new UserPermissionMappingEntity(
                        default,
                        default,
                        default,
                        default,
                        default,
                        default);

                    entities.Add(entity);

                    return new ValueTask<UserPermissionMappingEntity?>(entity);
                });

            var uut = testContext.BuildUut();

            await uut.UpdatePermissionMappingsAsync(
                mappingIds,
                deletionId,
                testContext.CancellationToken);

            testContext.MockContext.ShouldHaveReceived(x => x
                    .FindAsync<UserPermissionMappingEntity?>(It.IsAny<object[]>(), testContext.CancellationToken),
                Times.Exactly(mappingIds.Count));

            foundIds.ShouldBeSetEqualTo(mappingIds);

            entities.ForEach(entity => entity.DeletionId.ShouldBe(deletionId));

            testContext.MockContext.ShouldHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));
        }

        #endregion UpdatePermissionMappingsAsync() Tests

        #region UpdateRoleMappingsAsync() Tests

        internal static readonly IReadOnlyList<TestCaseData> UpdateRoleMappingsAsync_TestCaseData
            = new[]
            {
                /*                  mappingIds,                 deletionId      */
                new TestCaseData(   new[] { default(long) },    default(long)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   new[] { long.MinValue },    long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   new[] { long.MaxValue },    long.MaxValue   ).SetName("{m}(Max Values)"),
                new TestCaseData(   new[] { 1L },               2L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   new[] { 3L, 4L },           5L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   new[] { 6L, 7L, 8L },       9L              ).SetName("{m}(Unique Value Set 3)"),
            };

        [TestCaseSource(nameof(UpdateRoleMappingsAsync_TestCaseData))]
        public async Task UpdateRoleMappingsAsync_Always_UpdatesMatchingEntities(
            IReadOnlyList<long> mappingIds,
            long deletionId)
        {
            using var testContext = TestContext.CreateReadOnly();

            var entities = new List<UserRoleMappingEntity>();
            var foundIds = new List<long>();

            testContext.MockContext
                .Setup(x => x.FindAsync<UserRoleMappingEntity?>(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns<object[], CancellationToken>((keys, _) =>
                {
                    if ((keys.Length == 1) && (keys[0] is long id))
                        foundIds.Add(id);

                    var entity = new UserRoleMappingEntity(
                        default,
                        default,
                        default,
                        default,
                        default);

                    entities.Add(entity);

                    return new ValueTask<UserRoleMappingEntity?>(entity);
                });

            var uut = testContext.BuildUut();

            await uut.UpdateRoleMappingsAsync(
                mappingIds,
                deletionId,
                testContext.CancellationToken);

            testContext.MockContext.ShouldHaveReceived(x => x
                    .FindAsync<UserRoleMappingEntity?>(It.IsAny<object[]>(), testContext.CancellationToken),
                Times.Exactly(mappingIds.Count));

            foundIds.ShouldBeSetEqualTo(mappingIds);

            entities.ForEach(entity => entity.DeletionId.ShouldBe(deletionId));

            testContext.MockContext.ShouldHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));
        }

        #endregion UpdateRoleMappingsAsync() Tests
    }
}
