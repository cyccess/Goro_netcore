using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Goro.Check.Cache
{
    /// <summary>
    /// 定义缓存服务的接口
    /// </summary>
    public interface ICacheService
    {
        T Get<T>(string key) where T : class, new();

        string Get(string key);

        void Remove(string key);

        void Set(string key, object value, TimeSpan expirTimeSpan);

        void RemoveAll();
    }
}
