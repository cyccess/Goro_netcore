using System;
using System.Collections.Generic;
using System.Text;

namespace Goro.Check
{
    /// <summary>
    /// 表示一个操作的返回实体模型 ,包括是否成功、返回信息、返回数据
    /// </summary>
    [Serializable]
    public class ReturnModel
    {
        public ReturnModel()
        {
            
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 返回的信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 返回的数据
        /// </summary>
        public object Data { get; set; }
    }
}
