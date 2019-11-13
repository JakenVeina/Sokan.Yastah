using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Moq;
using Shouldly;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Sokan.Yastah.Common.Test.Extensions.Microsoft.Extensions.Caching.Options
{
    [TestFixture]
    public class OptionsBuilderExtensionsTests
    {
        #region ValidateOnStartup() Tests

        [Test]
        public void ValidateOnStartup_Always_AddsOptionsValidationStartupHandler()
        {
            var mockServiceCollection = new Mock<IServiceCollection>();

            var mockOptionsBuilder = new Mock<OptionsBuilder<object>>(mockServiceCollection.Object, "Mock Options");

            mockOptionsBuilder.Object.ValidateOnStartup<object>();

            mockOptionsBuilder.Invocations.ShouldBeEmpty();

            mockServiceCollection.ShouldHaveReceived(x => x.Add(It.IsNotNull<ServiceDescriptor>()));

            var serviceDescriptor = mockServiceCollection.Invocations
                .Where(x => x.Method.Name == nameof(IList<ServiceDescriptor>.Add))
                .Select(x => x.Arguments[0])
                .Cast<ServiceDescriptor>()
                .First();

            serviceDescriptor.ServiceType.ShouldBe(typeof(IStartupHandler));
            serviceDescriptor.ImplementationType.ShouldBe(typeof(OptionsValidationStartupHandler<object>));
            serviceDescriptor.Lifetime.ShouldBe(ServiceLifetime.Transient);
        }

        #endregion ValidateOnStartup() Tests
    }
}
