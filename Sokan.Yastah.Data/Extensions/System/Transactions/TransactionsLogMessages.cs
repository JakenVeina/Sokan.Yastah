using Microsoft.Extensions.Logging;

using Sokan.Yastah.Data;

namespace System.Transactions
{
    public static class TransactionsLogMessages
    {
        public enum EventType
        {
            TransactionScopeCreating    = DataLogEventType.Transactions + 0x0001,
            TransactionScopeCreated     = DataLogEventType.Transactions + 0x0002,
            TransactionScopeCommitting  = DataLogEventType.Transactions + 0x0003,
            TransactionScopeCommitted   = DataLogEventType.Transactions + 0x0004,
            TransactionScopeDisposing   = DataLogEventType.Transactions + 0x0005,
            TransactionScopeDisposed    = DataLogEventType.Transactions + 0x0006
        }

        public static void TransactionScopeCommitted(
                ILogger logger)
            => _transactionScopeCommitted.Invoke(
                logger);
        private static readonly Action<ILogger> _transactionScopeCommitted
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.TransactionScopeCommitted.ToEventId(),
                    $"{nameof(ITransactionScope)} committed")
                .WithoutException();

        public static void TransactionScopeCommitting(
                ILogger logger)
            => _transactionScopeCommitting.Invoke(
                logger);
        private static readonly Action<ILogger> _transactionScopeCommitting
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.TransactionScopeCommitting.ToEventId(),
                    $"Committing {nameof(ITransactionScope)}")
                .WithoutException();

        public static void TransactionScopeCreated(
                ILogger logger)
            => _transactionScopeCreated.Invoke(
                logger);
        private static readonly Action<ILogger> _transactionScopeCreated
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.TransactionScopeCreated.ToEventId(),
                    $"{nameof(ITransactionScope)} created")
                .WithoutException();

        public static void TransactionScopeCreating(
                ILogger logger)
            => _transactionScopeCreating.Invoke(
                logger);
        private static readonly Action<ILogger> _transactionScopeCreating
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.TransactionScopeCreating.ToEventId(),
                    $"Creating {nameof(ITransactionScope)}")
                .WithoutException();

        public static void TransactionScopeDisposed(
                ILogger logger)
            => _transactionScopeDisposed.Invoke(
                logger);
        private static readonly Action<ILogger> _transactionScopeDisposed
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.TransactionScopeDisposed.ToEventId(),
                    $"{nameof(ITransactionScope)} disposed")
                .WithoutException();

        public static void TransactionScopeDisposing(
                ILogger logger)
            => _transactionScopeDisposing.Invoke(
                logger);
        private static readonly Action<ILogger> _transactionScopeDisposing
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.TransactionScopeDisposing.ToEventId(),
                    $"Disposing {nameof(ITransactionScope)}")
                .WithoutException();
    }
}
