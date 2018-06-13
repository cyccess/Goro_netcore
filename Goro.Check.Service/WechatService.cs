using Goro.Check;
using Goro.Check.Cache;
using Goro.Check.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Psb.Service
{
    public static class WechatService
    {
        /// <summary> 
        /// 将c# DateTime时间格式转换为Unix时间戳格式 
        /// </summary> 
        /// <param name="time">时间</param> 
        /// <returns>long</returns> 
        public static long ConvertDateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位     
            return t;
        }

        public static async Task<string> GetAccessToken()
        {
            var token = CacheService.Get("AccessToken");
            if (string.IsNullOrEmpty(token))
            {
                string url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={WebConfig.APPID}&secret={WebConfig.APPSECRET}";

                var accessToken = await HttpHelper.GetAsync<AccessToken>(url);
                token = accessToken.access_token;
                CacheService.Set("AccessToken", token, new TimeSpan(0, 30, 0));
                LoggerHelper.Info($"重新获取AccessToken={token}");
            }

            return token;
        }


        public static async Task<string> GetUserInfo(string openid)
        {
            var key = "userinfo_" + openid;
            var info = CacheService.Get(key);
            if (info == null)
            {
                var token = await GetAccessToken();
                var url = $"https://api.weixin.qq.com/cgi-bin/user/info?access_token={token}&openid={openid}";

                info = await HttpHelper.GetAsync(url);
                info = info.Replace("\\", "");
                CacheService.Set(key, info, new TimeSpan(0, 10, 0));

                LoggerHelper.Info($"重新获取用户信息：openid={openid},userinfo={info}");

            }
            return info;
        }

        public static async Task<WechatConfig> GetWechatConfig()
        {
            var config = CacheService.Get<WechatConfig>("WxConfig");
            if (config == null)
            {
                var token = await GetAccessToken();
                var url = $"https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={token}&type=jsapi";

                config = new WechatConfig();
                config.webHost = WebConfig.WebHost;
                config.appId = WebConfig.APPID;
                config.nonceStr = "wechart";
                config.timestamp = ConvertDateTimeToInt(DateTime.Now).ToString();
                var jsUrl = $"{WebConfig.WebHost}/";
                var tiket = await HttpHelper.GetAsync<JsTicket>(url);
                var str = $"jsapi_ticket={tiket.ticket}&noncestr={config.nonceStr}&timestamp={config.timestamp}&url={jsUrl}";
                config.signature = HmacSha1Sign(str);
                CacheService.Set("WxConfig", config, new TimeSpan(0, 100, 0));

                LoggerHelper.Info($"重新获取WxConfig={JsonHelper.Serialize(config)}");
            }
            return config;
        }

        /// <summary>
        /// HMAC-SHA1加密算法
        /// </summary>
        /// <param name="str">加密字符串</param>
        /// <returns></returns>
        public static string HmacSha1Sign(string str)
        {
            var sha1 = System.Security.Cryptography.SHA1.Create();
            var hash = sha1.ComputeHash(Encoding.Default.GetBytes(str));
            string byte2String = null;
            for (int i = 0; i < hash.Length; i++)
            {
                byte2String += hash[i].ToString("x2");
            }
            return byte2String;
        }
    }
}
