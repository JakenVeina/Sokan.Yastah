using System;
using System.Collections.Generic;

namespace Sokan.Yastah.Common.OperationModel
{
    public struct OperationResult
        : IEquatable<OperationResult>
    {
        public static OperationResult Success
            = new OperationResult(null);

        public static OperationResult FromError(OperationError error)
            => new OperationResult(error);

        public static OperationResult<T> FromError<T>(OperationError error)
            => OperationResult<T>.FromError(error);

        public static OperationResult<T> FromValue<T>(T value)
            => OperationResult<T>.FromValue(value);

        private OperationResult(OperationError? error)
            => _error = error;

        public OperationError Error
            => _error ?? throw new InvalidOperationException($"Unable to retrieve {nameof(Error)} from a successful {nameof(OperationResult)}");

        public bool IsFailure
            => !(_error is null);

        public bool IsSuccess
            => _error is null;

        public override bool Equals(object? obj)
            => (obj is OperationResult other)
                && Equals(other);

        public bool Equals(OperationResult other)
            => EqualityComparer<OperationError?>.Default.Equals(_error, other._error);

        public override int GetHashCode()
            => HashCode.Combine(_error);

        public override string ToString()
            => IsSuccess
                ? "{Success}"
                : $"{{{_error!.ToString()}}}";

        public static implicit operator OperationResult(OperationError error)
            => FromError(error);
        
        public static bool operator ==(OperationResult x, OperationResult y)
            => x.Equals(y);

        public static bool operator !=(OperationResult x, OperationResult y)
            => !x.Equals(y);

        private readonly OperationError? _error;
    }
}
