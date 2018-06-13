using System;
using System.Collections.Generic;
using System.Text;

namespace Goro.Check.Service
{
    public class WechatToken
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string openid { get; set; }
        public string scope { get; set; }
    }

    public class WechatConfig
    {
        public string appId { get; set; }
        public string timestamp { get; set; }
        public string nonceStr { get; set; }
        public string signature { get; set; }

        public string webHost { get; set; }
    }
}
