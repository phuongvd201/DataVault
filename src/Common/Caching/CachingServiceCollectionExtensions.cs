using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DataVault.Common.Caching
{
    public static class CachingServiceCollectionExtensions
    {
        public static void AddCaching(this IServiceCollection services)
        {
            services.TryAddSingleton<IDistributedCacheSerializer, Utf8JsonDistributedCacheSerializer>();
            services.TryAddSingleton(typeof(IDistributedCache<>), typeof(DistributedCache<>));

            services.Configure<CacheOptions>(
                cacheOptions =>
                {
                    cacheOptions.GlobalCacheEntryOptions.SlidingExpiration = TimeSpan.FromMinutes(20);
                });
        }
    }
}