namespace System
{
    public static class StringExtensions
    {
        public static ulong ParseUInt64(this string value)
            => ulong.Parse(value);
    }
}
