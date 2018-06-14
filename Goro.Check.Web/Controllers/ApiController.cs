using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goro.Check.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Goro.Check.Web.Controllers
{
    [Produces("application/json")]

    public class ApiController : Controller
    {
        private IApiService apiService;

        public ApiController(IApiService apiService)
        {
            this.apiService = apiService;
        }

        public async Task<IActionResult> Test()
        {
            await apiService.GetUserInfo("13011111111", 15830);

            var res = apiService.GetSalesReturnNotice("13011111111");

            return Json(res);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Authorize()
        {
            string reurl = System.Net.WebUtility.UrlEncode($"{WebConfig.WebHost}/api/GetToken/");

            string url = "https://" + $"open.weixin.qq.com/connect/oauth2/authorize?appid={WebConfig.APPID}&redirect_uri={reurl}&response_type=code&scope=snsapi_base&state=STATE#wechat_redirect";

            return Redirect(url);
        }

        public async Task<IActionResult> GetToken(string code)
        {
            LoggerHelper.Info("微信登录code:" + code);

            string url = "https://" + $"api.weixin.qq.com/sns/oauth2/access_token?appid={WebConfig.APPID}&secret={WebConfig.APPSECRET}&code={code}&grant_type=authorization_code";

            var token = await HttpHelper.GetAsync<WechatToken>(url);

            LoggerHelper.Info("json:" + token.openid);
            Cache.CacheService.Set(token.openid, token);

            string redirectUrl = "/#/Account?openid=" + token.openid;

            return Redirect(redirectUrl);
        }
    }
}
