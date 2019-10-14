using System.Collections.Generic;

namespace System.Linq
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Do<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (var item in sequence)
            {
                action.Invoke(item);
                yield return item;
            }
        }

        public static void Enumerate<T>(this IEnumerable<T> sequence)
        {
            foreach (var item in sequence) ;
        }

        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (var item in sequence)
                action.Invoke(item);
        }
    }
}
