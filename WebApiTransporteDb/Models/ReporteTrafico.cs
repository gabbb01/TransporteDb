namespace WebApiTransporteDb.Models
{
    public class ReporteTrafico
    {
        public List<EstacionReporte> EstacionesMasConectadas { get; set; } = new();
        public List<Ruta> RutasOrdenadasPorDistancia { get; set; } = new();
        public List<EstacionPasajeros> PasajerosPorEstacion { get; set; } = new();
        public int TotalEstaciones { get; set; }
        public int TotalRutas { get; set; }
        public int TotalPasajerosEnEspera { get; set; }
    }

    public class EstacionReporte
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int NumeroConexiones { get; set; }
    }

    public class EstacionPasajeros
    {
        public string CodigoEstacion { get; set; } = string.Empty;
        public string NombreEstacion { get; set; } = string.Empty;
        public int PasajerosEnEspera { get; set; }
        public List<Pasajero> Pasajeros { get; set; } = new();
    }
}
