using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSmsGateway
{
    public class SmsConfig
    {
        public string SMSApiUrl { get; set; } = "";
        public string ApiKey { get; set; } = "";
        
        public int HttpPort { get; set; } 

        public int RetryCount { get; set; } = 3;

        public int TimeoutMs { get; set; } = 3000;
        public bool MockEnabled { get; set; } = true;


    }
}
