using Atlantic.Worker.Consumers;
using Atlantic.Worker.Logic;
using Atlantic.Worker.Services;
using Minedu.RabbitMQ.Lib;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IExcelProcesadorWorker, ExcelProcesadorWorker>();
builder.Services.AddHttpClient<IStorageService, SeaweedFsService>();

builder.Services.AddRabbit(builder.Configuration);
builder.Services.AddHostedService<ExcelConsumer>();

var app = builder.Build();
app.Run();