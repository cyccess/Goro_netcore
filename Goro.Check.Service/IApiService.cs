using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Goro.Check.Service
{
    public interface IApiService
    {
        Task GetUserInfo(string phone, int groupId);


        /// <summary>
        /// 退货通知单- 根据手机号获取单号和客户
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        Task<string[]> GetSalesReturnNotice(string phoneNumber);
    }
}
