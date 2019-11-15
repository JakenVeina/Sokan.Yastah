using System;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.Memory
{
    public static class MemoryCacheExtensions
    {
        public static async ValueTask<TItem> OptimisticGetOrCreateAsync<TItem>(this IMemoryCache cache, object key, Func<ICacheEntry, Task<TItem>> factory)
        {
            if (cache.TryGetValue(key, out object result))
                return (TItem)result; 

            var entry = cache.CreateEntry(key);
            var value = await factory(entry);
            entry.Value = value;
            // need to manually call dispose instead of having a using
            // in case the factory method throws, in which case we
            // do not want to add the entry to the cache
            entry.Dispose();

            return value;
        }
    }
}
