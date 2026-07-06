namespace WebApiTransporteDb.Models
{
    public class Estacion
    {
        public int EstacionId { get; set; }
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
        public string? Ubicacion { get; set; }
        public bool Activa { get; set; }
    }
}

