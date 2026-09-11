namespace Atlantic.Worker.Models
{
    public class CargaArchivo
    {
        public int Id { get; set; }
        public string NombreArchivo { get; set; }
        public string UrlAlmacenamiento { get; set; }
        public string Estado { get; set; }
        public int TotalRegistros { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
