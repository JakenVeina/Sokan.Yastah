using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;
using Moq;
using Shouldly;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using Sokan.Yastah.Business.Permissions;
using Sokan.Yastah.Data;
using Sokan.Yastah.Data.Permissions;

using Sokan.Yastah.Common.Test;

namespace Sokan.Yastah.Business.Test.Permissions
{
    [TestFixture]
    public class PermissionsServiceTests
    {
        #region Test Context

        public class TestContext
            : AsyncMethodTestContext
        {
            public TestContext()
            {
                MemoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

                MockPermissionsRepository = new Mock<IPermissionsRepository>();
            }

            public readonly MemoryCache MemoryCache;
            public readonly Mock<IPermissionsRepository> MockPermissionsRepository;

            public PermissionsService BuildUut()
                => new PermissionsService(
                    MemoryCache,
                    MockPermissionsRepository.Object);

            public void SetIdentitiesCache(IReadOnlyCollection<PermissionIdentityViewModel>? identities = null)
                => MemoryCache.Set(PermissionsService._getIdentitiesCacheKey, identities ?? Array.Empty<PermissionIdentityViewModel>());

            public void SetIdentitiesCache(IReadOnlyCollection<int> permissionIds)
                => SetIdentitiesCache(permissionIds
                    .Select(x => new PermissionIdentityViewModel(
                        id:     x,
                        name:   $"Permission {x}"))
                    .ToArray());
        }

        #endregion Test Context

        #region GetDescriptionsAsync() Tests

        [Test]
        public async Task GetDescriptionsAsync_DescriptionsAreNotCached_ReadsDescriptions()
        {
            using (var testContext = new TestContext())
            {
                var descriptions = new PermissionCategoryDescriptionViewModel[]
                { 
                    new PermissionCategoryDescriptionViewModel(
                        id:             default,
                        name:           string.Empty,
                        description:    string.Empty,
                        permissions:    Array.Empty<PermissionDescriptionViewModel>())
                };

                testContext.MockPermissionsRepository
                    .Setup(x => x.AsyncEnumerateDescriptions())
                    .Returns(descriptions.ToAsyncEnumerable());

                var uut = testContext.BuildUut();

                var result = await uut.GetDescriptionsAsync(
                    testContext.CancellationToken);

                testContext.MockPermissionsRepository.ShouldHaveReceived(x => x
                    .AsyncEnumerateDescriptions());

                testContext.MemoryCache.TryGetValue(PermissionsService._getDescriptionsCacheKey, out var cacheValue)
                    .ShouldBeTrue();
                cacheValue.ShouldBeAssignableTo<IReadOnlyCollection<PermissionCategoryDescriptionViewModel>>()
                    .ShouldBeSetEqualTo(descriptions);

                result.ShouldBeSameAs(cacheValue);
            }
        }

        [Test]
        public async Task GetDescriptionsAsync_DescriptionsAreCached_DoesNotReadDescriptions()
        {
            using (var testContext = new TestContext())
            {
                var descriptions = new PermissionCategoryDescriptionViewModel[] { };

                testContext.MemoryCache.Set(PermissionsService._getDescriptionsCacheKey, descriptions);

                var uut = testContext.BuildUut();

                var result = await uut.GetDescriptionsAsync(
                    testContext.CancellationToken);

                result.ShouldBeSameAs(descriptions);

                testContext.MockPermissionsRepository.ShouldNotHaveReceived(x => x
                    .AsyncEnumerateDescriptions());
            }
        }

        #endregion GetDescriptionsAsync() Tests

        #region GetIdentitiesAsync() Tests

        [Test]
        public async Task GetIdentitiesAsync_IdentitiesAreNotCached_ReadsIdentities()
        {
            using (var testContext = new TestContext())
            {
                var identities = new PermissionIdentityViewModel[]
                {
                    new PermissionIdentityViewModel(
                        id:     default,
                        name:   string.Empty)
                };

                testContext.MockPermissionsRepository
                    .Setup(x => x.AsyncEnumerateIdentities())
                    .Returns(identities.ToAsyncEnumerable());

                var uut = testContext.BuildUut();

                var result = await uut.GetIdentitiesAsync(
                    testContext.CancellationToken);

                testContext.MockPermissionsRepository.ShouldHaveReceived(x => x
                    .AsyncEnumerateIdentities());

                testContext.MemoryCache.TryGetValue(PermissionsService._getIdentitiesCacheKey, out var cacheValue)
                    .ShouldBeTrue();
                cacheValue.ShouldBeAssignableTo<IReadOnlyCollection<PermissionIdentityViewModel>>()
                    .ShouldBeSetEqualTo(identities);

                result.ShouldBeSameAs(cacheValue);
            }
        }

        [Test]
        public async Task GetIdentitiesAsync_IdentitiesAreCached_DoesNotReadIdentities()
        {
            using (var testContext = new TestContext())
            {
                var identities = new PermissionIdentityViewModel[] { };

                testContext.MemoryCache.Set(PermissionsService._getIdentitiesCacheKey, identities);

                var uut = testContext.BuildUut();

                var result = await uut.GetIdentitiesAsync(
                    testContext.CancellationToken);

                result.ShouldBeSameAs(identities);

                testContext.MockPermissionsRepository.ShouldNotHaveReceived(x => x
                    .AsyncEnumerateIdentities());
            }
        }

        #endregion GetIdentitiesAsync() Tests

