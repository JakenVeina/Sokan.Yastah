namespace Sokan.Yastah.Common.Test
{
    public static class TestArray
    {
        public static T[] Unique<T>()
            #pragma warning disable CA1825
            => new T[0];
            #pragma warning restore CA1825
    }
}
