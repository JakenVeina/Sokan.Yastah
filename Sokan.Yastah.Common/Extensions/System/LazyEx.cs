namespace System
{
    public static class LazyEx
    {
        public static Lazy<T> Create<T>(
                Func<T> factory)
            => new Lazy<T>(factory);

        public static Lazy<T> CreateThreadSafe<T>(
                Func<T> factory)
            => new Lazy<T>(
                factory,
                true);
    }
}
