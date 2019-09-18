using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Moq;

using NUnit.Framework;
using Moq;
using Shouldly;

using Microsoft.AspNetCore.Hosting;

namespace Sokan.Yastah.Common.Test.Extensions.Microsoft.AspNetCore.Hosting
{
    [TestFixture]
    public class OnConfigureServicesAttributeTests
    {
        #region Test Cases

        public static readonly IEnumerable<TestCaseData> MockAssemblyTestCaseData
            = new[]
            {
                new TestCaseData(new MockAssembly())
                    .SetName("{m}: DeclaredTypes is empty"),

                new TestCaseData(new MockAssembly()
                        .AddDefinedType()
                        .AddDefinedType()
                        .AddDefinedType())
                    .SetName("{m}: DeclaredMethods is empty"),

                new TestCaseData(new MockAssembly()
                        .AddDefinedType()
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod())
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod()
                            .AddDeclaredMethod()))
                    .SetName("{m}: CustomAttributes is empty"),

                new TestCaseData(new MockAssembly()
                        .AddDefinedType()
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod(mi => mi
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor())))
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod(mi => mi
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor())
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor()))
                            .AddDeclaredMethod(mi => mi
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor())
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor())
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor()))))
                    .SetName("{m}: CustomAttributes does not contain OnConfigureServicesAttribute"),

                new TestCaseData(new MockAssembly()
                        .AddDefinedType()
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod(mi => mi
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor()))
                            .AddDeclaredMethod(mi => mi
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor(ci => ci
                                        .SetDeclaringType(typeof(OnConfigureServicesAttribute))))))
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod(mi => mi
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor())
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor(ci => ci
                                        .SetDeclaringType(typeof(OnConfigureServicesAttribute)))))
                            .AddDeclaredMethod(mi => mi
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor())
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor(ci => ci
                                        .SetDeclaringType(typeof(OnConfigureServicesAttribute))))
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor()))))
                    .SetName("{m}: CustomAttributes contains OnConfigureServicesAttribute")
        };

        #endregion Test Cases

        #region EnumerateAttachedMethods() Tests

        [TestCaseSource(nameof(MockAssemblyTestCaseData))]
        public void EnumerateAttachedMethods_Always_ReturnsDelegateForEachAttachedMethod(
            MockAssembly mockAssembly)
        {
            var mockConfigureServicesHandlerMethodInfos = mockAssembly
                .MockDefinedTypes
                .SelectMany(ti => ti.MockDeclaredMethods)
                .Where(mi => mi.MockCustomAttributes
                    .Any(cad => cad.Object.AttributeType == typeof(OnConfigureServicesAttribute)))
                .ToArray();

            var mockConfigureServicesHandlers = mockConfigureServicesHandlerMethodInfos
                .Select(mi =>
                {
                    var mockConfigureServicesHandler = new Mock<ConfigureServicesHandler>();

                    mi.Setup(x => x.CreateDelegate(It.IsAny<Type>()))
                        .Returns(mockConfigureServicesHandler.Object);

                    return mockConfigureServicesHandler;
                })
                .ToArray();

            var result = OnConfigureServicesAttribute.EnumerateAttachedMethods(mockAssembly.Object)
                .ToArray();

            mockConfigureServicesHandlerMethodInfos
                .ForEach(mi => mi.Verify(x => x.CreateDelegate(typeof(ConfigureServicesHandler))));

            result.ShouldBeSetEqualTo(mockConfigureServicesHandlers.Select(x => x.Object));
        }

        #endregion EnumerateAttachedMethods() Tests
    }
}
