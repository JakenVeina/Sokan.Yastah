namespace System
{
    public static class StructExtensions
    {
        public static T? ToNullable<T>(
                    this T value)
                where T : struct
            => (T?)value;
    }
}
