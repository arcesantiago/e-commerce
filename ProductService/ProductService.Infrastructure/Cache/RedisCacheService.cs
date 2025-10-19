using Microsoft.Extensions.Caching.Distributed;
using ProductService.Application.Contracts.Infrastructure;
using System.Text.Json;

namespace ProductService.Infrastructure.Cache
{
    public class RedisCacheService(IDistributedCache distributedCache) : ICacheService
    {
        private readonly IDistributedCache _distributedCache = distributedCache;
        public async Task<T?> GetAsync<T>(string key)
        {
            var cached = await _distributedCache.GetStringAsync(key);
            return cached is null ? default : JsonSerializer.Deserialize<T>(cached);
        }
        public async Task SetAsync<T>(string key, T value, TimeSpan ttl)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            };

            var serialized = JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(key, serialized, options);
        }
        public Task RemoveAsync(string key)
        {
            return _distributedCache.RemoveAsync(key);
        }
    }
}
