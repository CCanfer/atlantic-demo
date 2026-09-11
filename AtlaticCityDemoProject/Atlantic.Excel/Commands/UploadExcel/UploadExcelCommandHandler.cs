using Atlantic.Excel.Services;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Minedu.RabbitMQ.Lib;

namespace Atlantic.Excel.Commands.UploadExcel
{
    public class UploadExcelCommandHandler : IRequestHandler<UploadExcelCommand, string>
    {
        private readonly IStorageService _storageService;
        private readonly IRabbitManager _rabbitManager;
        private readonly IConfiguration _config;
        public UploadExcelCommandHandler(IStorageService storageService, IRabbitManager rabbitManager, IConfiguration config)
        {
            _storageService = storageService;
            _rabbitManager = rabbitManager;
            _config = config;
        }

        public async Task<string> Handle(UploadExcelCommand request, CancellationToken cancellationToken)
        {

            if (!request.File.FileName.EndsWith(".xlsx"))
                throw new ArgumentException("El archivo debe ser un Excel (.xlsx)");

           
            var fileUrl = await _storageService.UploadFileAsync(request.File);

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var insertCargaQuery = @"INSERT INTO CargaArchivo (NombreArchivo, Url, Estado, FechaRegistro) 
                                         OUTPUT INSERTED.Id 
                                         VALUES (@NombreArchivo, @Url, @Estado, @FechaRegistro)";


            int cargaId = await connection.ExecuteScalarAsync<int>(insertCargaQuery, new
            {
                NombreArchivo = request.File.FileName,
                Url = fileUrl,
                Estado = "Pendiente",
                FechaRegistro = DateTime.UtcNow
            });

            var mensajeNotificacion = new
            {
                cargaId,
                fileUrl,
                request.Email
            };

            await _rabbitManager.EnviarMensaje(mensajeNotificacion, "carga_masiva_exchange", "direct", "carga_masiva");

            return fileUrl;
        }
    }
}
