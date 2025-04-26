using Microsoft.Extensions.Options;


namespace MiniSmsGateway
{
    public class MockSmsApiClient : ISmsApiClient
    {
        private readonly SmsConfig _config;
        private readonly ILogger<MockSmsApiClient> _logger;

        public MockSmsApiClient(IOptions<SmsConfig> config, ILogger<MockSmsApiClient> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        public async Task<List<string>> SendSmsAsync(SmsRequest request)
        {
            var results = new List<string>();

            foreach (var phone in request.PhoneNumbers)
            {
                bool sent = false;
                int attempts = 0;

                while (!sent && attempts < _config.RetryCount)
                {
                    attempts++;

                    // Simuler une tentative d'envoi
                    bool success = !phone.EndsWith("0"); // كل رقم كيسالي ب 0 غادي نفشلو فيه

                    if (success)
                    {
                        results.Add("Success");
                        sent = true;
                    }
                    else
                    {
                        if (attempts >= _config.RetryCount)
                        {
                            results.Add("Failed");
                            _logger.LogWarning($"⚠️ Sending to {phone} failed after {attempts} attempts.");
                        }
                        else
                        {
                            _logger.LogWarning($"Retrying to send to {phone}... attempt {attempts}");
                            await Task.Delay(500); // delay بسيط 0.5 ثانية بين المحاولات
                        }
                    }
                }
            }

            return results;
        }
    }
}
