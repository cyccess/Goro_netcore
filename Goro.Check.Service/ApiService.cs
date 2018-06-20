using Goro.Check.Data;
using Goro.Check.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Data;
using Goro.Check.Service.Model;

namespace Goro.Check.Service
{
    public class ApiService : IApiService
    {
        protected IRepository Repository { private set; get; }

        public ApiService(IRepository repository)
        {
            Repository = repository;
        }

        public async Task GetUserInfo(string phoneNumber, int groupId)
        {
            var userInfo = await Repository.Db.UserInfo
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.FPhoneNumber == phoneNumber && u.FUserGroupID == groupId);
        }

        /// <summary>
        /// 退货通知单
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<SalesReturnNotice> GetSalesReturnNotice(string phoneNumber, int page)
        {
            SqlParameter[] sqlParameter = new SqlParameter[]
            {
                new SqlParameter{ ParameterName = "@PhoneNumber", Value = phoneNumber, SqlDbType = SqlDbType.NVarChar }
            };

            //退货通知单-根据手机号获取单号和客户
            var dt = SqlHelper.ExecuteDataTable(CommandType.StoredProcedure, "tm_p_GetSalesReturnNotice", sqlParameter);

            int count = (page - 1) * 20;
            var salesReturnNotices = dt.AsEnumerable().Select(r => new SalesReturnNotice
            {
                FBillNo = r[0].ToString(),
                FCustName = r[1].ToString()
            }).Skip(count).Take(20).ToList();

            int sn = count;
            foreach (var item in salesReturnNotices)
            {
                item.Id = sn += 1;
            }

            return salesReturnNotices;
        }

        /// <summary>
        /// 退货通知单详情
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="groupId"></param>
        /// <param name="fBillNo"></param>
        /// <returns></returns>
        public SalesReturnNoticeDetail GetSalesReturnNoticeDetail(string phoneNumber, string fBillNo)
        {
            SalesReturnNoticeDetail returnNoticeDetail = new SalesReturnNoticeDetail();

            var fields = GetUserGroupFieldDisplayed(phoneNumber, "002");
            returnNoticeDetail.Field = fields;

            var field = string.Join(",", fields.Select(f => f.FFieldName).ToArray());

            SqlParameter[] sqlParameter = new SqlParameter[]
            {
              new SqlParameter{ ParameterName = "@FBillNo", Value = fBillNo, SqlDbType = SqlDbType.NVarChar }
            };

            string sql = $"select {field} from tm_v_SalesReturnNotice where FBillNo=@FBillNo";
            var res = SqlHelper.ExecuteDataTable(CommandType.Text, sql, sqlParameter);

            returnNoticeDetail.Order = res;

            return returnNoticeDetail;
        }

        /// <summary>
        /// 根据手机号获得可查看字段
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="billTypeNumber">单据类型，001：销售订单；002：销售退货通知单</param>
        /// <returns></returns>
        public List<FieldDisplayed> GetUserGroupFieldDisplayed(string phoneNumber, string billTypeNumber)
        {
            SqlParameter[] sqlParameter = new SqlParameter[]
            {
              new SqlParameter{ ParameterName = "@PhoneNumber", Value = phoneNumber, SqlDbType = SqlDbType.NVarChar },
              new SqlParameter{ ParameterName = "@BillTypeNumber", Value = billTypeNumber, SqlDbType = SqlDbType.NVarChar },
            };

            var dt = SqlHelper.ExecuteDataTable(CommandType.StoredProcedure, "tm_p_GetFieldDisplayed", sqlParameter);
            var fields = dt.AsEnumerable().Select(r => new FieldDisplayed
            {
                FFieldName = GetFirstLowerStr(r["FFieldName"].ToString()),
                FFieldDescription = r["FFieldDescription"].ToString()
            }).ToList();

            return fields;
        }

