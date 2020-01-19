using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;
using Moq;
using Shouldly;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sokan.Yastah.Common.Test.Extensions.Microsoft.AspNetCore.Hosting
{
    [TestFixture]
    public class StartupServiceTests
    {
        #region Test Context

        internal class TestContext
            : AsyncMethodTestContext
        {
            public TestContext()
            {
                MockServiceProvider = new Mock<IServiceProvider>();
                MockServiceProvider
                    .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                    .Returns(() => MockServiceScopeFactory.Object);

                MockServiceScopeFactory = new Mock<IServiceScopeFactory>();
                MockServiceScopeFactory
                    .Setup(x => x.CreateScope())
                    .Returns(() => MockServiceScope.Object);

                MockServiceScope = new Mock<IServiceScope>();
                MockServiceScope
                    .Setup(x => x.ServiceProvider)
                    .Returns(() => MockScopedServiceProvider.Object);

                MockScopedServiceProvider = new Mock<IServiceProvider>();
                MockScopedServiceProvider
                    .Setup(x => x.GetService(typeof(IEnumerable<IStartupHandler>)))
                    .Returns(() => MockStartupHandlers.Select(x => x.Object));

                MockStartupHandlers = new List<Mock<IStartupHandler>>();
            }

            public readonly Mock<IServiceProvider> MockServiceProvider;
            public readonly Mock<IServiceScopeFactory> MockServiceScopeFactory;
            public readonly Mock<IServiceScope> MockServiceScope;
            public readonly Mock<IServiceProvider> MockScopedServiceProvider;
            public readonly List<Mock<IStartupHandler>> MockStartupHandlers;

            public StartupService BuildUut()
                => new StartupService(
                    MockServiceProvider.Object);
        }

        #endregion Test Context

        #region StartAsync() Tests

        public static IReadOnlyList<TestCaseData> StartAsync_TestCaseData
            = new[]
            {
                /*                  startupHandlerCount */
                new TestCaseData(   1                   ).SetName("{m}(1 Startup Handler)"),
                new TestCaseData(   0                   ).SetName("{m}(0 Startup Handlers)"),
                new TestCaseData(   5                   ).SetName("{m}(5 Startup Handlers)")
            };

        [TestCaseSource(nameof(StartAsync_TestCaseData))]
        public async Task StartAsync_Always_InvokesOnStartupAsyncForAllStartupHandlers(
            int startupHandlerCount)
        {
            using var testContext = new TestContext();

            foreach (var _ in Enumerable.Repeat(0, startupHandlerCount))
                testContext.MockStartupHandlers.Add(new Mock<IStartupHandler>());

            var uut = testContext.BuildUut();

            await uut.StartAsync(
                testContext.CancellationToken);
            
            testContext.MockServiceScopeFactory
                .ShouldHaveReceived(x => x.CreateScope());

            testContext.MockStartupHandlers
                .ForEach(sh => sh.ShouldHaveReceived(x => x.OnStartupAsync(testContext.CancellationToken)));

            testContext.MockServiceScope
                .ShouldHaveReceived(x => x.Dispose());
        }

        #endregion StartAsync() Tests

        #region StopAsync() Tests

        [Test]
        public void StopAsync_Always_DoesNothing()
        {
            using var testContext = new TestContext();

            var uut = testContext.BuildUut();

            var result = uut.StopAsync(
                testContext.CancellationToken);

            result.IsCompletedSuccessfully.ShouldBeTrue();

            testContext.MockServiceScope.Invocations.ShouldBeEmpty();
        }

        #endregion StopAsync() Tests
    }
}
