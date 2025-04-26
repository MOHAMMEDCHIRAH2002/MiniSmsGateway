using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSmsGateway
{
    public class SmsRequest
    {
        public List<string> PhoneNumbers { get; set; } = new();
        public string Message { get; set; } = "";
        public string Reference { get; set; } = "";
        public string Severity { get; set; } = "";

        //public SmsRequest(List<string> phoneNumbers ,string message,string reference ,string severity)
        //{
        //    PhoneNumbers = phoneNumbers;
        //    Message = message;
        //    Reference = reference;
        //    Severity = severity;
        //}
        
            
     

    }
}
