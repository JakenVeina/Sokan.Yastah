namespace System.Reflection.Moq
{
    public static class MockTypeInfoExtensions
    {
        public static MockTypeInfo AddDeclaredMethod(this MockTypeInfo @this, Action<MockMethodInfo>? setup = null)
        {
            var mockDeclaredMethod = new MockMethodInfo();

            setup?.Invoke(mockDeclaredMethod);

            @this.MockDeclaredMethods.Add(mockDeclaredMethod);

            return @this;
        }
    }
}
