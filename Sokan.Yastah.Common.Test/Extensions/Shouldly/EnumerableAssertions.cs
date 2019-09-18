using System;
using System.Collections.Generic;

namespace Shouldly
{
    public static class EnumerableAssertions
    {
        public static void ShouldBeSequenceEqualTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
            => actual.ShouldBe(expected, ignoreOrder: false);

        public static void ShouldBeSequenceEqualTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected, string customMessage)
            => actual.ShouldBe(expected, ignoreOrder: false, customMessage);

        public static void ShouldBeSequenceEqualTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected, Func<string> customMessage)
            => actual.ShouldBe(expected, ignoreOrder: false, customMessage);

        public static void ShouldBeSetEqualTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
            => actual.ShouldBe(expected, ignoreOrder: true);

        public static void ShouldBeSetEqualTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected, string customMessage)
            => actual.ShouldBe(expected, ignoreOrder: true, customMessage);

        public static void ShouldBeSetEqualTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected, Func<string> customMessage)
            => actual.ShouldBe(expected, ignoreOrder: true, customMessage);
    }
}
