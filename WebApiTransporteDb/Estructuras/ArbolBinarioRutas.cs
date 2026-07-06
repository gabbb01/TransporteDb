using WebApiTransporteDb.Models;

namespace WebApiTransporteDb.Estructuras
{
    /// <summary>
    /// Nodo del Árbol Binario de Búsqueda.
    /// Almacena una ruta y referencias a sus hijos izquierdo y derecho.
    /// </summary>
    public class NodoArbol
    {
        /// <summary>
        /// Los datos de la ruta almacenada en este nodo.
        /// </summary>
        public Ruta Datos { get; set; }

        /// <summary>
        /// Referencia al hijo izquierdo (rutas con menor distancia).
        /// </summary>
        public NodoArbol? Izquierdo { get; set; }

        /// <summary>
        /// Referencia al hijo derecho (rutas con mayor distancia).
        /// </summary>
        public NodoArbol? Derecho { get; set; }

        /// <summary>
        /// Constructor: Crea un nodo con la ruta proporcionada.
        /// </summary>
        /// <param name="ruta">La ruta a almacenar en el nodo</param>
        public NodoArbol(Ruta ruta)
        {
            Datos = ruta;
            Izquierdo = null;
            Derecho = null;
        }
    }

    /// <summary>
    /// TDA Árbol Binario de Búsqueda (BST) para organizar rutas por distancia en kilómetros.
    /// Permite inserción, búsqueda y recorridos ordenados de las rutas del sistema de transporte.
    /// Criterio de ordenamiento: DistanciaKm de cada ruta.
    /// </summary>
    public class ArbolBinarioRutas
    {
        /// <summary>
        /// Raíz del árbol. Null si el árbol está vacío.
        /// </summary>
        public NodoArbol? Raiz { get; private set; }

        /// <summary>
        /// Constructor: Inicializa un árbol vacío.
        /// </summary>
        public ArbolBinarioRutas()
        {
            Raiz = null;
        }

        /// <summary>
        /// Inserta una nueva ruta en el árbol según su DistanciaKm.
        /// Las rutas con menor distancia van a la izquierda, las de mayor a la derecha.
        /// Análisis Big-O: O(log n) en promedio, O(n) en el peor caso (árbol degenerado)
        /// </summary>
        /// <param name="ruta">La ruta a insertar</param>
        public void Insertar(Ruta ruta)
        {
            Raiz = InsertarRecursivo(Raiz, ruta);
        }

        /// <summary>
        /// Método auxiliar recursivo para insertar un nodo en la posición correcta del árbol.
        /// </summary>
        /// <param name="nodo">Nodo actual en la recursión</param>
        /// <param name="ruta">La ruta a insertar</param>
        /// <returns>El nodo actualizado con la nueva inserción</returns>
        private NodoArbol InsertarRecursivo(NodoArbol? nodo, Ruta ruta)
        {
            // Caso base: llegamos a una posición vacía, aquí se crea el nuevo nodo
            if (nodo == null)
                return new NodoArbol(ruta);

            // Si la distancia es menor, insertamos en el subárbol izquierdo
            if (ruta.DistanciaKm < nodo.Datos.DistanciaKm)
                nodo.Izquierdo = InsertarRecursivo(nodo.Izquierdo, ruta);
            // Si la distancia es mayor o igual, insertamos en el subárbol derecho
            else
                nodo.Derecho = InsertarRecursivo(nodo.Derecho, ruta);

            return nodo;
        }

        /// <summary>
        /// Recorrido InOrder (Izquierda - Raíz - Derecha).
        /// Devuelve las rutas ordenadas de menor a mayor distancia.
        /// Análisis Big-O: O(n) donde n es el número de nodos
        /// </summary>
        /// <returns>Lista de rutas ordenadas ascendentemente por distancia</returns>
        public List<Ruta> RecorridoInOrder()
        {
            var resultado = new List<Ruta>();
            RecorridoInOrderRecursivo(Raiz, resultado);
            return resultado;
        }

        /// <summary>
        /// Método auxiliar recursivo para el recorrido InOrder.
        /// </summary>
        private void RecorridoInOrderRecursivo(NodoArbol? nodo, List<Ruta> resultado)
        {
            if (nodo == null) return;

            // Primero el subárbol izquierdo (distancias menores)
            RecorridoInOrderRecursivo(nodo.Izquierdo, resultado);
            // Luego el nodo actual
            resultado.Add(nodo.Datos);
            // Finalmente el subárbol derecho (distancias mayores)
            RecorridoInOrderRecursivo(nodo.Derecho, resultado);
        }

