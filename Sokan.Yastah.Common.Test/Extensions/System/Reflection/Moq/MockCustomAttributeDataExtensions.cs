namespace System.Reflection.Moq
{
    public static class MockCustomAttributeDataExtensions
    {
        public static MockCustomAttributeData SetAttributeType(this MockCustomAttributeData @this, Type attributeType)
        {
            @this.AttributeType = attributeType;

            return @this;
        }

        public static MockCustomAttributeData SetConstructor(this MockCustomAttributeData @this, Action<MockConstructorInfo>? setup = null)
        {
            var mockConstructor = new MockConstructorInfo();

            setup?.Invoke(mockConstructor);

            @this.MockConstructor = mockConstructor;

            return @this;
        }
    }
}
