namespace System
{
    public static class StringExtensions
    {
        public static long ParseInt64(this string value)
            => long.Parse(value);

        public static ulong ParseUInt64(this string value)
            => ulong.Parse(value);
    }
}
