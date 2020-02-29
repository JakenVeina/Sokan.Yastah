using System;
using System.Threading.Tasks;

using NUnit.Framework;
using Moq;
using Shouldly;

using Microsoft.Extensions.Options;

namespace Sokan.Yastah.Common.Test.Extensions.Microsoft.Extensions.Caching.Options
{
    [TestFixture]
    public class OptionsValidationBehaviorTests
    {
        #region Test Cases

        internal class TestContext<TOptions>
                : AsyncMethodWithLoggerTestContext
            where TOptions : class, new()
        {
            public TestContext()
            {
                MockServiceProvider
                    .Setup(x => x.GetService(typeof(IOptions<TOptions>)))
                    .Returns(() => MockOptions.Object);
            }

            public Mock<IServiceProvider> MockServiceProvider { get; }
                = new Mock<IServiceProvider>();

            public Mock<IOptions<TOptions>> MockOptions { get; set; }
                = new Mock<IOptions<TOptions>>();

            public OptionsValidationBehavior<TOptions> BuildUut()
                => new OptionsValidationBehavior<TOptions>(
                    LoggerFactory.CreateLogger<OptionsValidationBehavior<TOptions>>(),
                    MockServiceProvider.Object);
        }

        #endregion Test Cases

        #region StartAsync() Tests

        [Test]
        public async Task StartAsync_ServiceProviderDoesNotContainOptions_ThrowsException()
        {
            using var testContext = new TestContext<object>();
            
            testContext.MockServiceProvider
                .Setup(x => x.GetService(typeof(IOptions<object>)))
                .Returns(null);

            var uut = testContext.BuildUut();

            await Should.ThrowAsync<InvalidOperationException>(async () =>
            {
                await uut.StartAsync(testContext.CancellationToken);
            });
        }

        [Test]
        public async Task StartAsync_ServiceProviderContainsOptions_GetsOptionsFromServiceProvider()
        {
            using var testContext = new TestContext<object>();

            var uut = testContext.BuildUut();

            await uut.StartAsync(testContext.CancellationToken);

            testContext.MockServiceProvider.ShouldHaveReceived(x => x.GetService(typeof(IOptions<object>)));
        }

        #endregion StartAsync() Tests

        #region StopAsync() Tests

        [Test]
        public void StopAsync_Always_DoesNothing()
        {
            using var testContext = new TestContext<object>();

            var uut = testContext.BuildUut();

            var result = uut.StopAsync(testContext.CancellationToken);

            result.IsCompletedSuccessfully.ShouldBeTrue();
        }

        #endregion StopAsync() Tests
    }
}
