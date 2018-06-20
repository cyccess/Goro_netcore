using System;
using System.Collections.Generic;
using System.Text;

namespace Goro.Check.Service.Model
{
    public class SalesOrderViewModel
    {
        public string userGroupNumber { get; set; }

        public string phoneNumber { get; set; }

        public string billNo { get; set; }

        public string result { get; set; }

        public string reason { get; set; }


        /// <summary>
        /// 工艺是否审核
        /// </summary>
        public string isMe { get; set; }

        /// <summary>
        /// 供应是否审核
        /// </summary>
        public string isPo { get; set; }
    }
}
