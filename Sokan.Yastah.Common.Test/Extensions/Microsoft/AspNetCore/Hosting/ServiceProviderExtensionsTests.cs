using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;
using Moq;
using Shouldly;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Sokan.Yastah.Common.Test.Extensions.Microsoft.AspNetCore.Hosting
{
    [TestFixture]
    public class ServiceProviderExtensionsTests
    {
        #region Test Cases

        public static IEnumerable<TestCaseData> StartupHandlerCountTestCaseData
            = new[]
            {
                new TestCaseData(0)
                    .SetName("{m}(0 Startup Handlers)"),
                new TestCaseData(1)
                    .SetName("{m}(1 Startup Handler)"),
                new TestCaseData(5)
                    .SetName("{m}(5 Startup Handlers)")
            };

        public class TestContext
            : AsyncMethodTestContext
        {
            public TestContext(int startupHandlerCount)
            {
                foreach (var x in Enumerable.Range(0, startupHandlerCount))
                    _mockStartupHandlers.Add(new Mock<IStartupHandler>());

                MockServiceProvider
                    .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                    .Returns(() => MockServiceScopeFactory.Object);

                MockServiceScopeFactory
                    .Setup(x => x.CreateScope())
                    .Returns(() => MockServiceScope.Object);

                MockServiceScope
                    .Setup(x => x.ServiceProvider)
                    .Returns(() => MockScopedServiceProvider.Object);

                MockScopedServiceProvider
                    .Setup(x => x.GetService(typeof(IEnumerable<IStartupHandler>)))
                    .Returns(() => _mockStartupHandlers.Select(x => x.Object));
            }

            public Mock<IServiceProvider> MockServiceProvider { get; }
                = new Mock<IServiceProvider>();

            public Mock<IServiceScopeFactory> MockServiceScopeFactory { get; }
                = new Mock<IServiceScopeFactory>();

            public Mock<IServiceScope> MockServiceScope { get; }
                = new Mock<IServiceScope>();

            public Mock<IServiceProvider> MockScopedServiceProvider { get; }
                = new Mock<IServiceProvider>();

            public IReadOnlyList<Mock<IStartupHandler>> MockStartupHandlers
                => _mockStartupHandlers;
            private readonly List<Mock<IStartupHandler>> _mockStartupHandlers
                = new List<Mock<IStartupHandler>>();
        }

        #endregion Test Cases

        #region HandleStartupAsync() Tests

        [TestCaseSource(nameof(StartupHandlerCountTestCaseData))]
        public async Task HandleStartupAsync_Always_InvokesOnStartupAsyncForAllStartupHandlers(
            int startupHandlerCount)
        {
            using (var testContext = new TestContext(startupHandlerCount))
            {
                await testContext.MockServiceProvider.Object.HandleStartupAsync(
                    testContext.CancellationToken);

                testContext.MockServiceScopeFactory
                    .ShouldHaveReceived(x => x.CreateScope());

                testContext.MockStartupHandlers
                    .ForEach(sh => sh.ShouldHaveReceived(x => x.OnStartupAsync(testContext.CancellationToken)));

                testContext.MockServiceScope
                    .ShouldHaveReceived(x => x.Dispose());
            }
        }

        #endregion HandleStartupAsync() Tests
    }
}
