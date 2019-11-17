using Moq;

namespace System.Reflection.Moq
{
    public class MockParameterInfo
        : Mock<ParameterInfo>
    {
        public MockParameterInfo()
        {
            Setup(x => x.ParameterType)
                .Returns(() => ParameterType!);
        }

        public Type? ParameterType { get; set; }
    }
}
