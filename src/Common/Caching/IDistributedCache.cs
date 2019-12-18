using System;

using Microsoft.Extensions.Caching.Distributed;

namespace DataVault.Common.Caching
{
    public interface IDistributedCache<TCacheItem> where TCacheItem : class
    {
        TCacheItem Get(string key);

        TCacheItem GetOrAdd(
            string key,
            Func<TCacheItem> factory,
            Func<DistributedCacheEntryOptions> optionsFactory = null
        );

        void Set(
            string key,
            TCacheItem value,
            DistributedCacheEntryOptions options = null
        );

        void Refresh(string key);

        void Remove(string key);
    }
}