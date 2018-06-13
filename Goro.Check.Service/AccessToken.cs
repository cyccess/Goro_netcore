using System;
using System.Collections.Generic;
using System.Text;

namespace Goro.Check.Service
{
    public class AccessToken
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }
}
