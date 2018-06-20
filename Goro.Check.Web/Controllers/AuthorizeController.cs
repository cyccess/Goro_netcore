using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Goro.Check.Service;
using Microsoft.AspNetCore.Mvc;

namespace Goro.Check.Web.Controllers
{
    public class AuthorizeController : Controller
    {
        public IActionResult Index()
        {
            string reurl = WebUtility.UrlEncode($"{WebConfig.WebHost}/Authorize/WxRedirect");
            string url = "https://" + $"open.weixin.qq.com/connect/oauth2/authorize?appid={WebConfig.APPID}&redirect_uri={reurl}&response_type=code&scope=snsapi_base&state=STATE#wechat_redirect";
            return Redirect(url);
        }


        public async Task<IActionResult> WxRedirect(string code)
        {
            LoggerHelper.Info("微信登录code:" + code);

            string url = "https://" + $"api.weixin.qq.com/sns/oauth2/access_token?appid={WebConfig.APPID}&secret={WebConfig.APPSECRET}&code={code}&grant_type=authorization_code";

            var token = await HttpHelper.GetAsync<WechatToken>(url);

            LoggerHelper.Info("json:" + token.openid);
            Cache.CacheService.Set(token.openid, token);

            string redirectUrl = "/#/?openid=" + token.openid;
          
            return Redirect(redirectUrl);
        }
    }
}