using Kemora.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Kemora.Infrastructure.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T? Get<T>(string key)
        {
            _memoryCache.TryGetValue(key, out T? value);
            return value;
        }

        public void Set<T>(string key, T value, TimeSpan? absoluteExpireTime = null)
        {
            var options = new MemoryCacheEntryOptions();

            if (absoluteExpireTime.HasValue)
            {
                options.SetAbsoluteExpiration(absoluteExpireTime.Value);
            }
            else
            {
                // Default expiration
                options.SetSlidingExpiration(TimeSpan.FromMinutes(10));
            }

            _memoryCache.Set(key, value, options);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
