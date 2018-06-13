using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Goro.Check.Cache
{
    public class MemoryCacheService : ICacheService
    {
        protected IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string key) where T : class, new()
        {
            return _cache.Get(key) as T;
        }

        public string Get(string key)
        {
            return _cache.Get(key).ToString();
        }

        public void Remove(string key)
        {
             _cache.Remove(key);
        }

        public void RemoveAll()
        {
            throw new NotImplementedException();
        }

        public void Set(string key, object value, TimeSpan expirTimeSpan)
        {
            _cache.Set(key, value, expirTimeSpan);
        }
    }
}
