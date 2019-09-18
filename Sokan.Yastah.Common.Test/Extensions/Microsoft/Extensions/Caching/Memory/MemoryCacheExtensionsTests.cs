using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;
using Moq;
using Shouldly;

using Microsoft.Extensions.Caching.Memory;

namespace Sokan.Yastah.Common.Test.Extensions.Microsoft.Extensions.Caching.Memory
{
    [TestFixture]
    public class MemoryCacheExtensionsTests
    {
        #region Test Cases

        public static readonly IReadOnlyList<object> ItemTestCases
            = new []
            {
                null,
                new object()
            };

        public class TestContext
        {
            public TestContext()
            {
                MockMemoryCache
                    .Setup(x => x.CreateEntry(It.IsAny<object>()))
                    .Returns(() => MockCacheEntry.Object);

                MockFactory
                    .Setup(x => x.Invoke(It.IsAny<ICacheEntry>()))
                    .ReturnsAsync(() => Item);
            }

            public Mock<IMemoryCache> MockMemoryCache { get; }
                = new Mock<IMemoryCache>();

            public Mock<ICacheEntry> MockCacheEntry { get; }
                = new Mock<ICacheEntry>();

            public Mock<Func<ICacheEntry, Task<object>>> MockFactory { get; }
                = new Mock<Func<ICacheEntry, Task<object>>>();

            public object Item { get; set; }

            public delegate void TryGetValueCallback(object key, out object value);
        }

        #endregion Test Cases

        #region OptimisticGetOrCreateAsync() Tests

        [TestCaseSource(nameof(ItemTestCases))]
        public async Task OptimisticGetOrCreateAsync_MemoryCacheDoesNotContainKey_CreatesCacheEntry(
            object item)
        {
            var testContext = new TestContext()
            {
                Item = item
            };

            testContext.MockMemoryCache
                .Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
                .Callback(new TestContext.TryGetValueCallback((object k, out object v) => v = default))
                .Returns(false);

            var key = new object();

            var result = await testContext.MockMemoryCache.Object.OptimisticGetOrCreateAsync(
                key,
                testContext.MockFactory.Object);

            result.ShouldBeSameAs(item);

            testContext.MockMemoryCache.ShouldHaveReceived(x => x.CreateEntry(key));

            testContext.MockFactory.ShouldHaveReceived(x => x.Invoke(testContext.MockCacheEntry.Object));

            testContext.MockCacheEntry.ShouldHaveReceivedSet(x => x.Value = item);
            testContext.MockCacheEntry.ShouldHaveReceived(x => x.Dispose());
        }

        [TestCaseSource(nameof(ItemTestCases))]
        public async Task OptimisticGetOrCreateAsync_FactoryThrowsException_DoesNotCommitCacheEntry(
            object item)
        {
            var testContext = new TestContext()
            {
                Item = item
            };

            testContext.MockMemoryCache
                .Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
                .Callback(new TestContext.TryGetValueCallback((object k, out object v) => v = default))
                .Returns(false);

            testContext.MockFactory
                .Setup(x => x.Invoke(It.IsAny<ICacheEntry>()))
                .Returns(() => throw new Exception());

            var key = new object();

            await Should.ThrowAsync<Exception>(async () =>
            {
                await testContext.MockMemoryCache.Object.OptimisticGetOrCreateAsync(
                    key,
                    testContext.MockFactory.Object);
            });

            testContext.MockMemoryCache.ShouldHaveReceived(x => x.CreateEntry(key));

            testContext.MockFactory.ShouldHaveReceived(x => x.Invoke(testContext.MockCacheEntry.Object));

            testContext.MockCacheEntry.ShouldNotHaveReceivedSet(x => x.Value = It.IsAny<object>());
            testContext.MockCacheEntry.ShouldNotHaveReceived(x => x.Dispose());
        }

        [TestCaseSource(nameof(ItemTestCases))]
        public async Task OptimisticGetOrCreateAsync_MemoryCacheContainsKey_DoesNotCreateCacheEntry(
            object item)
        {
            var testContext = new TestContext()
            {
                Item = item
            };

            testContext.MockMemoryCache
                .Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
                .Callback(new TestContext.TryGetValueCallback((object k, out object v) => v = item))
                .Returns(true);

            var key = new object();

            var result = await testContext.MockMemoryCache.Object.OptimisticGetOrCreateAsync(
                key,
                testContext.MockFactory.Object);

            result.ShouldBeSameAs(item);

            testContext.MockMemoryCache.ShouldNotHaveReceived(x => x.CreateEntry(key));

            testContext.MockFactory.Invocations.ShouldBeEmpty();

            testContext.MockCacheEntry.Invocations.ShouldBeEmpty();
        }

        #endregion OptimisticGetOrCreateAsync() Tests
    }
}
