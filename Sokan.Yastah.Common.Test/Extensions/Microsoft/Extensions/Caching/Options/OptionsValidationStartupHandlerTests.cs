using System;
using System.Threading.Tasks;

using NUnit.Framework;
using Moq;
using Shouldly;

using Microsoft.Extensions.Options;

namespace Sokan.Yastah.Common.Test.Extensions.Microsoft.Extensions.Caching.Options
{
    [TestFixture]
    public class OptionsValidationStartupHandlerTests
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

            public OptionsValidationStartupHandler<TOptions> BuildUut()
                => new OptionsValidationStartupHandler<TOptions>(
                    LoggerFactory.CreateLogger<OptionsValidationStartupHandler<TOptions>>(),
                    MockServiceProvider.Object);
        }

        #endregion Test Cases

        #region OnStartupAsync() Tests

        [Test]
        public async Task OnStartupAsync_ServiceProviderDoesNotContainOptions_ThrowsException()
        {
            using var testContext = new TestContext<object>();
            
            testContext.MockServiceProvider
                .Setup(x => x.GetService(typeof(IOptions<object>)))
                .Returns(null);

            var uut = testContext.BuildUut();

            await Should.ThrowAsync<InvalidOperationException>(async () =>
            {
                await uut.OnStartupAsync(testContext.CancellationToken);
            });
        }

        [Test]
        public async Task OnStartupAsync_ServiceProviderContainsOptions_GetsOptionsFromServiceProvider()
        {
            using var testContext = new TestContext<object>();

            var uut = testContext.BuildUut();

            await uut.OnStartupAsync(testContext.CancellationToken);

            testContext.MockServiceProvider.ShouldHaveReceived(x => x.GetService(typeof(IOptions<object>)));
        }

        #endregion OnStartupAsync() Tests
    }
}
