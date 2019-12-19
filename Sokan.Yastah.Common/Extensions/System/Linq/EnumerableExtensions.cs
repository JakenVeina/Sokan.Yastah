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
            foreach (var _ in sequence) ;
        }

        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (var item in sequence)
                action.Invoke(item);
        }

        public static bool SetEquals<T>(this IEnumerable<T> first, IEnumerable<T> second)
            => first.OrderBy(x => x)
                .SequenceEqual(second.OrderBy(x => x));
    }
}
