using Moq;

namespace System.Reflection.Moq
{
    public class MockCustomAttributeData
        : Mock<CustomAttributeData>
    {
        public MockCustomAttributeData()
        {
            Setup(x => x.Constructor)
                .Returns(() => MockConstructor!.Object);
        }

        public MockConstructorInfo? MockConstructor { get; set; }
    }
}
