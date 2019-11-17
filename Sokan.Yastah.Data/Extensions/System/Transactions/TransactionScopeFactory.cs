using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace System.Transactions
{
    public interface ITransactionScopeFactory
    {
        ITransactionScope CreateScope(
            IsolationLevel? isolationLevel = default);
    }

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

        private class TransactionScopeWrapper
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

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
            => services.AddSingleton<ITransactionScopeFactory, TransactionScopeFactory>();
    }
}
