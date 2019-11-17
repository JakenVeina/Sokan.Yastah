using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Moq;

using NUnit.Framework;
using Moq;
using Shouldly;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                    .SetName("{m}(Assembly does not contain OnConfigureServicesAttribute methods)"),

                new TestCaseData(new MockAssembly()
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod(mi => mi
                                .AddParameter(pi => pi
                                    .SetParameterType(typeof(IServiceCollection)))
                                .AddParameter(pi => pi
                                    .SetParameterType(typeof(IConfiguration)))
                                .AddCustomAttribute(cad => cad
                                    .SetAttributeType(typeof(OnConfigureServicesAttribute))))
                            .AddDeclaredMethod(mi => mi
                                .AddParameter(pi => pi
                                    .SetParameterType(typeof(IServiceCollection)))
                                .AddCustomAttribute(cad => cad
                                    .SetAttributeType(typeof(OnConfigureServicesAttribute)))))
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod(mi => mi
                                .AddParameter(pi => pi
                                    .SetParameterType(typeof(IServiceCollection)))
                                .AddParameter(pi => pi
                                    .SetParameterType(typeof(IConfiguration)))
                                .AddCustomAttribute(cad => cad
                                    .SetAttributeType(typeof(OnConfigureServicesAttribute))))))
                    .SetName("{m}(Assembly contains OnConfigureServicesAttribute methods)")
        };

        #endregion Test Cases

        #region AddAssembly() Tests

        [TestCaseSource(nameof(MockAssemblyTestCaseData))]
        public void AddAssembly_Always_InvokesEachOnConfigureServicesMethodInAssembly(
            MockAssembly mockAssembly)
        {
            var mockDelegates = mockAssembly
                .MockDefinedTypes
                .SelectMany(ti => ti.MockDeclaredMethods)
                .Where(mi => mi.MockCustomAttributes
                    .Any(cad => cad.Object.AttributeType == typeof(OnConfigureServicesAttribute)))
                .Select(mi =>
                {
                    if(mi.MockParameters.Count == 1)
                    {
                        var mockConfigureServicesHandler = new Mock<ConfigureServicesHandler>();

                        mi.Setup(x => x.CreateDelegate(typeof(ConfigureServicesHandler)))
                            .Returns(mockConfigureServicesHandler.Object);

                        return mockConfigureServicesHandler as Mock;
                    }
                    else
                    {
                        var mockConfigureServicesWithConfigurationHandler = new Mock<ConfigureServicesWithConfigurationHandler>();

                        mi.Setup(x => x.CreateDelegate(typeof(ConfigureServicesWithConfigurationHandler)))
                            .Returns(mockConfigureServicesWithConfigurationHandler.Object);

                        return mockConfigureServicesWithConfigurationHandler as Mock;
                    }
                })
                .ToArray();

            var mockServiceCollection = new Mock<IServiceCollection>();
            var mockConfiguration = new Mock<IConfiguration>();

            var result = mockServiceCollection.Object.AddAssembly(mockAssembly.Object, mockConfiguration.Object);

            result.ShouldBeSameAs(mockServiceCollection.Object);

            foreach(var mockDelegate in mockDelegates)
            {
                if(mockDelegate is Mock<ConfigureServicesHandler> mockConfigureServicesHandler)
                    mockConfigureServicesHandler.Verify(x => x
                        .Invoke(mockServiceCollection.Object));
                else if (mockDelegate is Mock<ConfigureServicesWithConfigurationHandler> mockConfigureServicesWithConfigurationHandler)
                    mockConfigureServicesWithConfigurationHandler.Verify(x => x
                        .Invoke(mockServiceCollection.Object, mockConfiguration.Object));
            }
        }

        #endregion AddAssembly() Tests
    }
}
