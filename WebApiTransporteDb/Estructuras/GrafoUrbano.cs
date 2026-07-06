using System.Collections.Generic;
using WebApiTransporteDb.Models;
using System.Linq;

namespace WebApiTransporteDb.Estructuras
{
    // TDA 1: El Nodo del Grafo
    public class NodoEstacion
    {
        public Estacion Datos { get; set; }
        public List<Ruta> Conexiones { get; set; } // TDA 2: Lista de Adyacencia
        public ColaPasajeros ColaPasajeros { get; set; } // TDA 6: Cola de pasajeros esperando

        public NodoEstacion(Estacion estacion)
        {
            Datos = estacion;
            Conexiones = new List<Ruta>();
            ColaPasajeros = new ColaPasajeros();
        }
    }

    // TDA 3: El Grafo Principal
    public class GrafoUrbano
    {
        // TDA 4: La Tabla Hash (Para búsquedas O(1))
        public Dictionary<string, NodoEstacion> Nodos { get; set; }

        // TDA 5: Árbol Binario de Búsqueda para organizar rutas
        public ArbolBinarioRutas ArbolRutas { get; set; }

        // Diccionario auxiliar para mapeo rápido EstacionId -> Codigo O(1)
        private Dictionary<int, string> _idACodigo;

        public GrafoUrbano()
        {
            Nodos = new Dictionary<string, NodoEstacion>();
            ArbolRutas = new ArbolBinarioRutas();
            _idACodigo = new Dictionary<int, string>();
        }

        // Análisis Big-O: O(1) gracias a la Tabla Hash
        public void AgregarEstacion(Estacion estacion)
        {
            if (!Nodos.ContainsKey(estacion.Codigo))
            {
                Nodos.Add(estacion.Codigo, new NodoEstacion(estacion));
                _idACodigo[estacion.EstacionId] = estacion.Codigo;
            }
        }

        // Análisis Big-O: O(1) para agregar + O(log n) para insertar en árbol
        public void AgregarRuta(Estacion origen, Estacion destino, Ruta ruta)
        {
            if (Nodos.ContainsKey(origen.Codigo) && Nodos.ContainsKey(destino.Codigo))
            {
                Nodos[origen.Codigo].Conexiones.Add(ruta);
                // Insertar la ruta en el Árbol Binario de Búsqueda
                ArbolRutas.Insertar(ruta);
            }
        }

        // Obtener el código de una estación por su ID — O(1)
        public string? ObtenerCodigoPorId(int estacionId)
        {
            return _idACodigo.TryGetValue(estacionId, out var codigo) ? codigo : null;
        }

        // Algoritmo de Dijkstra para encontrar el camino más corto
        // Análisis Big-O: O((V + E) log V) con Cola de Prioridad
        public ResultadoRuta CalcularCaminoCorto(string codigoOrigen, string codigoDestino)
        {
            var resultado = new ResultadoRuta();

            // 1. Validar que ambas estaciones existan en nuestra Tabla Hash O(1)
            if (!Nodos.ContainsKey(codigoOrigen) || !Nodos.ContainsKey(codigoDestino))
            {
                resultado.RutaEncontrada = false;
                return resultado;
            }

            // Diccionario para guardar la distancia mínima conocida desde el origen a cada nodo
            var distancias = new Dictionary<string, decimal>();

            // Diccionario para saber de dónde venimos y poder reconstruir el camino al final
            var previos = new Dictionary<string, string>();

            // HashSet para no re-procesar nodos ya visitados
            var visitados = new HashSet<string>();

            // TDA: La Cola de Prioridad. Ordenará automáticamente las estaciones por la distancia más corta
            var colaPrioridad = new PriorityQueue<string, decimal>();

            // 2. Inicialización
            foreach (var nodo in Nodos.Keys)
            {
                distancias[nodo] = decimal.MaxValue; // Asignamos "Infinito" a todos
            }
            distancias[codigoOrigen] = 0; // La distancia del origen a sí mismo es 0
            colaPrioridad.Enqueue(codigoOrigen, 0);

            // 3. Ciclo principal de Dijkstra
            while (colaPrioridad.Count > 0)
            {
                // Extraemos la estación con la menor distancia acumulada
                string actual = colaPrioridad.Dequeue();

                // Si ya visitamos este nodo, lo saltamos
                if (visitados.Contains(actual)) continue;
                visitados.Add(actual);

                // Si llegamos al destino, podemos detener la búsqueda (optimización)
                if (actual == codigoDestino) break;

                // Explorar los vecinos (Lista de Adyacencia)
                foreach (var ruta in Nodos[actual].Conexiones)
                {
                    // CORREGIDO: Usamos el diccionario de mapeo O(1) en vez de búsqueda lineal O(N)
                    var nodoDestino = _idACodigo.TryGetValue(ruta.DestinoId, out var cod) ? cod : null;
                    if (nodoDestino == null || visitados.Contains(nodoDestino)) continue;

                    decimal nuevaDistancia = distancias[actual] + ruta.DistanciaKm;

                    // Si encontramos un camino más corto hacia este vecino, actualizamos
                    if (nuevaDistancia < distancias[nodoDestino])
                    {
                        distancias[nodoDestino] = nuevaDistancia;
                        previos[nodoDestino] = actual;
                        colaPrioridad.Enqueue(nodoDestino, nuevaDistancia);
                    }
                }
            }

            // 4. Reconstrucción del camino (Backtracking)
            if (distancias[codigoDestino] == decimal.MaxValue)
            {
                resultado.RutaEncontrada = false;
                return resultado;
            }

            string? pasoActual = codigoDestino;
            while (pasoActual != null)
            {
                resultado.Camino.Insert(0, Nodos[pasoActual].Datos);
                pasoActual = previos.TryGetValue(pasoActual, out var prev) ? prev : null;
            }

            resultado.DistanciaTotalKm = distancias[codigoDestino];
            resultado.RutaEncontrada = true;

            return resultado;
        }
    }
}