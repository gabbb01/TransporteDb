import { Estacion } from './estacion.model';

export interface ResultadoRuta {
  camino: Estacion[];
  distanciaTotalKm: number;
  costoTotalLempiras: number;
  rutaEncontrada: boolean;
}
