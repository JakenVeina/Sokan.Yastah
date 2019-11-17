using System;
using System.Collections.Generic;

namespace Sokan.Yastah.Common.OperationModel
{
    public struct Optional<T>
        : IEquatable<Optional<T>>
    {
        public static readonly Optional<T> Unspecified
            = new Optional<T>(false, default!);

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

        public override bool Equals(object? obj)
            => (obj is Optional<T> other)
                && Equals(other);

        public bool Equals(Optional<T> other)
            => EqualityComparer<bool>.Default.Equals(_isSpecified, other._isSpecified)
                && EqualityComparer<T>.Default.Equals(_value, other._value);

        public override int GetHashCode()
            => HashCode.Combine(_isSpecified, _value);

        public static implicit operator Optional<T>(T value)
            => FromValue(value);

        public static bool operator ==(Optional<T> x, Optional<T> y)
            => x.Equals(y);

        public static bool operator !=(Optional<T> x, Optional<T> y)
            => !x.Equals(y);

        private readonly bool _isSpecified;
        private readonly T _value;
    }
}
