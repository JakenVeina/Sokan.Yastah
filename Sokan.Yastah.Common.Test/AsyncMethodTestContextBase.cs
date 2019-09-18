using System;
using System.Threading;

namespace Sokan.Yastah.Common.Test
{
    #pragma warning disable CA1063 // IDisposable warnings
    public class AsyncMethodTestContextBase
        : IDisposable
    {
        protected AsyncMethodTestContextBase() { }

        public CancellationTokenSource CancellationTokenSource { get; }
            = new CancellationTokenSource();

        public CancellationToken CancellationToken
            => CancellationTokenSource.Token;

        public void Dispose()
            => CancellationTokenSource.Dispose();
    }
    #pragma warning restore CA1063
}
