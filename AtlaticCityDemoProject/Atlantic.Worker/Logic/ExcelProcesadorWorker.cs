using Atlantic.Worker.Models;
using ClosedXML.Excel;
using Dapper;
using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.Data.SqlClient;
using Minedu.RabbitMQ.Lib;
using System.Data;

namespace Atlantic.Worker.Logic
{
    public class ExcelProcesadorWorker: IExcelProcesadorWorker
    {
        private readonly string _connectionString;
        private readonly IRabbitManager _rabbitManager;
        public ExcelProcesadorWorker(IConfiguration configuration, IRabbitManager rabbitManager)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new ArgumentNullException("No se encontró la cadena de conexión");
            _rabbitManager = rabbitManager;
        }

        public async Task ProcesarLoteExcelAsync(Stream excelStream, ExcelMessage message)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                using var workbook = new XLWorkbook(excelStream);
                var worksheet = workbook.Worksheet(1);

                var periodo = worksheet.Cell("B2").GetString().Trim();

                if (string.IsNullOrEmpty(periodo))
                {
                    await ActualizarEstadoCarga(connection, message.CargaId, "Error");
                    return;
                }

                var queryValidar = @"
    SELECT TOP 1 c.Estado 
    FROM DetalleCarga d
    INNER JOIN CargaArchivo c ON d.CargaArchivoId = c.Id
    WHERE d.Periodo = @Periodo
    ORDER BY c.Id DESC";

                var estadoExistente = await connection.QueryFirstOrDefaultAsync<string>(queryValidar, new { Periodo = periodo });

                if (estadoExistente != null)
                {
                    if (estadoExistente == "Cargado" || estadoExistente == "Finalizado" || estadoExistente == "Notificado")
                    {
                        await ActualizarEstadoCarga(connection, message.CargaId, "Rechazada");
                        return;
                    }
                    else if (estadoExistente == "Pendiente" || estadoExistente == "En Proceso")
                    {
                        await ActualizarEstadoCarga(connection, message.CargaId, "Bloqueada");
                        return;
                    }
                }

                var codigosProducto = new List<string>();

                var filas = worksheet.RowsUsed().Skip(1);

                foreach (var fila in filas)
                {
                    var codigoProducto = fila.Cell(1).Value.ToString() ?? string.Empty;
                    codigosProducto.Add(codigoProducto);
                }

                if (codigosProducto.Count == 0)
                {
                    await ActualizarEstadoCarga(connection, message.CargaId, "Error");
                    return;
                }

                var queryDetalle = @"INSERT INTO DetalleCarga (CargaArchivoId, Periodo, CantidadFilas) 
                                 VALUES (@CargaId, @Periodo, @Cantidad);";
                await connection.ExecuteAsync(queryDetalle, new { CargaId = message.CargaId, Periodo = periodo, Cantidad = codigosProducto.Count });

                var dataAInsertar = codigosProducto.Select(codigo => new
                {
                    CargaArchivoId = message.CargaId,
                    CodigoProducto = codigo
                }).ToList();

                var queryData = @"
    INSERT INTO DataProcesada (CargaArchivoId, CodigoProducto, MensajeLog) 
    SELECT 
        @CargaArchivoId, 
        @CodigoProducto, 
        CASE 
            WHEN EXISTS (SELECT 1 FROM DataProcesada WHERE CodigoProducto = @CodigoProducto) 
            THEN 'Existente' 
            ELSE NULL 
        END;";

                await connection.ExecuteAsync(queryData, dataAInsertar);

                await ActualizarEstadoCarga(connection, message.CargaId, "Completado");

                var mensajeNotificacion = new
                {
                    CargaId = message.CargaId,
                    Usuario = message.Email,
                    FechaFin = DateTime.Now
                };

                await _rabbitManager.EnviarMensaje(mensajeNotificacion, "carga_masiva_exchange", "direct", "notificaciones");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando Excel: {ex.Message}");
                await ActualizarEstadoCarga(connection, message.CargaId, "Error");
            }
        }

        private Task ActualizarEstadoCarga(IDbConnection db, int id, string estado)
        {
            return db.ExecuteAsync("UPDATE CargaArchivo SET Estado = @Estado WHERE Id = @Id", new { Estado = estado, Id = id });
        }
    }

    public interface IExcelProcesadorWorker
    {
        Task ProcesarLoteExcelAsync(Stream excelStream, ExcelMessage message);
    }
}
