namespace Sokan.Yastah.Common.OperationModel
{
    public abstract class OperationError
    {
        protected OperationError(string message)
        {
            _message = message;
        }

        public string Code
            => GetType().Name;

        public string Message
            => _message;

        public override string ToString()
            => $"{Code}: {_message}";

        private readonly string _message;
    }
}