        /// <summary>
        /// 首字母转小写
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string GetFirstLowerStr(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                if (s.Length > 1)
                {
                    return char.ToLower(s[0]) + s.Substring(1);
                }
                return char.ToLower(s[0]).ToString();
            }
            return null;
        }

        /// <summary>
        /// 退货通知单审核
        /// </summary>
        /// <param name="userGroupNumber"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="billNo"></param>
        /// <param name="result"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public string UpdateSalesReturn(string userGroupNumber, string phoneNumber, string billNo, string result, string reason)
        {
            SqlParameter[] sqlParameter = new SqlParameter[]
            {
                new SqlParameter("@PhoneNumber",phoneNumber),
                new SqlParameter("@BillNo",billNo),
                new SqlParameter("@Result",result),
                new SqlParameter("@Reason",reason),
                new SqlParameter{ ParameterName = "@Msg", Value="", Direction = ParameterDirection.Output, Size=100, SqlDbType = SqlDbType.NVarChar }
            };

            string cmdText = "";
            if (userGroupNumber == "001") //销售总监组审核更新
            {
                cmdText = "tm_p_UpdateSalesReturnCSO";
            }
            if (userGroupNumber == "009") //质检组审核更新
            {
                cmdText = "tm_p_UpdateSalesReturnQC";
            }

            var res = SqlHelper.ExecuteNonQuery(CommandType.StoredProcedure, cmdText, sqlParameter);
            var msg = sqlParameter[4].Value;

            return msg.ToString();
        }


        /// <summary>
        /// 销售订单-根据手机号获取单号、客户、类型
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="page"></param>
        public List<SalesOrder> GetSalesOrderList(string phoneNumber, int page)
        {
            SqlParameter[] sqlParameter = new SqlParameter[]
            {
                new SqlParameter{ ParameterName = "@PhoneNumber", Value = phoneNumber, SqlDbType = SqlDbType.NVarChar }
            };

            //退货通知单-根据手机号获取单号和客户
            var dt = SqlHelper.ExecuteDataTable(CommandType.StoredProcedure, "tm_p_GetSalesOrderList", sqlParameter);

            int count = (page - 1) * 20;
            var salesOrders = dt.AsEnumerable().Select(r => new SalesOrder
            {
                FBillNo = r[0].ToString(),
                FCustName = r[1].ToString(),
                FType = r[2].ToString(),
            }).Skip(count).Take(20).ToList();

            int sn = count;
            foreach (var item in salesOrders)
            {
                item.Id = sn += 1;
            }

            return salesOrders;
        }

        public SalesOrderDetail GetSalesOrderDetail(string phoneNumber, string fBillNo)
        {
            SalesOrderDetail salesOrder = new SalesOrderDetail();
            var fields = GetUserGroupFieldDisplayed(phoneNumber, "001");
            salesOrder.Field = fields;
            var field = string.Join(",", fields.Select(f => f.FFieldName).ToArray());

            SqlParameter[] sqlParameter = new SqlParameter[]
            {
              new SqlParameter("@FBillNo",fBillNo)
            };

            string sql = $"select {field} from tm_v_SeOrderList where FBillNo=@FBillNo";
            var res = SqlHelper.ExecuteDataTable(CommandType.Text, sql, sqlParameter);
            salesOrder.Order = res;

            return salesOrder;
        }

        /// <summary>
        /// 销售单审核
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string UpdateSalesOrder(SalesOrderViewModel model)
        {
            List<SqlParameter> sqlParameter = new List<SqlParameter>()
            {
                new SqlParameter("@PhoneNumber",model.phoneNumber),
                new SqlParameter("@BillNo",model.billNo),
                new SqlParameter("@Result",model.result),
                new SqlParameter("@Reason",model.reason)
            };

            string cmdText = "";
            if (model.userGroupNumber == "002") //总经理审核更新
            {
                cmdText = "tm_p_UpdateSalesOrderGM";
            }

            if (model.userGroupNumber == "004")
            {
                if (model.result == "Y") //生产确认通过更新
                {
                    cmdText = "tm_p_UpdateSalesOrderPDC";
                }

                if (model.result == "N") //生产不通过更新
                {
                    cmdText = "tm_p_UpdateSalesOrderPDD";
                    sqlParameter.Add(new SqlParameter("@ISME", model.isMe));
                    sqlParameter.Add(new SqlParameter("@ISPO", model.isPo));
                }
            }

            if(model.userGroupNumber == "005") //工艺回复更新
            {
                cmdText = "tm_p_UpdateSalesOrderME";
            }

            if (model.userGroupNumber == "006") //供应回复更新
            {
                cmdText = "tm_p_UpdateSalesOrderPO";
            }

            sqlParameter.Add(new SqlParameter { ParameterName = "@Msg", Value = "", Direction = ParameterDirection.Output, Size = 100, SqlDbType = SqlDbType.NVarChar });

            var res = SqlHelper.ExecuteNonQuery(CommandType.StoredProcedure, cmdText, sqlParameter.ToArray());
            var msg = sqlParameter.Last().Value;

            return msg.ToString();
        }
    }
}
