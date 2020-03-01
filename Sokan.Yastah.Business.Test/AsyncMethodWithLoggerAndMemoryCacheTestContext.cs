using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using Sokan.Yastah.Common.Test;

namespace Sokan.Yastah.Business
{
    public class AsyncMethodWithLoggerFactoryAndMemoryCacheTestContext
        : AsyncMethodWithLoggerTestContext
    {
        public readonly MemoryCache MemoryCache
            = new MemoryCache(Options.Create(new MemoryCacheOptions()));

        protected override void Dispose(
            bool disposeManaged)
        {
            if (disposeManaged)
                MemoryCache.Dispose();
            base.Dispose(disposeManaged);
        }
    }
}
