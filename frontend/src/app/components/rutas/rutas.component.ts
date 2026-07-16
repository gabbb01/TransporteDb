import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TransporteService } from '../../services/transporte.service';
import { Estacion } from '../../models/estacion.model';
import { Ruta } from '../../models/ruta.model';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-rutas',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './rutas.component.html',
  styleUrl: './rutas.component.scss'
})
export class RutasComponent implements OnInit {
  rutas: Ruta[] = [];
  estaciones: Estacion[] = [];
  rutasArbol: Ruta[] = [];
  mostrarFormulario = false;
  mostrarArbol = false;

  nuevaRuta: Partial<Ruta> = {
    origenId: 0,
    destinoId: 0,
    distanciaKm: 0,
    tiempoMinutos: 0,
    costoLempiras: 10
  };

  constructor(private transporteService: TransporteService) {}

  ngOnInit(): void {
    this.cargarDatos();
  }

  cargarDatos(): void {
    this.transporteService.getRutas().subscribe({
      next: (data) => this.rutas = data,
      error: () => Swal.fire('Error', 'No se pudieron cargar las rutas.', 'error')
    });
    this.transporteService.getEstaciones().subscribe({
      next: (data) => this.estaciones = data
    });
  }

  getNombreEstacion(id: number): string {
    const est = this.estaciones.find(e => e.estacionId === id);
    return est ? est.nombre : `ID: ${id}`;
  }

  crearRuta(): void {
    if (!this.nuevaRuta.origenId || !this.nuevaRuta.destinoId) {
      Swal.fire('Atención', 'Selecciona origen y destino.', 'warning');
      return;
    }
    if (this.nuevaRuta.origenId === this.nuevaRuta.destinoId) {
      Swal.fire('Atención', 'El origen y destino no pueden ser iguales.', 'warning');
      return;
    }

    this.transporteService.crearRuta(this.nuevaRuta).subscribe({
      next: (ruta) => {
        this.rutas.push(ruta);
        this.nuevaRuta = { origenId: 0, destinoId: 0, distanciaKm: 0, tiempoMinutos: 0, costoLempiras: 10 };
        this.mostrarFormulario = false;
        Swal.fire('¡Éxito!', 'Ruta creada correctamente.', 'success');
      },
      error: (err) => Swal.fire('Error', err.error?.mensaje || 'No se pudo crear la ruta.', 'error')
    });
  }

  verArbol(): void {
    this.transporteService.getRutasArbol().subscribe({
      next: (data) => {
        this.rutasArbol = data.rutas;
        this.mostrarArbol = true;
      },
      error: () => Swal.fire('Error', 'No se pudo obtener el árbol.', 'error')
    });
  }

  cancelar(): void {
    this.mostrarFormulario = false;
    this.nuevaRuta = { origenId: 0, destinoId: 0, distanciaKm: 0, tiempoMinutos: 0, costoLempiras: 10 };
  }
}
