import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TransporteService } from '../../services/transporte.service';
import { Estacion } from '../../models/estacion.model';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-estaciones',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './estaciones.component.html',
  styleUrl: './estaciones.component.scss'
})
export class EstacionesComponent implements OnInit {
  estaciones: Estacion[] = [];
  filtro: string = '';
  mostrarFormulario = false;

  nuevaEstacion: Partial<Estacion> = {
    codigo: '',
    nombre: '',
    ubicacion: '',
    activa: true
  };

  constructor(private transporteService: TransporteService) {}

  ngOnInit(): void {
    this.cargarEstaciones();
  }

  cargarEstaciones(): void {
    this.transporteService.getEstaciones().subscribe({
      next: (data) => this.estaciones = data,
      error: () => Swal.fire('Error', 'No se pudieron cargar las estaciones.', 'error')
    });
  }

  get estacionesFiltradas(): Estacion[] {
    if (!this.filtro.trim()) return this.estaciones;
    const f = this.filtro.toLowerCase();
    return this.estaciones.filter(e =>
      e.codigo.toLowerCase().includes(f) ||
      e.nombre.toLowerCase().includes(f) ||
      (e.ubicacion && e.ubicacion.toLowerCase().includes(f))
    );
  }

  crearEstacion(): void {
    if (!this.nuevaEstacion.codigo || !this.nuevaEstacion.nombre) {
      Swal.fire('Atención', 'El código y nombre son obligatorios.', 'warning');
      return;
    }

    this.transporteService.crearEstacion(this.nuevaEstacion).subscribe({
      next: (estacion) => {
        this.estaciones.push(estacion);
        this.nuevaEstacion = { codigo: '', nombre: '', ubicacion: '', activa: true };
        this.mostrarFormulario = false;
        Swal.fire('¡Éxito!', `Estación "${estacion.nombre}" creada correctamente.`, 'success');
      },
      error: (err) => {
        Swal.fire('Error', err.error?.mensaje || 'No se pudo crear la estación.', 'error');
      }
    });
  }

  cancelar(): void {
    this.mostrarFormulario = false;
    this.nuevaEstacion = { codigo: '', nombre: '', ubicacion: '', activa: true };
  }
}
