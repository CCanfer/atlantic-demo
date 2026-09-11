namespace Atlantic.Worker.Models
{
    public class DataProcesada
    {
        public int Id { get; set; }
        public int CargaArchivoId { get; set; }
        public string CodigoProducto { get; set; } 
        public string Periodo { get; set; }
        public DateTime FechaProceso { get; set; }
    }
}
