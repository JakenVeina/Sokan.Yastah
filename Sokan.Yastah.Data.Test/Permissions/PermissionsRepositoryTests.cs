using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;
using Moq;
using Shouldly;

using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data.Permissions;

namespace Sokan.Yastah.Data.Test.Permissions
{
    [TestFixture]
    public class PermissionsRepositoryTests
    {
        #region Test Context

        internal class TestContext
            : MockYastahDbTestContext
        {
            public static TestContext CreateReadOnly()
                => new TestContext(
                    PermissionsTestEntitySetBuilder.SharedSet);

            private TestContext(
                    YastahTestEntitySet entities)
                : base(
                    entities) { }

            public PermissionsRepository BuildUut()
                => new PermissionsRepository(
                    MockContext.Object,
                    LoggerFactory.CreateLogger<PermissionsRepository>());
        }

        #endregion Test Context

        #region AsyncEnumerateDescriptions() Tests

        [Test]
        public async Task AsyncEnumerateDescriptions_Always_ReturnsAllDescriptions()
        {
            using var testContext = TestContext.CreateReadOnly();
            
            var uut = testContext.BuildUut();

            var results = await uut.AsyncEnumerateDescriptions()
                .ToArrayAsync();

            var entities = testContext.Entities.PermissionCategories;

            results.ShouldNotBeNull();
            results.ForEach(result => result.ShouldNotBeNull());
            results.Select(x => x.Id).ShouldBeSetEqualTo(entities.Select(x => x.Id));
            results.ForEach(result =>
            {
                var entity = entities.First(x => x.Id == result.Id);

                result.Name.ShouldBe(entity.Name);
                result.Description.ShouldBe(entity.Description);

                result.Permissions.ShouldNotBeNull();
                result.Permissions.ForEach(permission => permission.ShouldNotBeNull());
                result.Permissions.Select(x => x.Id).ShouldBeSetEqualTo(
                    entity.Permissions.Select(x => x.PermissionId));

                result.Permissions.ForEach(permission =>
                {
                    var permissionEntity = entity.Permissions.First(x => x.PermissionId == permission.Id);

                    permission.Name.ShouldBe(permissionEntity.Name);
                    permission.Description.ShouldBe(permissionEntity.Description);
                });
            });

            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        #endregion AsyncEnumerateDescriptions() Tests

        #region AsyncEnumerateIdentities() Tests

        [Test]
        public async Task AsyncEnumerateIdentities_Always_ReturnsAllDescriptions()
        {
            using var testContext = TestContext.CreateReadOnly();

            var uut = testContext.BuildUut();

            var results = await uut.AsyncEnumerateIdentities()
                .ToArrayAsync();

            var entities = testContext.Entities.Permissions;

            results.ShouldNotBeNull();
            results.ForEach(result => result.ShouldNotBeNull());
            results.Select(x => x.Id).ShouldBeSetEqualTo(entities.Select(x => x.PermissionId));
            results.ForEach(result =>
            {
                var entity = entities.First(x => x.PermissionId == result.Id);

                result.Name.ShouldContain(entity.Name);
                result.Name.ShouldContain(entity.Category.Name);
            });

            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        #endregion AsyncEnumerateIdentities() Tests

        #region AsyncEnumeratePermissionIds() Tests

        internal static readonly IReadOnlyList<TestCaseData> AsyncEnumeratePermissionIds_TestCaseData
            = new[]
            {
                /*                  permissionIds                                                                               expectedResult                      */
                new TestCaseData(   Optional<IReadOnlyCollection<int>>.Unspecified,                                             new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 } ).SetName("{m}()"),
                new TestCaseData(   Optional<IReadOnlyCollection<int>>.FromValue(Array.Empty<int>()),                           Array.Empty<int>()                  ).SetName("{m}(permissionIds: Empty)"),
                new TestCaseData(   Optional<IReadOnlyCollection<int>>.FromValue(new[] {    1, 2, 3, 4, 5, 6, 7, 8, 9     }),   new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 } ).SetName("{m}(permissionIds: All)"),
                new TestCaseData(   Optional<IReadOnlyCollection<int>>.FromValue(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }),   new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 } ).SetName("{m}(permissionIds: All, plus extras)"),
                new TestCaseData(   Optional<IReadOnlyCollection<int>>.FromValue(new[] { 0                                }),   Array.Empty<int>()                  ).SetName("{m}(permissionIds: 0)"),
                new TestCaseData(   Optional<IReadOnlyCollection<int>>.FromValue(new[] {    1                             }),   new[] { 1                         } ).SetName("{m}(permissionIds: 1)"),
                new TestCaseData(   Optional<IReadOnlyCollection<int>>.FromValue(new[] {       2                          }),   new[] {    2                      } ).SetName("{m}(permissionIds: 2)"),
                new TestCaseData(   Optional<IReadOnlyCollection<int>>.FromValue(new[] {          3                       }),   new[] {       3                   } ).SetName("{m}(permissionIds: 3)"),
                new TestCaseData(   Optional<IReadOnlyCollection<int>>.FromValue(new[] {             4                    }),   new[] {          4                } ).SetName("{m}(permissionIds: 4)"),
                new TestCaseData(   Optional<IReadOnlyCollection<int>>.FromValue(new[] {                5                 }),   new[] {             5             } ).SetName("{m}(permissionIds: 5)"),
                new TestCaseData(   Optional<IReadOnlyCollection<int>>.FromValue(new[] {                   6              }),   new[] {                6          } ).SetName("{m}(permissionIds: 6)"),
                new TestCaseData(   Optional<IReadOnlyCollection<int>>.FromValue(new[] {                      7           }),   new[] {                   7       } ).SetName("{m}(permissionIds: 7)"),
                new TestCaseData(   Optional<IReadOnlyCollection<int>>.FromValue(new[] {                         8        }),   new[] {                      8    } ).SetName("{m}(permissionIds: 8)"),
                new TestCaseData(   Optional<IReadOnlyCollection<int>>.FromValue(new[] {                            9     }),   new[] {                         9 } ).SetName("{m}(permissionIds: 9)"),
                new TestCaseData(   Optional<IReadOnlyCollection<int>>.FromValue(new[] {                               10 }),   Array.Empty<int>()                  ).SetName("{m}(permissionIds: 10)")
            };

        [TestCaseSource(nameof(AsyncEnumeratePermissionIds_TestCaseData))]
        public async Task AsyncEnumeratePermissionIds_Always_ReturnsMatches(
            Optional<IReadOnlyCollection<int>> permissionIds,
            IReadOnlyList<int> expectedResult)
        {
            using var testContext = TestContext.CreateReadOnly();

            var uut = testContext.BuildUut();

            var result = await uut.AsyncEnumeratePermissionIds(
                    permissionIds)
                .ToArrayAsync();

            result.ShouldBeSetEqualTo(expectedResult);

            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        #endregion AsyncEnumeratePermissionIds() Tests
    }
}
