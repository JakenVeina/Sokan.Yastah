using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Moq;

namespace System.Reflection.Moq
{
    public class MockAssembly
        : Mock<Assembly>
    {
        public MockAssembly()
        {
            Setup(x => x.DefinedTypes)
                .Returns(() => MockDefinedTypes.Select(x => x.Object));
        }

        public List<MockTypeInfo> MockDefinedTypes { get; }
            = new List<MockTypeInfo>();
    }
}
