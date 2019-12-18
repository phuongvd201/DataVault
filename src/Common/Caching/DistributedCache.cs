using System;

using DataVault.Common.Extensions;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Nito.AsyncEx;

namespace DataVault.Common.Caching
{
    public class DistributedCache<TCacheItem> : IDistributedCache<TCacheItem>
        where TCacheItem : class
    {
        public ILogger<DistributedCache<TCacheItem>> Logger { get; set; }

        protected string CacheName { get; set; }

        protected IDistributedCache Cache { get; }

        protected IDistributedCacheSerializer Serializer { get; }

        protected AsyncLock AsyncLock { get; } = new AsyncLock();

        protected DistributedCacheEntryOptions DefaultCacheOptions;

        private readonly CacheOptions _cacheOption;

        public DistributedCache(
            IOptions<CacheOptions> cacheOption,
            IDistributedCache cache,
            IDistributedCacheSerializer serializer)
        {
            _cacheOption = cacheOption.Value;
            Cache = cache;
            Logger = NullLogger<DistributedCache<TCacheItem>>.Instance;
            Serializer = serializer;

            SetDefaultOptions();
        }

        public virtual TCacheItem Get(string key)
        {
            byte[] cachedBytes;

            try
            {
                cachedBytes = Cache.Get(NormalizeKey(key));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return null;
            }

            if (cachedBytes == null)
            {
                return null;
            }

            return Serializer.Deserialize<TCacheItem>(cachedBytes);
        }

        public TCacheItem GetOrAdd(
            string key,
            Func<TCacheItem> factory,
            Func<DistributedCacheEntryOptions> optionsFactory = null)
        {
            var value = Get(key);
            if (value != null)
            {
                return value;
            }

            using (AsyncLock.Lock())
            {
                value = Get(key);
                if (value != null)
                {
                    return value;
                }

                value = factory();
                Set(key, value, optionsFactory?.Invoke());
            }

            return value;
        }

        public virtual void Set(
            string key,
            TCacheItem value,
            DistributedCacheEntryOptions options = null)
        {
            try
            {
                Cache.Set(
                    NormalizeKey(key),
                    Serializer.Serialize(value),
                    options ?? DefaultCacheOptions
                );
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public virtual void Refresh(string key)
        {
            try
            {
                Cache.Refresh(NormalizeKey(key));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public virtual void Remove(string key)
        {
            try
            {
                Cache.Remove(NormalizeKey(key));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        protected virtual string NormalizeKey(string key)
        {
            var normalizedKey = "c:" + CacheName + ",k:" + key;

            return normalizedKey;
        }

        protected virtual DistributedCacheEntryOptions GetDefaultCacheEntryOptions()
        {
            foreach (var configure in _cacheOption.CacheConfigurators)
            {
                var options = configure.Invoke(CacheName);
                if (options != null)
                {
                    return options;
                }
            }

            return _cacheOption.GlobalCacheEntryOptions;
        }

        protected virtual void SetDefaultOptions()
        {
            //Configure default cache entry options
            DefaultCacheOptions = GetDefaultCacheEntryOptions();
        }
    }
}