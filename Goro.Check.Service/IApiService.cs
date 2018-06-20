using Goro.Check.Service.Model;
using System;
using System.Collections.Generic;
using System.Data;
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
        /// <param name="page"></param>
        /// <returns></returns>
        List<SalesReturnNotice> GetSalesReturnNotice(string phoneNumber, int page);

        SalesReturnNoticeDetail GetSalesReturnNoticeDetail(string phoneNumber, string fBillNo);

        string UpdateSalesReturn(string userGroupNumber, string phoneNumber, string billNo, string result, string reason);

        List<SalesOrder> GetSalesOrderList(string phoneNumber, int page);

        SalesOrderDetail GetSalesOrderDetail(string phoneNumber, string fBillNo);

        string UpdateSalesOrder(SalesOrderViewModel model);
    }
}
