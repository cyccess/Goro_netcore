
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Goro.Check.Cache
{
    /// <summary>
    /// 缓存服务
    /// </summary>
    public class CacheService
    {

        //static ILogger log = null;
        static object _locker = new object();

        public static void Init(IMemoryCache cache)
        {
            lock (_locker)
            {
                CurrentService = new MemoryCacheService(cache);
            }
        }

        /// <summary>
        /// 当前实例化的缓存服务
        /// </summary>
        public static ICacheService CurrentService
        {
            get;
            set;
        }

        /// <summary>
        /// 自定义CacheService
        /// </summary>
        /// <param name="service"></param>
        private static void SetCacheService(ICacheService service)
        {
            CurrentService = service;
        }



        /// <summary>
        /// 获取缓存的项
        /// </summary>
        /// <typeparam name="T">缓存的数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public static T Get<T>(string key) where T : class, new()
        {
            lock (_locker)
            {
                return CurrentService.Get<T>(key);
            }

        }



        /// <summary>
        /// 获取缓存的项
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public static string Get(string key)
        {
            lock (_locker)
            {
                return CurrentService.Get(key);
            }
        }

        /// <summary>
        /// 删除缓存项
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public static void Remove(string key)
        {
            lock (_locker)
            {
                CurrentService.Remove(key);
            }
        }

        /// <summary>
        /// 设置缓存项,默认为1天
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="value">缓存对象</param>
        /// <returns></returns>
        public static void Set(string key, object value)
        {
            Set(key, value, new TimeSpan(1, 0, 0, 0));
        }

        /// <summary>
        /// 设置缓存项
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="value">缓存对象</param>
        /// <param name="expirTime">过期时间</param>
        /// <returns></returns>
        public static void Set(string key, object value, DateTime expirTime)
        {
            TimeSpan sp = expirTime - DateTime.Now;
            Set(key, value, sp);
        }

        /// <summary>
        /// 设置缓存项
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="value">缓存对象</param>
        /// <param name="expirTimeSpan">过期时间（时间间隔）</param>
        /// <returns></returns>
        public static void Set(string key, object value, TimeSpan expirTimeSpan)
        {
            lock (_locker)
            {
                CurrentService.Set(key, value, expirTimeSpan);
            }
        }

        /// <summary>
        /// 清除所有缓存项
        /// </summary>
        public static void RemoveAll()
        {
            lock (_locker)
            {
                CurrentService.RemoveAll();
            }
        }
    }
}
