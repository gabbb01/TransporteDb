import { Ruta } from './ruta.model';
import { Pasajero } from './pasajero.model';

export interface ReporteTrafico {
  estacionesMasConectadas: EstacionReporte[];
  rutasOrdenadasPorDistancia: Ruta[];
  pasajerosPorEstacion: EstacionPasajeros[];
  totalEstaciones: number;
  totalRutas: number;
  totalPasajerosEnEspera: number;
}

export interface EstacionReporte {
  codigo: string;
  nombre: string;
  numeroConexiones: number;
}

export interface EstacionPasajeros {
  codigoEstacion: string;
  nombreEstacion: string;
  pasajerosEnEspera: number;
  pasajeros: Pasajero[];
}
