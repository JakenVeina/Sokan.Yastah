using System;

namespace Sokan.Yastah.Common.OperationModel
{
    public struct Optional<T>
    {
        public static readonly Optional<T> Unspecified
            = new Optional<T>();

        public static Optional<T> FromValue(T value)
            => new Optional<T>(true, value);

        private Optional(bool isSpecified, T value)
        {
            _isSpecified = isSpecified;
            _value = value;
        }

        public bool IsSpecified
            => _isSpecified;

        public bool IsUnspecified
            => !_isSpecified;

        public T Value
            => _isSpecified
                ? _value
                : throw new InvalidOperationException($"Cannot retrieve {nameof(Value)} from an unspecified {nameof(Optional<T>)}");

        private readonly bool _isSpecified;

        private readonly T _value;
    }
}
