using System.Collections.Generic;
using System.Linq;

using Moq;

namespace System.Reflection.Moq
{
    public class MockTypeInfo
        : Mock<TypeInfo>
    {
        public MockTypeInfo()
        {
            Setup(x => x.DeclaredMethods)
                .Returns(() => MockDeclaredMethods.Select(x => x.Object));
        }

        public List<MockMethodInfo> MockDeclaredMethods { get; }
            = new List<MockMethodInfo>();
    }
}
