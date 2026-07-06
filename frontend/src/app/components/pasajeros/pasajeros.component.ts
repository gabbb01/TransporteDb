import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TransporteService } from '../../services/transporte.service';
import { Estacion } from '../../models/estacion.model';
import { Pasajero } from '../../models/pasajero.model';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-pasajeros',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './pasajeros.component.html',
  styleUrl: './pasajeros.component.scss'
})
export class PasajerosComponent implements OnInit {
  estaciones: Estacion[] = [];
  estacionSeleccionada = '';
  pasajeros: Pasajero[] = [];
  nombreEstacion = '';
  cantidadEnEspera = 0;

  nuevoPasajero: Partial<Pasajero> = {
    nombre: '',
    destinoDeseado: ''
  };

  constructor(private transporteService: TransporteService) {}

  ngOnInit(): void {
    this.transporteService.getEstaciones().subscribe({
      next: (data) => this.estaciones = data
    });
  }

  cargarPasajeros(): void {
    if (!this.estacionSeleccionada) return;

    this.transporteService.getPasajeros(this.estacionSeleccionada).subscribe({
      next: (data) => {
        this.pasajeros = data.pasajeros;
        this.nombreEstacion = data.estacion;
        this.cantidadEnEspera = data.cantidadEnEspera;
      },
      error: () => Swal.fire('Error', 'No se pudieron cargar los pasajeros.', 'error')
    });
  }

  encolar(): void {
    if (!this.estacionSeleccionada) {
      Swal.fire('Atención', 'Selecciona una estación primero.', 'warning');
      return;
    }
    if (!this.nuevoPasajero.nombre) {
      Swal.fire('Atención', 'El nombre del pasajero es obligatorio.', 'warning');
      return;
    }

    this.transporteService.encolarPasajero(this.estacionSeleccionada, this.nuevoPasajero as Pasajero).subscribe({
      next: (res) => {
        this.nuevoPasajero = { nombre: '', destinoDeseado: '' };
        this.cargarPasajeros();
        Swal.fire('¡Encolado!', res.mensaje, 'success');
      },
      error: (err) => Swal.fire('Error', err.error?.mensaje || 'Error al encolar.', 'error')
    });
  }

  desencolar(): void {
    if (!this.estacionSeleccionada) {
      Swal.fire('Atención', 'Selecciona una estación primero.', 'warning');
      return;
    }

    Swal.fire({
      title: '¿Atender siguiente pasajero?',
      text: 'Se desencolará al primer pasajero en la fila (FIFO).',
      icon: 'question',
      showCancelButton: true,
      confirmButtonText: 'Sí, atender',
      cancelButtonText: 'Cancelar'
    }).then((result) => {
      if (result.isConfirmed) {
        this.transporteService.desencolarPasajero(this.estacionSeleccionada).subscribe({
          next: (res) => {
            this.cargarPasajeros();
            Swal.fire('Atendido', res.mensaje, 'success');
          },
          error: (err) => Swal.fire('Info', err.error?.mensaje || 'No hay pasajeros en la cola.', 'info')
        });
      }
    });
  }

  onEstacionChange(): void {
    this.cargarPasajeros();
  }
}
