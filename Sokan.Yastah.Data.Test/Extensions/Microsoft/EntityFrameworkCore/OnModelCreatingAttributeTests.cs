using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Moq;

using NUnit.Framework;
using Moq;
using Shouldly;

using Microsoft.EntityFrameworkCore;

namespace Sokan.Yastah.Data.Test.Extensions.Microsoft.EntityFrameworkCore
{
    [TestFixture]
    public class OnModelCreatingAttributeTests
    {
        #region Test Cases

        public static readonly IEnumerable<TestCaseData> MockAssemblyTestCaseData
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
                    .SetName("{m}(CustomAttributes does not contain OnModelCreatingAttribute)"),

                new TestCaseData(new MockAssembly()
                        .AddDefinedType()
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod(mi => mi
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor()))
                            .AddDeclaredMethod(mi => mi
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor(ci => ci
                                        .SetDeclaringType(typeof(OnModelCreatingAttribute))))))
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod(mi => mi
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor())
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor(ci => ci
                                        .SetDeclaringType(typeof(OnModelCreatingAttribute)))))
                            .AddDeclaredMethod(mi => mi
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor())
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor(ci => ci
                                        .SetDeclaringType(typeof(OnModelCreatingAttribute))))
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor()))))
                    .SetName("{m}(CustomAttributes contains OnModelCreatingAttribute)")
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
                    .Any(cad => cad.Object.AttributeType == typeof(OnModelCreatingAttribute)))
                .ToArray();

            var mockModelCreatingHandlers = mockConfigureServicesHandlerMethodInfos
                .Select(mi =>
                {
                    var mockModelCreatingHandler = new Mock<Action<ModelBuilder>>();

                    mi.Setup(x => x.CreateDelegate(It.IsAny<Type>()))
                        .Returns(mockModelCreatingHandler.Object);

                    return mockModelCreatingHandler;
                })
                .ToArray();

            var result = OnModelCreatingAttribute.EnumerateAttachedMethods(mockAssembly.Object)
                .ToArray();

            mockConfigureServicesHandlerMethodInfos
                .ForEach(mi => mi.Verify(x => x.CreateDelegate(typeof(Action<ModelBuilder>))));

            result.ShouldBeSetEqualTo(mockModelCreatingHandlers.Select(x => x.Object));
        }

        #endregion EnumerateAttachedMethods() Tests
    }
}
