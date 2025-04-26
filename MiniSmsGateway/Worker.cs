using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Text.Json;


namespace MiniSmsGateway
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly SmsConfig _config;

        public Worker(ILogger<Worker> logger, IOptions<SmsConfig> config)
        {
            _logger = logger;
            _config = config.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{_config.HttpPort}/sms/");
            listener.Start();

            _logger.LogInformation($"HTTP Listener started on port {_config.HttpPort}");

            while (!stoppingToken.IsCancellationRequested) { 
            
            var context =await listener.GetContextAsync();

                if(context.Request.HttpMethod=="POST")
                {
                    using var reader = new StreamReader(context.Request.InputStream);
                    var body = await reader.ReadToEndAsync();

                    Console.WriteLine("Request Body :" +body);

                    //deserialize the body
                    var request=JsonSerializer.Deserialize<SmsRequest>(body);

                    Console.WriteLine("Deserialized Request Body :" + request.ToString());

                    //validation
                    if (request == null || request.PhoneNumbers.Count == 0 || string.IsNullOrWhiteSpace(request.Message)) {
                        context.Response.StatusCode = 400;
                        var error = Encoding.UTF8.GetBytes("Invalid request format.");
                        await context.Response.OutputStream.WriteAsync(error);
                        context.Response.Close();
                        continue;

                    }

                    _logger.LogInformation("📤 Simulating SMS sending...");
                    foreach (var number in request.PhoneNumbers)
                    {
                        _logger.LogInformation($"Sent to {number}: {request.Message} (Severity: {request.Severity})");
                    }



                    _logger.LogInformation("🔔 Received SMS request:");
                    _logger.LogInformation(body);




                    //Response
                    context.Response.StatusCode = 202;
                    context.Response.ContentType = "application/json";

                    var jsonResponse = JsonSerializer.Serialize(
                        new
                        {
                            message = "SMS en cours de traitement.",
                        }
                        );
                    var buffer= Encoding.UTF8.GetBytes(jsonResponse);
                    
                    await context.Response.OutputStream.WriteAsync(buffer);
                    context.Response.Close();
                }
                else
                {

                    context.Response.StatusCode = 405;
                    context.Response.Close();
                }


            }
            listener.Stop();

        }
    }
}
