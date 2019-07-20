using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                new TransactionScope(
                    scopeOption: TransactionScopeOption.Required,
                    transactionOptions: new TransactionOptions()
                    {
                        IsolationLevel = isolationLevel ?? IsolationLevel.ReadCommitted,
                        Timeout = TimeSpan.FromSeconds(30)
                    },
                    asyncFlowOption: TransactionScopeAsyncFlowOption.Enabled));

        private class TransactionScopeWrapper
            : ITransactionScope
        {
            public TransactionScopeWrapper(TransactionScope scope)
            {
                _scope = scope;
            }

            public void Complete()
                => _scope.Complete();

            public void Dispose()
                => _scope.Dispose();

            private readonly TransactionScope _scope;
        }

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services.AddSingleton<ITransactionScopeFactory, TransactionScopeFactory>();
    }
}
