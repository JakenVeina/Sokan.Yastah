namespace System.Reflection.Moq
{
    public static class MockParameterInfoExtensions
    {
        public static MockParameterInfo SetParameterType(this MockParameterInfo @this, Type parameterType)
        {
            @this.ParameterType = parameterType;

            return @this;
        }
    }
}
