namespace WebApiTransporteDb.Models
{
    public class Ruta
    {
        public int RutaId { get; set; }
        public int OrigenId { get; set; }
        public int DestinoId { get; set; }
        public decimal DistanciaKm { get; set; }
        public int TiempoMinutos { get; set; }
        public int CostoLempiras { get; set; }
    }
}
