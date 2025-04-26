using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSmsGateway
{
    public interface ISmsApiClient
    {
        Task<List<string>> SendSmsAsync(SmsRequest request);
    }
}
