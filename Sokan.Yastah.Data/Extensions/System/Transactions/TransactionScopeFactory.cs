using Microsoft.Extensions.DependencyInjection;

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
        public ITransactionScope CreateScope(
                IsolationLevel? isolationLevel = default)
            => new TransactionScopeWrapper(
                new TransactionOptions()
                {
                    IsolationLevel = isolationLevel ?? IsolationLevel.ReadCommitted,
                    Timeout = TimeSpan.FromSeconds(30)
                });

        private sealed class TransactionScopeWrapper
            : ITransactionScope
        {
            public TransactionScopeWrapper(
                TransactionOptions options)
            {
                _options = options;
                _scope = new TransactionScope(
                    scopeOption: TransactionScopeOption.Required,
                    transactionOptions: options,
                    asyncFlowOption: TransactionScopeAsyncFlowOption.Enabled);
            }

            public TransactionOptions Options
                => _options;

            public void Complete()
                => _scope.Complete();

            public void Dispose()
                => _scope.Dispose();

            private readonly TransactionOptions _options;
            private readonly TransactionScope _scope;
        }
    }
}
