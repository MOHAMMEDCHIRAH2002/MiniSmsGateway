using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSmsGateway
{
    internal class MockSmsApiClient : ISmsApiClient
    {
        public Task<List<string>> SendSmsAsync(SmsRequest request)
        {
            var results = new List<string>();

            foreach (var phone in request.PhoneNumbers)
            {
                // Simuler success أو failure حسب الرقم
                if (phone.EndsWith("0"))
                    results.Add("Failed");
                else
                    results.Add("Success");
            }
            return Task.FromResult(results);

        }
    }
}
