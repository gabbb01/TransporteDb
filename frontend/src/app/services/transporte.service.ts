import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Estacion } from '../models/estacion.model';
import { Ruta } from '../models/ruta.model';
import { Pasajero } from '../models/pasajero.model';
import { ResultadoRuta } from '../models/resultado-ruta.model';
import { ReporteTrafico } from '../models/reporte-trafico.model';

@Injectable({
  providedIn: 'root'
})
export class TransporteService {

  private apiUrl = 'https://localhost:7004/api/transporte';

  constructor(private http: HttpClient) {}

  // ==================== ESTACIONES ====================

  getEstaciones(): Observable<Estacion[]> {
    return this.http.get<Estacion[]>(`${this.apiUrl}/estaciones`);
  }

  getEstacionPorCodigo(codigo: string): Observable<Estacion> {
    return this.http.get<Estacion>(`${this.apiUrl}/estaciones/${codigo}`);
  }

  crearEstacion(estacion: Partial<Estacion>): Observable<Estacion> {
    return this.http.post<Estacion>(`${this.apiUrl}/estaciones`, estacion);
  }

  // ==================== RUTAS ====================

  getRutas(): Observable<Ruta[]> {
    return this.http.get<Ruta[]>(`${this.apiUrl}/rutas`);
  }

  crearRuta(ruta: Partial<Ruta>): Observable<Ruta> {
    return this.http.post<Ruta>(`${this.apiUrl}/rutas`, ruta);
  }

  getRutasArbol(): Observable<{ totalRutas: number; rutas: Ruta[] }> {
    return this.http.get<{ totalRutas: number; rutas: Ruta[] }>(`${this.apiUrl}/rutas/arbol`);
  }

  // ==================== CONEXIONES ====================

  getConexiones(codigo: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/conexiones/${codigo}`);
  }

  // ==================== CAMINO MÁS CORTO ====================

  getCaminoCorto(origen: string, destino: string): Observable<ResultadoRuta> {
    const params = new HttpParams().set('origen', origen).set('destino', destino);
    return this.http.get<ResultadoRuta>(`${this.apiUrl}/camino-corto`, { params });
  }

  // ==================== PASAJEROS ====================

  encolarPasajero(codigoEstacion: string, pasajero: Partial<Pasajero>): Observable<any> {
    return this.http.post(`${this.apiUrl}/pasajeros/encolar/${codigoEstacion}`, pasajero);
  }

  desencolarPasajero(codigoEstacion: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/pasajeros/desencolar/${codigoEstacion}`, {});
  }

  getPasajeros(codigoEstacion: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/pasajeros/${codigoEstacion}`);
  }

  // ==================== REPORTES ====================

  getReporteTrafico(): Observable<ReporteTrafico> {
    return this.http.get<ReporteTrafico>(`${this.apiUrl}/reportes/trafico`);
  }
}
