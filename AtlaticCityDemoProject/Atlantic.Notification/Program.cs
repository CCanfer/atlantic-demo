using Atlantic.Worker.Consumers;
using Minedu.RabbitMQ.Lib;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRabbit(builder.Configuration);
builder.Services.AddHostedService<NotificationConsumer>();

var app = builder.Build();
app.Run();

