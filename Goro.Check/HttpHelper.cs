using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Goro.Check
{
    public abstract class HttpHelper
    {


        private readonly static HttpClient client = new HttpClient();


        /// <summary>
        /// 发起异步POST请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="forms">请求参数</param>
        /// <returns></returns>
        public static async Task<string> PostAsync(string url, Dictionary<string, string> forms)
        {
            HttpContent content = new FormUrlEncodedContent(forms);
            var res = await client.PostAsync(url, content);
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// 发起异步POST请求，并将返回数据反序列化为指定对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="forms"></param>
        /// <returns></returns>
        public static async Task<T> PostAsync<T>(string url, Dictionary<string, string> forms)
        {
            var json = await PostAsync(url, forms);
            //LoggerHelper.Info($"响应内容：{json}");
            return JsonHelper.Deserialize<T>(json);
        }


        /// <summary>
        /// 发起异步Put请求，并将返回数据反序列化为指定对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="forms"></param>
        /// <returns></returns>
        public static async Task<T> PutAsync<T>(string url, Dictionary<string, string> forms)
        {
            var json = await PutAsync(url, forms);
            return JsonHelper.Deserialize<T>(json);
        }
        /// <summary>
        /// 发起异步Put请求，返回字符串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="forms"></param>
        /// <returns></returns>
        public static async Task<string> PutAsync(string url, Dictionary<string, string> forms)
        {
            HttpContent content = new FormUrlEncodedContent(forms);
            var res = await client.PutAsync(url, content);
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            return json;
        }

        /// <summary>
        /// 发起异步Delete请求，并将返回数据反序列化为指定对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="forms"></param>
        /// <returns></returns>
        public static async Task<T> DeleteAsync<T>(string url)
        {
            var json = await DeleteAsync(url);
            return JsonHelper.Deserialize<T>(json);
        }
        /// <summary>
        /// 发起异步Delete请求，返回字符串
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> DeleteAsync(string url)
        {
            var res = await client.DeleteAsync(url);
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            return json;
        }

        /// <summary>
        /// 发起异步GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> GetAsync(string url)
        {
            var res = await client.GetAsync(url);
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();

            // LoggerHelper.Debug($"GET请求：{url},返回：{json}");
            return json;
        }

        /// <summary>
        /// 发起异步GET请求，并将返回数据反序列化为指定对象
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(string url)
        {
            var json = await GetAsync(url);
            return JsonHelper.Deserialize<T>(json);
        }
    }
}
