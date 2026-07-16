using System.Collections.Generic;

namespace WebApiTransporteDb.Models
{
    public class ResultadoRuta
    {
        public List<Estacion> Camino { get; set; } = new List<Estacion>();
        public decimal DistanciaTotalKm { get; set; }
        public int CostoTotalLempiras { get; set; }
        public bool RutaEncontrada { get; set; }
    }
}
