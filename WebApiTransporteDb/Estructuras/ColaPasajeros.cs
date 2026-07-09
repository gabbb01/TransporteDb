using WebApiTransporteDb.Models;

namespace WebApiTransporteDb.Estructuras
{

    /// TDA Cola de Pasajeros (FIFO - Primero en entrar, primero en salir).
    /// Gestiona la espera de pasajeros en cada estación del sistema de transporte.
    public class ColaPasajeros
    {
        // Estructura interna: Cola genérica de .NET que implementa FIFO
        private readonly Queue<Pasajero> _cola;

        /// Constructor: Inicializa la cola vacía.

        public ColaPasajeros()
        {
            _cola = new Queue<Pasajero>();
        }

        /// <summary>
        /// Propiedad que devuelve la cantidad de pasajeros esperando en la cola.
        /// Análisis Big-O: O(1)
        /// </summary>
        public int CantidadEsperando => _cola.Count;

        /// <summary>
        /// Agrega un pasajero al final de la cola (operación Enqueue).
        /// Análisis Big-O: O(1) amortizado
        /// </summary>
        /// <param name="p">El pasajero a encolar</param>
        public void Encolar(Pasajero p)
        {
            _cola.Enqueue(p);
        }

        /// <summary>
        /// Remueve y devuelve el pasajero al frente de la cola (operación Dequeue).
        /// Análisis Big-O: O(1)
        /// </summary>
        /// <returns>El pasajero atendido, o null si la cola está vacía</returns>
        public Pasajero? Desencolar()
        {
            if (_cola.Count == 0)
                return null;

            return _cola.Dequeue();
        }

        /// <summary>
        /// Observa el siguiente pasajero a ser atendido sin removerlo de la cola (operación Peek).
        /// Análisis Big-O: O(1)
        /// </summary>
        /// <returns>El siguiente pasajero, o null si la cola está vacía</returns>
        public Pasajero? VerSiguiente()
        {
            if (_cola.Count == 0)
                return null;

            return _cola.Peek();
        }

        /// <summary>
        /// Devuelve una lista con todos los pasajeros en la cola sin removerlos.
        /// Útil para consultas y reportes.
        /// Análisis Big-O: O(n) donde n es el número de pasajeros en la cola
        /// </summary>
        /// <returns>Lista de todos los pasajeros en orden de llegada</returns>
        public List<Pasajero> ObtenerTodos()
        {
            return _cola.ToList();
        }

        /// <summary>
        /// Verifica si la cola está vacía.
        /// Análisis Big-O: O(1)
        /// </summary>
        /// <returns>true si no hay pasajeros en la cola, false en caso contrario</returns>
        public bool EstaVacia()
        {
            return _cola.Count == 0;
        }
    }
}