        /// <summary>
        /// Recorrido PreOrder (Raíz - Izquierda - Derecha).
        /// Útil para copiar la estructura del árbol.
        /// Análisis Big-O: O(n)
        /// </summary>
        /// <returns>Lista de rutas en orden PreOrder</returns>
        public List<Ruta> RecorridoPreOrder()
        {
            var resultado = new List<Ruta>();
            RecorridoPreOrderRecursivo(Raiz, resultado);
            return resultado;
        }

        /// <summary>
        /// Método auxiliar recursivo para el recorrido PreOrder.
        /// </summary>
        private void RecorridoPreOrderRecursivo(NodoArbol? nodo, List<Ruta> resultado)
        {
            if (nodo == null) return;

            // Primero el nodo actual (raíz)
            resultado.Add(nodo.Datos);
            // Luego el subárbol izquierdo
            RecorridoPreOrderRecursivo(nodo.Izquierdo, resultado);
            // Finalmente el subárbol derecho
            RecorridoPreOrderRecursivo(nodo.Derecho, resultado);
        }

        /// <summary>
        /// Recorrido PostOrder (Izquierda - Derecha - Raíz).
        /// Útil para eliminar el árbol de forma segura.
        /// Análisis Big-O: O(n)
        /// </summary>
        /// <returns>Lista de rutas en orden PostOrder</returns>
        public List<Ruta> RecorridoPostOrder()
        {
            var resultado = new List<Ruta>();
            RecorridoPostOrderRecursivo(Raiz, resultado);
            return resultado;
        }

        /// <summary>
        /// Método auxiliar recursivo para el recorrido PostOrder.
        /// </summary>
        private void RecorridoPostOrderRecursivo(NodoArbol? nodo, List<Ruta> resultado)
        {
            if (nodo == null) return;

            // Primero el subárbol izquierdo
            RecorridoPostOrderRecursivo(nodo.Izquierdo, resultado);
            // Luego el subárbol derecho
            RecorridoPostOrderRecursivo(nodo.Derecho, resultado);
            // Finalmente el nodo actual (raíz)
            resultado.Add(nodo.Datos);
        }

        /// <summary>
        /// Busca una ruta por su distancia en kilómetros.
        /// Análisis Big-O: O(log n) en promedio, O(n) en el peor caso
        /// </summary>
        /// <param name="distanciaKm">La distancia a buscar</param>
        /// <returns>La ruta encontrada o null si no existe</returns>
        public Ruta? Buscar(decimal distanciaKm)
        {
            return BuscarRecursivo(Raiz, distanciaKm);
        }

        /// <summary>
        /// Método auxiliar recursivo para buscar un nodo por distancia.
        /// </summary>
        private Ruta? BuscarRecursivo(NodoArbol? nodo, decimal distanciaKm)
        {
            // Caso base: no se encontró o el árbol está vacío
            if (nodo == null)
                return null;

            // Si la distancia coincide, encontramos la ruta
            if (distanciaKm == nodo.Datos.DistanciaKm)
                return nodo.Datos;

            // Si la distancia buscada es menor, buscar en el subárbol izquierdo
            if (distanciaKm < nodo.Datos.DistanciaKm)
                return BuscarRecursivo(nodo.Izquierdo, distanciaKm);

            // Si la distancia buscada es mayor, buscar en el subárbol derecho
            return BuscarRecursivo(nodo.Derecho, distanciaKm);
        }

        /// <summary>
        /// Cuenta el número total de nodos (rutas) en el árbol.
        /// Análisis Big-O: O(n)
        /// </summary>
        /// <returns>El número total de rutas en el árbol</returns>
        public int Contar()
        {
            return ContarRecursivo(Raiz);
        }

        /// <summary>
        /// Método auxiliar recursivo para contar nodos.
        /// </summary>
        private int ContarRecursivo(NodoArbol? nodo)
        {
            if (nodo == null)
                return 0;

            // Total = 1 (nodo actual) + nodos del subárbol izquierdo + nodos del subárbol derecho
            return 1 + ContarRecursivo(nodo.Izquierdo) + ContarRecursivo(nodo.Derecho);
        }
    }
}
