using System.Collections.Generic;
using System.Linq;

using Moq;

namespace System.Reflection.Moq
{
    public class MockMemberInfo<T>
        : Mock<T>
            where T : MemberInfo
    {
        public MockMemberInfo()
        {
            Setup(x => x.DeclaringType)
                .Returns(() => DeclaringType);

            Setup(x => x.CustomAttributes)
                .Returns(() => MockCustomAttributes.Select(x => x.Object));
        }

        public Type? DeclaringType { get; set; }

        public List<MockCustomAttributeData> MockCustomAttributes { get; }
            = new List<MockCustomAttributeData>();
    }
}
