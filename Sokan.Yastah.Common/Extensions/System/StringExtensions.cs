namespace System
{
    public static class StringExtensions
    {
        public static long ParseInt64(this string value)
            => long.Parse(value);

        public static ulong ParseUInt64(this string value)
            => ulong.Parse(value);

        public static string ToCamelCaseFromPascalCase(
                this string value)
            => value switch
            {
                { Length: 0 }   => string.Empty,
                { Length: 1 }   => value.ToLowerInvariant(),
                _               => char.ToLowerInvariant(value[0]) + value.Substring(1)
            };
    }
}
