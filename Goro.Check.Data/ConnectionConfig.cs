using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Goro.Check.Data
{
    /// <summary>
    /// 数据库连接配置
    /// </summary>
    internal class ConnectionConfig
    {
        /// <summary>
        /// 当前连接字符串
        /// </summary>
        public static string CurrentConnection
        {
            get
            {
                return WebConfig.ConnectionString;
            }
        }
    }
}