        #region ValidateIdsAsync() Tests

        [Test]
        public async Task ValidateIdsAsync_PermissionIdsIsEmpty_ReturnsSuccess()
        {
            using (var testContext = new TestContext())
            {
                var uut = testContext.BuildUut();

                var result = await uut.ValidateIdsAsync(
                    Array.Empty<int>(),
                    testContext.CancellationToken);

                result.IsSuccess.ShouldBeTrue();
            }
        }

        public static readonly IReadOnlyList<TestCaseData> ValidateIdsAsync_PermissionIdsHasInvalidIds_TestCaseData
            = new[]
            {
                /*                  permissionIds,          validPermissionIds,     invalidPermissionIds    */
                new TestCaseData(   new[] { int.MinValue }, Array.Empty<int>(),     new[] { int.MinValue }  ).SetName("{m}(Min Values)"),
                new TestCaseData(   new[] { 1 },            new[] { 2 },            new[] { 1 }             ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   new[] { 3, 4 },         new[] { 4, 5 },         new[] { 3 }             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   new[] { 6, 7, 8 },      new[] { 9, 10, 11 },    new[] { 6, 7, 8 }       ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   new[] { int.MaxValue }, Array.Empty<int>(),     new[] { int.MaxValue }  ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(ValidateIdsAsync_PermissionIdsHasInvalidIds_TestCaseData))]
        public async Task ValidateIdsAsync_PermissionIdsHasInvalidIds_ReturnsDataNotFound(
            IReadOnlyList<int> permissionIds,
            IReadOnlyList<int> validPermissionIds,
            IReadOnlyList<int> invalidPermissionIds)
        {
            using (var testContext = new TestContext())
            {
                testContext.SetIdentitiesCache(validPermissionIds);

                var uut = testContext.BuildUut();

                var result = await uut.ValidateIdsAsync(
                    permissionIds,
                    testContext.CancellationToken);

                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldBeOfType<DataNotFoundError>();
                foreach (var invalidPermissionId in invalidPermissionIds)
                    result.Error.Message.ShouldContain(invalidPermissionId.ToString());
            }
        }

        public static readonly IReadOnlyList<TestCaseData> ValidateIdsAsync_PermissionIdsDoesNotHaveInvalidIds_TestCaseData
            = new[]
            {
                /*                  permissionIds,          validPermissionIds      */
                new TestCaseData(   new[] { int.MinValue }, new[] { int.MinValue }  ).SetName("{m}(Min Values)"),
                new TestCaseData(   new[] { 1 },            new[] { 1, 2 }          ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   new[] { 3, 4 },         new[] { 2, 3, 4 }       ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   new[] { 5, 6, 7 },      new[] { 5, 6, 7 }       ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   new[] { int.MaxValue }, new[] { int.MaxValue }  ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(ValidateIdsAsync_PermissionIdsDoesNotHaveInvalidIds_TestCaseData))]
        public async Task ValidateIdsAsync_PermissionIdsDoesNotHaveInvalidIds_ReturnsSuccess(
            IReadOnlyList<int> permissionIds,
            IReadOnlyList<int> validPermissionIds)
        {
            using (var testContext = new TestContext())
            {
                testContext.SetIdentitiesCache(validPermissionIds);

                var uut = testContext.BuildUut();

                var result = await uut.ValidateIdsAsync(
                    permissionIds,
                    testContext.CancellationToken);

                result.IsSuccess.ShouldBeTrue();
            }
        }

        [TestCaseSource(nameof(ValidateIdsAsync_PermissionIdsDoesNotHaveInvalidIds_TestCaseData))]
        public async Task ValidateIdsAsync_IdentitiesAreNotCached_ReadsIdentities(
            IReadOnlyList<int> permissionIds,
            IReadOnlyList<int> validPermissionIds)
        {
            using (var testContext = new TestContext())
            {
                var identities = validPermissionIds
                    .Select(x => new PermissionIdentityViewModel(
                        id:     x,
                        name:   $"Permission {x}"))
                    .ToArray();

                testContext.MockPermissionsRepository
                    .Setup(x => x.AsyncEnumerateIdentities())
                    .Returns(identities.ToAsyncEnumerable());
                
                var uut = testContext.BuildUut();

                var result = await uut.ValidateIdsAsync(
                    permissionIds,
                    testContext.CancellationToken);

                result.IsSuccess.ShouldBeTrue();

                testContext.MockPermissionsRepository.ShouldHaveReceived(x => x
                    .AsyncEnumerateIdentities());
            }
        }

        [TestCaseSource(nameof(ValidateIdsAsync_PermissionIdsDoesNotHaveInvalidIds_TestCaseData))]
        public async Task ValidateIdsAsync_IdentitiesAreCached_DoesNotReadIdentities(
            IReadOnlyList<int> permissionIds,
            IReadOnlyList<int> validPermissionIds)
        {
            using (var testContext = new TestContext())
            {
                testContext.SetIdentitiesCache(validPermissionIds);

                var uut = testContext.BuildUut();

                var result = await uut.ValidateIdsAsync(
                    permissionIds,
                    testContext.CancellationToken);

                result.IsSuccess.ShouldBeTrue();

                testContext.MockPermissionsRepository.ShouldNotHaveReceived(x => x
                    .AsyncEnumerateIdentities());
            }
        }

        #endregion ValidateIdsAsync() Tests
    }
}
