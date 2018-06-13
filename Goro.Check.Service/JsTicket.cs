using System;
using System.Collections.Generic;
using System.Text;

namespace Goro.Check.Service
{
    public class JsTicket
    {

        public string errcode { get; set; }
        public string errmsg  { get; set; }
        public string ticket { get; set; }
        public int expires_in { get; set; }

    }
}
