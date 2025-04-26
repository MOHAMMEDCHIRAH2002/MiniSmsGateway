using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiniSmsGateway
{
    public class SmsLogger
    {

        private readonly string _logFilePath = "sms-events.json";

        public async Task LogSmsAsync(SmsRequest request, List<string> statuses)
        {
            var logEntry = new
            {
                Timestamp = DateTime.UtcNow,
                Reference = request.Reference,
                PhoneNumbers = request.PhoneNumbers,
                Statuses = statuses,
                Severity = request.Severity,
                Message = request.Message
            };

            var json = JsonSerializer.Serialize(logEntry);

            await File.AppendAllTextAsync(_logFilePath, json + Environment.NewLine);
        }

    }
}
