namespace Microsoft.EntityFrameworkCore
{
    public struct ConcurrencyResolutionResult
        : IEquatable<ConcurrencyResolutionResult>
    {
        public static readonly ConcurrencyResolutionResult Handled
            = new ConcurrencyResolutionResult(true);

        public static readonly ConcurrencyResolutionResult Unhandled
            = new ConcurrencyResolutionResult(false);

        internal ConcurrencyResolutionResult(
            bool isHandled)
        {
            _isHandled = isHandled;
        }

        public bool IsHandled
            => _isHandled;

        public bool IsUnhandled
            => !_isHandled;

        public bool Equals(ConcurrencyResolutionResult result)
            => _isHandled == result._isHandled;

        public override bool Equals(object obj)
            => (obj is ConcurrencyResolutionResult result)
                ? Equals(result)
                : false;

        public override int GetHashCode()
            => _isHandled.GetHashCode();

        public static bool operator ==(ConcurrencyResolutionResult x, ConcurrencyResolutionResult y)
            => x.Equals(y);

        public static bool operator !=(ConcurrencyResolutionResult x, ConcurrencyResolutionResult y)
            => !x.Equals(y);

        private readonly bool _isHandled;
    }
}
