namespace System.Reflection.Moq
{
    public static class MockMethodBaseExtensions
    {
        public static MockMethodBase<T> AddParameter<T>(this MockMethodBase<T> @this, Action<MockParameterInfo>? setup = null)
            where T : MethodBase
        {
            var mockParameterInfo = new MockParameterInfo();

            setup?.Invoke(mockParameterInfo);

            @this.MockParameters.Add(mockParameterInfo);

            return @this;
        }
    }
}
