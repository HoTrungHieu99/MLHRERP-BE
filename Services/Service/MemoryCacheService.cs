using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Services.IService;

namespace Services.Service
{
    

    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(_cache.TryGetValue(key, out _));
        }

        public Task SetAsync(string key, object value, TimeSpan? expiry = null)
        {
            _cache.Set(key, value, expiry ?? TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }
    }

}
