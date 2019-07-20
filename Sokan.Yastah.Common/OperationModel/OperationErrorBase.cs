namespace Sokan.Yastah.Common.OperationModel
{
    public class OperationErrorBase
        : IOperationError
    {
        protected OperationErrorBase(string message)
        {
            _message = message;
        }

        public string Code
            => GetType().Name;

        public string Message
            => _message;

        private readonly string _message;
    }
}
