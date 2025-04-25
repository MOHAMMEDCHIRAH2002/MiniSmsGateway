using MiniSmsGateway;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.Configure<SmsConfig>(
builder.Configuration.GetSection("SmsGateway"));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
