using System;

namespace Sokan.Yastah.Common.OperationModel
{
    public struct OperationResult
    {
        public static OperationResult Success
            = new OperationResult(null);

        public static OperationResult FromError(IOperationError error)
        {
            if (error is null)
                throw new ArgumentNullException(nameof(error));

            return new OperationResult(error);
        }

        public static OperationResult<T> FromError<T>(IOperationError error)
            => OperationResult<T>.FromError(error);

        public static OperationResult<T> FromValue<T>(T value)
            => OperationResult<T>.FromValue(value);

        private OperationResult(IOperationError error)
            => _error = error;

        public IOperationError Error
            => _error ?? throw new InvalidOperationException($"Unable to retrieve {nameof(Error)} from a successful {nameof(OperationResult)}");

        public bool IsFailure
            => !(_error is null);

        public bool IsSuccess
            => _error is null;

        private readonly IOperationError _error;
    }
}
