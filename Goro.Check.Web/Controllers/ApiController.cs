using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goro.Check.Service;
using Goro.Check.Service.Model;
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

        public async Task<IActionResult> SalesReturnNotice(int page = 1)
        {
            await apiService.GetUserInfo("13011111111", 15830);

            var res = apiService.GetSalesReturnNotice("13011111111", page);

            var model = new ReturnModel();

            model.Code = 100;
            model.Data = res;

            return Json(model);
        }

        public IActionResult SalesReturnNoticeDetail(string phoneNumber, string fBillNo)
        {
            var res = apiService.GetSalesReturnNoticeDetail("13011111111", fBillNo);

            var model = new ReturnModel();

            model.Code = 100;
            model.Data = res;

            return Json(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userGroupNumber"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="billNo"></param>
        /// <param name="result">审核结果 Y/N</param>
        /// <param name="reason">未通过原因</param>
        /// <returns></returns>
        public IActionResult UpdateSalesReturn(string userGroupNumber, string phoneNumber, string billNo, string result, string reason)
        {
            reason = reason ?? "";
            var res = apiService.UpdateSalesReturn("001", "13022222222", billNo, result, reason);

            var model = new ReturnModel();

            model.Code = 100;
            model.Message = res;

            return Json(model);
        }



        public IActionResult SalesOrders(int page = 1)
        {
            var res = apiService.GetSalesOrderList("13044444444", page);

            var model = new ReturnModel();

            model.Code = 100;
            model.Data = res;

            return Json(model);
        }


        public IActionResult SalesOrderDetail(string phoneNumber, string billTypeNumber, string fBillNo)
        {
            var res = apiService.GetSalesOrderDetail("13033333333", fBillNo);

            var model = new ReturnModel();

            model.Code = 100;
            model.Data = res;

            return Json(model);
        }


        public IActionResult UpdateSalesOrder(SalesOrderViewModel model)
        {
            model.reason = model.reason ?? "";
            var res = apiService.UpdateSalesOrder(model);

            var data = new ReturnModel();

            data.Code = 100;
            data.Message = res;

            return Json(data);
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
