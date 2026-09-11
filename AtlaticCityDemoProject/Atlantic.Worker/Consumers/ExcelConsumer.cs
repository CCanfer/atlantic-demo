using Atlantic.Worker.Logic;
using Atlantic.Worker.Models;
using Atlantic.Worker.Services;
using Newtonsoft.Json;
using RabbitLib;
using RabbitLib.Suscriptor;

namespace Atlantic.Worker.Consumers
{
    public class ExcelConsumer : MineduRabbitSuscribe, IHostedService
    {
        private readonly IConfiguration _config;
        private readonly IStorageService _storageService;
        private readonly IExcelProcesadorWorker _excelProcesadorWorker;

        protected override string exchange => "carga_masiva_exchange";
        protected override string type => "direct";
        protected override string queue => "carga_masiva";
        protected override string routingKey => "carga_masiva";

        public ExcelConsumer(IRabbitConnection rabbitConnection, IConfiguration config, IStorageService storageService, IExcelProcesadorWorker excelProcesadorWorker)
            : base(rabbitConnection)
        {
            _config = config;
            _storageService = storageService;
            _excelProcesadorWorker = excelProcesadorWorker;
        }

        public virtual Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public override async Task<bool> ConsumirMensaje(string mensajeSerializado)
        {

            try
            {
                var mensaje = JsonConvert.DeserializeObject<ExcelMessage>(mensajeSerializado);
                if (mensaje == null) return false;

                var fileStream = await _storageService.DownloadFileAsync(mensaje.FileUrl);

                await _excelProcesadorWorker.ProcesarLoteExcelAsync(fileStream, mensaje);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }

}
