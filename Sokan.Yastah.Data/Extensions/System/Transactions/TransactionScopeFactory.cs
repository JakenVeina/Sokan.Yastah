using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace System.Transactions
{
    public interface ITransactionScopeFactory
    {
        ITransactionScope CreateScope(
            IsolationLevel? isolationLevel = default);
    }

    [ServiceBinding(ServiceLifetime.Singleton)]
    public class TransactionScopeFactory
        : ITransactionScopeFactory
    {
        public TransactionScopeFactory(
            ILogger<TransactionScopeFactory> logger)
        {
            _logger = logger;
        }

        public ITransactionScope CreateScope(
                IsolationLevel? isolationLevel = default)
        {
            TransactionsLogMessages.TransactionScopeCreating(_logger);

            var result = new TransactionScopeWrapper(_logger, new TransactionOptions()
            {
                IsolationLevel = isolationLevel ?? IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromSeconds(30)
            });

            TransactionsLogMessages.TransactionScopeCreated(_logger);
            return result;
        }

        private sealed class TransactionScopeWrapper
            : ITransactionScope
        {
            public TransactionScopeWrapper(
                ILogger logger,
                TransactionOptions options)
            {
                _logger = logger;
                _options = options;
                _scope = new TransactionScope(
                    scopeOption: TransactionScopeOption.Required,
                    transactionOptions: options,
                    asyncFlowOption: TransactionScopeAsyncFlowOption.Enabled);
            }

            public TransactionOptions Options
                => _options;

            public void Complete()
            {
                TransactionsLogMessages.TransactionScopeCommitting(_logger);
                _scope.Complete();
                TransactionsLogMessages.TransactionScopeCommitted(_logger);
            }

            public void Dispose()
            {
                TransactionsLogMessages.TransactionScopeDisposing(_logger);
                _scope.Dispose();
                TransactionsLogMessages.TransactionScopeDisposed(_logger);
            }

            private readonly ILogger _logger;
            private readonly TransactionOptions _options;
            private readonly TransactionScope _scope;
        }

        private readonly ILogger _logger;
    }
}
