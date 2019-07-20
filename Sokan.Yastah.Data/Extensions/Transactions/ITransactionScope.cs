namespace System.Transactions
{
    public interface ITransactionScope
        : IDisposable
    {
        void Complete();
    }
}
