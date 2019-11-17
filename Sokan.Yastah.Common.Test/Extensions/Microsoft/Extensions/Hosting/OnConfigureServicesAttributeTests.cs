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
    public class OnConfigureServicesAttributeTests
    {
        #region Test Cases

        public static readonly IEnumerable<TestCaseData> MockAssembly_TaggedMethodsAreNotValid_TestCaseData
            = new[]
            {
                new TestCaseData(new MockAssembly()
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod(mi => mi
                                .SetDeclaringType(typeof(OnConfigureServicesAttributeTests))
                                .AddCustomAttribute(cad => cad
                                    .SetAttributeType(typeof(OnConfigureServicesAttribute))))))
                    .SetName("{m}(GetParameters() is empty)"),
                new TestCaseData(new MockAssembly()
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod(mi => mi
                                .AddParameter(pi => pi
                                    .SetParameterType(typeof(IConfiguration)))
                                .SetDeclaringType(typeof(OnConfigureServicesAttributeTests))
                                .AddCustomAttribute(cad => cad
                                    .SetAttributeType(typeof(OnConfigureServicesAttribute))))))
                    .SetName("{m}(GetParameters()[0] is wrong type)"),
                new TestCaseData(new MockAssembly()
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod(mi => mi
                                .AddParameter(pi => pi
                                    .SetParameterType(typeof(IServiceCollection)))
                                .AddParameter(pi => pi
                                    .SetParameterType(typeof(IServiceCollection)))
                                .SetDeclaringType(typeof(OnConfigureServicesAttributeTests))
                                .AddCustomAttribute(cad => cad
                                    .SetAttributeType(typeof(OnConfigureServicesAttribute))))))
                    .SetName("{m}(GetParameters()[1] is wrong type)"),
                new TestCaseData(new MockAssembly()
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod(mi => mi
                                .AddParameter(pi => pi
                                    .SetParameterType(typeof(IConfiguration)))
                                .AddParameter(pi => pi
                                    .SetParameterType(typeof(IServiceCollection)))
                                .SetDeclaringType(typeof(OnConfigureServicesAttributeTests))
                                .AddCustomAttribute(cad => cad
                                    .SetAttributeType(typeof(OnConfigureServicesAttribute))))))
                    .SetName("{m}(GetParameters() is wrong order)")
        };

        public static readonly IEnumerable<TestCaseData> MockAssembly_TaggedMethodsAreValid_TestCaseData
            = new[]
            {
                new TestCaseData(new MockAssembly())
                    .SetName("{m}(DeclaredTypes is empty)"),

                new TestCaseData(new MockAssembly()
                        .AddDefinedType()
                        .AddDefinedType()
                        .AddDefinedType())
                    .SetName("{m}(DeclaredMethods is empty)"),

                new TestCaseData(new MockAssembly()
                        .AddDefinedType()
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod())
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod()
                            .AddDeclaredMethod()))
                    .SetName("{m}(CustomAttributes is empty)"),

                new TestCaseData(new MockAssembly()
                        .AddDefinedType()
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod(mi => mi
                                .AddCustomAttribute()))
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod(mi => mi
                                .AddCustomAttribute()
                                .AddCustomAttribute())
                            .AddDeclaredMethod(mi => mi
                                .AddCustomAttribute()
                                .AddCustomAttribute()
                                .AddCustomAttribute())))
                    .SetName("{m}(CustomAttributes does not contain OnConfigureServicesAttribute)"),

                new TestCaseData(new MockAssembly()
                        .AddDefinedType()
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod(mi => mi
                                .AddCustomAttribute())
                            .AddDeclaredMethod(mi => mi
                                .AddParameter(pi => pi
                                    .SetParameterType(typeof(IServiceCollection)))
                                .AddParameter(pi => pi
                                    .SetParameterType(typeof(IConfiguration)))
                                .AddCustomAttribute(cad => cad
                                    .SetAttributeType(typeof(OnConfigureServicesAttribute)))))
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod(mi => mi
                                .AddParameter(pi => pi
                                    .SetParameterType(typeof(IServiceCollection)))
                                .AddCustomAttribute()
                                .AddCustomAttribute(cad => cad
                                    .SetAttributeType(typeof(OnConfigureServicesAttribute))))
                            .AddDeclaredMethod(mi => mi
                                .AddParameter(pi => pi
                                    .SetParameterType(typeof(IServiceCollection)))
                                .AddParameter(pi => pi
                                    .SetParameterType(typeof(IConfiguration)))
                                .AddCustomAttribute()
                                .AddCustomAttribute(cad => cad
                                    .SetAttributeType(typeof(OnConfigureServicesAttribute)))
                                .AddCustomAttribute())))
                    .SetName("{m}(CustomAttributes contains OnConfigureServicesAttribute)")
        };

        #endregion Test Cases

        #region EnumerateAttachedMethods() Tests

        [TestCaseSource(nameof(MockAssembly_TaggedMethodsAreNotValid_TestCaseData))]
        public void EnumerateAttachedMethods_MethodsHaveIncompatibleSignatures_ThrowsException(
            MockAssembly mockAssembly)
        {
            var mockConfigureServicesHandlerMethodInfos = mockAssembly
                .MockDefinedTypes
                .SelectMany(ti => ti.MockDeclaredMethods)
                .Where(mi => mi.MockCustomAttributes
                    .Any(cad => cad.Object.AttributeType == typeof(OnConfigureServicesAttribute)))
                .ToArray();

            var result = Should.Throw<ArgumentException>(() =>
            {
                _ = OnConfigureServicesAttribute.EnumerateAttachedMethods(mockAssembly.Object)
                    .ToArray();
            });

            result.Message.ShouldContain(nameof(OnConfigureServicesAttribute));
            result.Message.ShouldContain(nameof(ConfigureServicesHandler));
            result.Message.ShouldContain(nameof(ConfigureServicesWithConfigurationHandler));

            mockConfigureServicesHandlerMethodInfos
                .ForEach(mi => mi
                    .ShouldNotHaveReceived(x => x.CreateDelegate(It.IsAny<Type>())));
        }

        [TestCaseSource(nameof(MockAssembly_TaggedMethodsAreValid_TestCaseData))]
        public void EnumerateAttachedMethods_Otherwise_ReturnsDelegateForEachAttachedMethod(
            MockAssembly mockAssembly)
        {
            var mockConfigureServicesHandlerMethodInfos = mockAssembly
                .MockDefinedTypes
                .SelectMany(ti => ti.MockDeclaredMethods)
                .Where(mi => mi.MockCustomAttributes
                    .Any(cad => cad.Object.AttributeType == typeof(OnConfigureServicesAttribute)))
                .ToArray();

            var mockDelegates = mockConfigureServicesHandlerMethodInfos
                .Select(mi =>
                {
                    if (mi.MockParameters.Count == 1)
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

            var result = OnConfigureServicesAttribute.EnumerateAttachedMethods(mockAssembly.Object)
                .ToArray();

            mockConfigureServicesHandlerMethodInfos
                .ForEach(mi =>
                {
                    if (mi.MockParameters.Count == 1)
                        mi.ShouldHaveReceived(x => x.CreateDelegate(typeof(ConfigureServicesHandler)));
                    else
                        mi.ShouldHaveReceived(x => x.CreateDelegate(typeof(ConfigureServicesWithConfigurationHandler)));
                });

            result.ShouldBeSetEqualTo(mockDelegates.Select(x => x.Object));
        }

        #endregion EnumerateAttachedMethods() Tests
    }
}
