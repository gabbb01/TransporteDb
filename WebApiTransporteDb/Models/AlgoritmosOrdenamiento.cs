using System.Collections.Generic;
using WebApiTransporteDb.Models;

namespace WebApiTransporteDb.Estructuras
{
    public static class AlgoritmosOrdenamiento
    {
        // Algoritmo MergeSort (Recursivo)
        public static List<Ruta> MergeSort(List<Ruta> rutas)
        {
            if (rutas.Count <= 1) return rutas;

            int medio = rutas.Count / 2;
            var izquierda = MergeSort(rutas.GetRange(0, medio));
            var derecha = MergeSort(rutas.GetRange(medio, rutas.Count - medio));

            return Fusionar(izquierda, derecha);
        }

        private static List<Ruta> Fusionar(List<Ruta> izq, List<Ruta> der)
        {
            var resultado = new List<Ruta>();
            int i = 0, j = 0;

            while (i < izq.Count && j < der.Count)
            {
                if (izq[i].DistanciaKm <= der[j].DistanciaKm)
                    resultado.Add(izq[i++]);
                else
                    resultado.Add(der[j++]);
            }
            resultado.AddRange(izq.GetRange(i, izq.Count - i));
            resultado.AddRange(der.GetRange(j, der.Count - j));
            return resultado;
        }
    }
}