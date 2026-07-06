using Microsoft.Data.SqlClient;
using Dapper;
using WebApiTransporteDb.Models;
using WebApiTransporteDb.Estructuras;

namespace WebApiTransporteDb.Services
{
    public class TransporteService
    {
        private readonly string _connectionString;

        // Aquí vive tu TDA en memoria RAM
        public GrafoUrbano Grafo { get; private set; }

        public TransporteService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Falta la cadena de conexión 'DefaultConnection' en appsettings.json");
            Grafo = new GrafoUrbano();
            CargarDatosEnMemoria();
        }

        private void CargarDatosEnMemoria()
        {
            using var connection = new SqlConnection(_connectionString);

            // 1. Traer datos crudos de SQL con Dapper
            var estaciones = connection.Query<Estacion>("SELECT * FROM Estaciones WHERE Activa = 1").ToList();
            var rutas = connection.Query<Ruta>("SELECT * FROM Rutas").ToList();

            // 2. Poblar la Tabla Hash y los Vértices del Grafo
            foreach (var est in estaciones)
            {
                Grafo.AgregarEstacion(est);
            }

            // 3. Crear las Aristas (Las rutas entre las estaciones)
            foreach (var ruta in rutas)
            {
                var origen = estaciones.FirstOrDefault(e => e.EstacionId == ruta.OrigenId);
                var destino = estaciones.FirstOrDefault(e => e.EstacionId == ruta.DestinoId);

                if (origen != null && destino != null)
                {
                    Grafo.AgregarRuta(origen, destino, ruta);
                }
            }
        }

        // ==================== CONSULTAS ====================

        // Búsqueda O(1) gracias a la Tabla Hash
        public Estacion? BuscarEstacionPorCodigo(string codigo)
        {
            return Grafo.Nodos.TryGetValue(codigo, out var nodo) ? nodo.Datos : null;
        }

        // Obtener todas las estaciones del grafo
        public List<Estacion> ObtenerEstaciones()
        {
            return Grafo.Nodos.Values.Select(n => n.Datos).ToList();
        }

        // Obtener conexiones de una estación específica
        public List<object> ObtenerConexiones(string codigo)
        {
            if (!Grafo.Nodos.TryGetValue(codigo, out var nodo))
                return new List<object>();

            return nodo.Conexiones.Select(r => new
            {
                r.RutaId,
                r.OrigenId,
                r.DestinoId,
                r.DistanciaKm,
                r.TiempoMinutos,
                NombreDestino = Grafo.ObtenerCodigoPorId(r.DestinoId) is string codDest
                    && Grafo.Nodos.TryGetValue(codDest, out var nodoDest)
                    ? nodoDest.Datos.Nombre : "Desconocido"
            }).ToList<object>();
        }

        // Calcular camino más corto (encapsula acceso al grafo)
        public ResultadoRuta CalcularCaminoCorto(string origen, string destino)
        {
            return Grafo.CalcularCaminoCorto(origen, destino);
        }

        // ==================== CRUD ESTACIONES ====================

        public Estacion CrearEstacion(Estacion estacion)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"INSERT INTO Estaciones (Codigo, Nombre, Ubicacion, Activa)
                        VALUES (@Codigo, @Nombre, @Ubicacion, @Activa);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

            estacion.EstacionId = connection.QuerySingle<int>(sql, estacion);

            // Sincronizar con el TDA en memoria
            Grafo.AgregarEstacion(estacion);

            return estacion;
        }

        // ==================== CRUD RUTAS ====================

        public Ruta CrearRuta(Ruta ruta)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos)
                        VALUES (@OrigenId, @DestinoId, @DistanciaKm, @TiempoMinutos);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

            ruta.RutaId = connection.QuerySingle<int>(sql, ruta);

            // Sincronizar con el TDA en memoria
            var origenCodigo = Grafo.ObtenerCodigoPorId(ruta.OrigenId);
            var destinoCodigo = Grafo.ObtenerCodigoPorId(ruta.DestinoId);

            if (origenCodigo != null && destinoCodigo != null
                && Grafo.Nodos.TryGetValue(origenCodigo, out var nodoOrigen)
                && Grafo.Nodos.TryGetValue(destinoCodigo, out var nodoDestino))
            {
                Grafo.AgregarRuta(nodoOrigen.Datos, nodoDestino.Datos, ruta);
            }

            return ruta;
        }

        public List<Ruta> ObtenerRutas()
        {
            using var connection = new SqlConnection(_connectionString);
            return connection.Query<Ruta>("SELECT * FROM Rutas").ToList();
        }

        // ==================== PASAJEROS (COLA) ====================

        public bool EncolarPasajero(string codigoEstacion, Pasajero pasajero)
        {
            if (!Grafo.Nodos.TryGetValue(codigoEstacion, out var nodo))
                return false;

            pasajero.HoraLlegada = DateTime.Now;
            nodo.ColaPasajeros.Encolar(pasajero);
            return true;
        }

        public Pasajero? DesencolarPasajero(string codigoEstacion)
        {
            if (!Grafo.Nodos.TryGetValue(codigoEstacion, out var nodo))
                return null;

            return nodo.ColaPasajeros.Desencolar();
        }

        public List<Pasajero> ObtenerPasajeros(string codigoEstacion)
        {
            if (!Grafo.Nodos.TryGetValue(codigoEstacion, out var nodo))
                return new List<Pasajero>();

            return nodo.ColaPasajeros.ObtenerTodos();
        }

        // ==================== RUTAS ARBOL BINARIO ====================

        public List<Ruta> ObtenerRutasOrdenadas()
        {
            return Grafo.ArbolRutas.RecorridoInOrder();
        }

        // ==================== REPORTES ====================

        public ReporteTrafico GenerarReporteTrafico()
        {
            var reporte = new ReporteTrafico();

            // Estaciones más conectadas (usando MergeSort indirectamente, ordenamos por conexiones)
            reporte.EstacionesMasConectadas = Grafo.Nodos.Values
                .Select(n => new EstacionReporte
                {
                    Codigo = n.Datos.Codigo,
                    Nombre = n.Datos.Nombre,
                    NumeroConexiones = n.Conexiones.Count
                })
                .OrderByDescending(e => e.NumeroConexiones)
                .ToList();

            // Rutas ordenadas por distancia usando MergeSort (TDA obligatorio)
            var todasLasRutas = Grafo.Nodos.Values
                .SelectMany(n => n.Conexiones)
                .Distinct()
                .ToList();
            reporte.RutasOrdenadasPorDistancia = AlgoritmosOrdenamiento.MergeSort(todasLasRutas);

            // Pasajeros por estación (datos de las Colas)
            reporte.PasajerosPorEstacion = Grafo.Nodos.Values
                .Select(n => new EstacionPasajeros
                {
                    CodigoEstacion = n.Datos.Codigo,
                    NombreEstacion = n.Datos.Nombre,
                    PasajerosEnEspera = n.ColaPasajeros.CantidadEsperando,
                    Pasajeros = n.ColaPasajeros.ObtenerTodos()
                })
                .Where(ep => ep.PasajerosEnEspera > 0)
                .ToList();

            reporte.TotalEstaciones = Grafo.Nodos.Count;
            reporte.TotalRutas = todasLasRutas.Count;
            reporte.TotalPasajerosEnEspera = reporte.PasajerosPorEstacion.Sum(ep => ep.PasajerosEnEspera);

            return reporte;
        }
    }
}