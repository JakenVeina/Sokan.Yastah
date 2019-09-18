using System.Collections.Generic;
using System.Linq;
using System.Reflection.Moq;

using NUnit.Framework;
using Moq;
using Shouldly;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Sokan.Yastah.Common.Test.Extensions.Microsoft.AspNetCore.Hosting
{
    [TestFixture]
    public class ServiceCollectionExtensionsTests
    {
        #region Test Cases

        public static readonly IEnumerable<TestCaseData> MockAssemblyTestCaseData
            = new[]
            {
                new TestCaseData(new MockAssembly())
                    .SetName("{m}: Assembly does not contain OnConfigureServicesAttribute methods"),

                new TestCaseData(new MockAssembly()
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod(mi => mi
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor(ci => ci
                                        .SetDeclaringType(typeof(OnConfigureServicesAttribute)))))
                            .AddDeclaredMethod(mi => mi
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor(ci => ci
                                        .SetDeclaringType(typeof(OnConfigureServicesAttribute))))))
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod(mi => mi
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor(ci => ci
                                        .SetDeclaringType(typeof(OnConfigureServicesAttribute)))))))
                    .SetName("{m}: Assembly contains OnConfigureServicesAttribute methods")
        };

        #endregion Test Cases

        #region AddAssembly() Tests

        [TestCaseSource(nameof(MockAssemblyTestCaseData))]
        public void AddAssembly_Always_InvokesEachOnConfigureServicesMethodInAssembly(
            MockAssembly mockAssembly)
        {
            var mockConfigureServicesHandlers = mockAssembly
                .MockDefinedTypes
                .SelectMany(ti => ti.MockDeclaredMethods)
                .Where(mi => mi.MockCustomAttributes
                    .Any(cad => cad.Object.AttributeType == typeof(OnConfigureServicesAttribute)))
                .Select(mi =>
                {
                    var mockConfigureServicesHandler = new Mock<ConfigureServicesHandler>();

                    mi.Setup(x => x.CreateDelegate(typeof(ConfigureServicesHandler)))
                        .Returns(mockConfigureServicesHandler.Object);

                    return mockConfigureServicesHandler;
                })
                .ToArray();

            var mockServiceCollection = new Mock<IServiceCollection>();
            var mockConfiguration = new Mock<IConfiguration>();

            var result = mockServiceCollection.Object.AddAssembly(mockAssembly.Object, mockConfiguration.Object);

            result.ShouldBeSameAs(mockServiceCollection.Object);

            foreach(var mockConfigureServicesHandler in mockConfigureServicesHandlers)
                mockConfigureServicesHandler.Verify(x => x
                    .Invoke(mockServiceCollection.Object, mockConfiguration.Object));
        }

        #endregion AddAssembly() Tests
    }
}
