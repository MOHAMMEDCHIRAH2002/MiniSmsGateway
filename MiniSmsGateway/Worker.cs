using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Text.Json;

namespace MiniSmsGateway
{
    public class Worker : BackgroundService
    {
        private readonly ISmsApiClient _smsApiClient;
        private readonly ILogger<Worker> _logger;
        private readonly SmsConfig _config;
        private readonly SmsLogger _smsLogger ;

        public Worker(ILogger<Worker> logger, IOptions<SmsConfig> config, ISmsApiClient smsApiClient, SmsLogger smsLogger)
        {
            _logger = logger;
            _config = config.Value;
            _smsApiClient = smsApiClient;
            _smsLogger = smsLogger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{_config.HttpPort}/sms/");
            listener.Start();

            _logger.LogInformation($"🚀 HTTP Listener started on port {_config.HttpPort}");

            while (!stoppingToken.IsCancellationRequested)
            {
                var context = await listener.GetContextAsync();

                if (context.Request.HttpMethod == "POST")
                {
                    using var reader = new StreamReader(context.Request.InputStream);
                    var body = await reader.ReadToEndAsync();

                    _logger.LogInformation($"📥 Received Request Body: {body}");

                    // Deserialize JSON Body
                    SmsRequest? request;
                    try
                    {
                        request = JsonSerializer.Deserialize<SmsRequest>(body);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"❌ Failed to parse request: {ex.Message}");
                        context.Response.StatusCode = 400;
                        var error = Encoding.UTF8.GetBytes("Invalid JSON format.");
                        await context.Response.OutputStream.WriteAsync(error);
                        context.Response.Close();
                        continue;
                    }

                    // Validation
                    if (request == null || request.PhoneNumbers.Count == 0 || string.IsNullOrWhiteSpace(request.Message))
                    {
                        _logger.LogWarning("❗ Invalid request: Missing phone numbers or message.");
                        context.Response.StatusCode = 400;
                        var error = Encoding.UTF8.GetBytes("Invalid request format: Missing phone numbers or message.");
                        await context.Response.OutputStream.WriteAsync(error);
                        context.Response.Close();
                        continue;
                    }

                    // Log Request Details
                    _logger.LogInformation("✅ Request validated successfully.");

                    // Simulate Sending SMS
                    _logger.LogInformation("📤 Simulating SMS sending...");
                    var results = await _smsApiClient.SendSmsAsync(request);

                    for (int i = 0; i < request.PhoneNumbers.Count; i++)
                    {
                        _logger.LogInformation($"📤 Number: {request.PhoneNumbers[i]} - Status: {results[i]}");
                    }


                    //add results to sms-events.json for audit in the future
                    await _smsLogger.LogSmsAsync(request, results);
                    _logger.LogInformation("📝 SMS processing logged into sms-events.json");

                    // Create Success Response
                    context.Response.StatusCode = 202;
                    context.Response.ContentType = "application/json";

                    var jsonResponse = JsonSerializer.Serialize(new
                    {
                        message = "✅ SMS en cours de traitement.",
                        reference = request.Reference,
                        phoneNumbers = request.PhoneNumbers,
                        severity = request.Severity
                    });

                    var buffer = Encoding.UTF8.GetBytes(jsonResponse);
                    await context.Response.OutputStream.WriteAsync(buffer);
                    context.Response.Close();
                }
                else
                {
                    // Not Allowed
                    context.Response.StatusCode = 405; // Method Not Allowed
                    context.Response.Close();
                }
            }

            listener.Stop();
        }
    }
}
