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
            var userInfo = await Repository.Db.UserInfo.SingleOrDefaultAsync(u => u.FPhoneNumber == phoneNumber && u.FUserGroupID == groupId);

        }

        public Task<string[]> GetSalesReturnNotice(string phoneNumber)
        {
            SqlParameter[] sqlParameter = new SqlParameter[]
            {
                new SqlParameter{ ParameterName = "@PhoneNumber", Value = phoneNumber, SqlDbType = SqlDbType.NVarChar }
            };

            var dt = SqlHelper.ExecuteDataTable(CommandType.StoredProcedure, "tm_p_GetSalesReturnNotice", sqlParameter);

            var s = dt.AsEnumerable().Select(r => r[0].ToString()).ToArray();

            SqlParameter[] sqlParameter1 = new SqlParameter[s.Length];

            string fBillNo = "";

            for (int i = 0; i < s.Length; i++)
            {
                sqlParameter1[i] = new SqlParameter { ParameterName = "@FBillNo" + i, Value = s[i], SqlDbType = SqlDbType.NVarChar };
                fBillNo += "@FBillNo" + i + ",";
            }

            fBillNo = fBillNo.Substring(0, fBillNo.Length - 1);

            string sql = $"select * from tm_v_SalesReturnNotice where FBillNo in({fBillNo}) ";
            var dt1 = SqlHelper.ExecuteDataTable(CommandType.Text, sql, sqlParameter1);

            throw new NotImplementedException();
        }
    }
}
