using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RedisCaching.Services
{
    public class CacheConfigService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly string _redisMasterKey;

        public CacheConfigService(IDistributedCache distributedCache, IConfiguration configuration)
        {
            _distributedCache = distributedCache;
            _redisMasterKey = configuration.GetSection("RedisMasterKey").Value;
        }

        private string GetCacheKey(string id)
        {
            return $"{_redisMasterKey}:{id}";
        }

        public async Task<JObject?> GetAsync(string id)
        {
            var cacheKey = GetCacheKey(id);
            var cachedData = await _distributedCache.GetStringAsync(cacheKey);

            if (string.IsNullOrEmpty(cachedData))
                return null;

            return JObject.Parse(cachedData);
        }


        public async Task SetAsync(string id, JObject data, TimeSpan? expiration = null)
        {
            var cacheKey = GetCacheKey(id);
            var serializedData = data.ToString(Formatting.None);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(1)
            };

            await _distributedCache.SetStringAsync(cacheKey, serializedData, options);
        }


        public async Task RemoveAsync(string id)
        {
            var cacheKey = GetCacheKey(id);
            await _distributedCache.RemoveAsync(cacheKey);
        }
    }
}
