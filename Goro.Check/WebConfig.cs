using System;
using System.Collections.Generic;
using System.Text;

namespace Goro.Check
{
    public class WebConfig
    {
        public static string AppBasePath = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// 本网站域名
        /// </summary>
        public static string WebHost = string.Empty;

        /// <summary>
        /// 链接字符串
        /// </summary>
        public static string ConnectionString = string.Empty;


        /// 微信Appid
        /// </summary>
        public static string APPID = string.Empty;

        /// <summary>
        /// 公众帐号secert（仅JSAPI支付的时候需要配置）
        /// </summary>
        public static string APPSECRET = string.Empty;

        /// <summary>
        /// 商户id 1499014362
        /// </summary>
        public static string MCHID = string.Empty;


        /// <summary>
        /// 商户支付密钥，参考开户邮件设置（必须配置）
        /// </summary>
        public static string KEY = string.Empty;
    }
}
