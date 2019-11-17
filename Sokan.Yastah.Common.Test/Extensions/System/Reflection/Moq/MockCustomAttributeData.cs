using Moq;

namespace System.Reflection.Moq
{
    public class MockCustomAttributeData
        : Mock<CustomAttributeData>
    {
        public MockCustomAttributeData()
        {
            Setup(x => x.AttributeType)
                .Returns(() => AttributeType!);

            Setup(x => x.Constructor)
                .Returns(() => MockConstructor!.Object);
        }

        public Type? AttributeType { get; set; }

        public MockConstructorInfo? MockConstructor { get; set; }
    }
}
