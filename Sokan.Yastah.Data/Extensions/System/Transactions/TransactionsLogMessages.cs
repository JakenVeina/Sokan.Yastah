using Microsoft.Extensions.Logging;

namespace System.Transactions
{
    internal static class TransactionsLogMessages
    {
        public static void TransactionScopeCreating(
                ILogger logger)
            => _transactionScopeCreating.Invoke(
                logger);
        private static readonly Action<ILogger> _transactionScopeCreating
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4201, nameof(TransactionScopeCreating)),
                    $"Creating {nameof(ITransactionScope)}")
                .WithoutException();

        public static void TransactionScopeCreated(
                ILogger logger)
            => _transactionScopeCreated.Invoke(
                logger);
        private static readonly Action<ILogger> _transactionScopeCreated
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4202, nameof(TransactionScopeCreated)),
                    $"{nameof(ITransactionScope)} created")
                .WithoutException();

        public static void TransactionScopeCommitting(
                ILogger logger)
            => _transactionScopeCommitting.Invoke(
                logger);
        private static readonly Action<ILogger> _transactionScopeCommitting
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4203, nameof(TransactionScopeCommitting)),
                    $"Committing {nameof(ITransactionScope)}")
                .WithoutException();

        public static void TransactionScopeCommitted(
                ILogger logger)
            => _transactionScopeCommitted.Invoke(
                logger);
        private static readonly Action<ILogger> _transactionScopeCommitted
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4204, nameof(TransactionScopeCommitted)),
                    $"{nameof(ITransactionScope)} committed")
                .WithoutException();

        public static void TransactionScopeDisposing(
                ILogger logger)
            => _transactionScopeDisposing.Invoke(
                logger);
        private static readonly Action<ILogger> _transactionScopeDisposing
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4205, nameof(TransactionScopeDisposing)),
                    $"Disposing {nameof(ITransactionScope)}")
                .WithoutException();

        public static void TransactionScopeDisposed(
                ILogger logger)
            => _transactionScopeDisposed.Invoke(
                logger);
        private static readonly Action<ILogger> _transactionScopeDisposed
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4206, nameof(TransactionScopeDisposed)),
                    $"{nameof(ITransactionScope)} disposed")
                .WithoutException();
    }
}
