using Moq;

namespace System.Reflection.Moq
{
    public static class FakeAssemblyExtensions
    {
        public static MockAssembly AddDefinedType(this MockAssembly @this, Action<MockTypeInfo>? setup = null)
        {
            var mockDefinedType = new MockTypeInfo();

            setup?.Invoke(mockDefinedType);

            @this.MockDefinedTypes.Add(mockDefinedType);

            return @this;
        }
    }
}
