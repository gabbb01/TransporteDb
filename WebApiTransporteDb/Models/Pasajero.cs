namespace WebApiTransporteDb.Models
{
    public class Pasajero
    {
        public string Nombre { get; set; } = string.Empty;
        public string DestinoDeseado { get; set; } = string.Empty;
        public DateTime HoraLlegada { get; set; } = DateTime.Now;
    }
}
