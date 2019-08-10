using System.Collections.Generic;

namespace System.Linq
{
    public static class ValueExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this T value)
        {
            yield return value;
        }
    }
}
