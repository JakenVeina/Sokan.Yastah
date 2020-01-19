namespace Sokan.Yastah.Common.OperationModel
{
    public static class ValueExtensions
    {
        public static Optional<T> ToOptional<T>(this T value)
            => Optional<T>.FromValue(value);

        public static OperationResult<T> ToSuccess<T>(this T value)
            => OperationResult<T>.FromValue(value);
    }
}
