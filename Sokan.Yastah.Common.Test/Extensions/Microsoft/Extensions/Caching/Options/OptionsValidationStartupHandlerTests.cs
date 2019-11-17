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

        internal class TestContext
            : AsyncMethodTestContext
        {
            public TestContext()
            {
                MockServiceProvider
                    .Setup(x => x.GetService(typeof(IOptions<object>)))
                    .Returns(() => MockOptions.Object);
            }

            public Mock<IServiceProvider> MockServiceProvider { get; }
                = new Mock<IServiceProvider>();

            public Mock<IOptions<object>> MockOptions { get; set; }
                = new Mock<IOptions<object>>();
        }

        #endregion Test Cases

        #region OnStartupAsync() Tests

        [Test]
        public async Task OnStartupAsync_ServiceProviderDoesNotContainOptions_ThrowsException()
        {
            using var testContext = new TestContext();
            
            testContext.MockServiceProvider
                .Setup(x => x.GetService(typeof(IOptions<object>)))
                .Returns(null);

            var uut = new OptionsValidationStartupHandler<object>(
                testContext.MockServiceProvider.Object);

            await Should.ThrowAsync<InvalidOperationException>(async () =>
            {
                await uut.OnStartupAsync(testContext.CancellationToken);
            });
        }

        [Test]
        public async Task OnStartupAsync_ServiceProviderContainsOptions_GetsOptionsFromServiceProvider()
        {
            using var testContext = new TestContext();
            
            var uut = new OptionsValidationStartupHandler<object>(
                testContext.MockServiceProvider.Object);

            await uut.OnStartupAsync(testContext.CancellationToken);

            testContext.MockServiceProvider.ShouldHaveReceived(x => x.GetService(typeof(IOptions<object>)));

            testContext.MockOptions.Invocations.ShouldBeEmpty();
        }

        #endregion OnStartupAsync() Tests
    }
}
