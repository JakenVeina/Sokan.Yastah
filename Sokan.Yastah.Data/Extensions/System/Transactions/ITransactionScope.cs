namespace System.Transactions
{
    public interface ITransactionScope
        : IDisposable
    {
        TransactionOptions Options { get; }

        void Complete();
    }
}
