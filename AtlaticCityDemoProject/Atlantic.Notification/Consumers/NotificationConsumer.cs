using Atlantic.Notification.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using RabbitLib;
using RabbitLib.Suscriptor;

namespace Atlantic.Worker.Consumers
{
    public class NotificationConsumer : MineduRabbitSuscribe, IHostedService
    {
        private readonly IConfiguration _config;

        protected override string exchange => "carga_masiva_exchange";
        protected override string type => "direct";
        protected override string queue => "notificaciones";
        protected override string routingKey => "notificaciones";

        public NotificationConsumer(IRabbitConnection rabbitConnection, IConfiguration config)
            : base(rabbitConnection)
        {
            _config = config;
        }

        public virtual Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public override async Task<bool> ConsumirMensaje(string mensajeSerializado)
        {

            try
            {
                var mensaje = JsonConvert.DeserializeObject<NotificationMessage>(mensajeSerializado);
                if (mensaje == null) return false;

                Console.WriteLine($"Notificacion :  {mensajeSerializado}");

                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var queryUpdate = "UPDATE CargaArchivo SET Estado = 'Notificado' WHERE Id = @Id";
                await connection.ExecuteAsync(queryUpdate, new { Id = mensaje.CargaId });

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
