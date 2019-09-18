namespace System.Reflection.Moq
{
    public static class MockMemberInfoExtensions
    {
        public static MockMemberInfo<T> AddCustomAttribute<T>(this MockMemberInfo<T> @this, Action<MockCustomAttributeData> setup = null)
            where T : MemberInfo
        {
            var mockCustomAttribute = new MockCustomAttributeData();

            setup?.Invoke(mockCustomAttribute);

            @this.MockCustomAttributes.Add(mockCustomAttribute);

            return @this;
        }

        public static MockMemberInfo<T> SetDeclaringType<T>(this MockMemberInfo<T> @this, Type declaringType)
            where T : MemberInfo
        {
            @this.DeclaringType = declaringType;

            return @this;
        }
    }
}
