using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Moq;

using NUnit.Framework;
using Moq;
using Shouldly;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Sokan.Yastah.Data.Test.Extensions.Microsoft.EntityFrameworkCore
{
    [TestFixture]
    public class ModelBuilderExtensionsTests
    {
        #region Test Cases

        public static readonly IEnumerable<TestCaseData> MockAssemblyTestCaseData
            = new[]
            {
                new TestCaseData(new MockAssembly())
                    .SetName("{m}(Assembly does not contain OnModelCreatingAttribute methods)"),

                new TestCaseData(new MockAssembly()
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod(mi => mi
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor(ci => ci
                                        .SetDeclaringType(typeof(OnModelCreatingAttribute)))))
                            .AddDeclaredMethod(mi => mi
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor(ci => ci
                                        .SetDeclaringType(typeof(OnModelCreatingAttribute))))))
                        .AddDefinedType(ti => ti
                            .AddDeclaredMethod(mi => mi
                                .AddCustomAttribute(cad => cad
                                    .SetConstructor(ci => ci
                                        .SetDeclaringType(typeof(OnModelCreatingAttribute)))))))
                    .SetName("{m}(Assembly contains OnModelCreatingAttribute methods)")
        };

        #endregion Test Cases

        #region ApplyAssembly() Tests

        [TestCaseSource(nameof(MockAssemblyTestCaseData))]
        public void AddAssembly_Always_InvokesEachOnConfigureServicesMethodInAssembly(
            MockAssembly mockAssembly)
        {
            var mockModelCreatingHandlers = mockAssembly
                .MockDefinedTypes
                .SelectMany(ti => ti.MockDeclaredMethods)
                .Where(mi => mi.MockCustomAttributes
                    .Any(cad => cad.Object.AttributeType == typeof(OnModelCreatingAttribute)))
                .Select(mi =>
                {
                    var mockModelCreatingHandler = new Mock<Action<ModelBuilder>>();

                    mi.Setup(x => x.CreateDelegate(typeof(Action<ModelBuilder>)))
                        .Returns(mockModelCreatingHandler.Object);

                    return mockModelCreatingHandler;
                })
                .ToArray();

            var mockModelBuilder = new Mock<ModelBuilder>(new ConventionSet());

            var result = mockModelBuilder.Object.ApplyAssembly(mockAssembly.Object);

            result.ShouldBeSameAs(mockModelBuilder.Object);

            foreach(var mockModelCreatingHandler in mockModelCreatingHandlers)
                mockModelCreatingHandler.Verify(x => x
                    .Invoke(mockModelBuilder.Object));
        }

        #endregion ApplyAssembly() Tests
    }
}
