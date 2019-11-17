using System.Collections.Generic;
using System.Linq;

namespace System.Reflection.Moq
{
    public class MockMethodBase<T>
        : MockMemberInfo<T>
            where T : MethodBase
    {
        public MockMethodBase()
        {
            Setup(x => x.GetParameters())
                .Returns(() => MockParameters.Select(x => x.Object).ToArray());
        }

        public List<MockParameterInfo> MockParameters { get; }
            = new List<MockParameterInfo>();
    }
}
