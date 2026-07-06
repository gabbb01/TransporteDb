import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TransporteService } from '../../services/transporte.service';
import { Estacion } from '../../models/estacion.model';
import { ResultadoRuta } from '../../models/resultado-ruta.model';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-camino-corto',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './camino-corto.component.html',
  styleUrl: './camino-corto.component.scss'
})
export class CaminoCortoComponent implements OnInit {
  estaciones: Estacion[] = [];
  origenSeleccionado = '';
  destinoSeleccionado = '';
  resultado: ResultadoRuta | null = null;
  buscando = false;

  constructor(private transporteService: TransporteService) {}

  ngOnInit(): void {
    this.transporteService.getEstaciones().subscribe({
      next: (data) => this.estaciones = data
    });
  }

  calcular(): void {
    if (!this.origenSeleccionado || !this.destinoSeleccionado) {
      Swal.fire('Atención', 'Selecciona una estación de origen y destino.', 'warning');
      return;
    }
    if (this.origenSeleccionado === this.destinoSeleccionado) {
      Swal.fire('Atención', 'El origen y destino no pueden ser iguales.', 'warning');
      return;
    }

    this.buscando = true;
    this.resultado = null;

    this.transporteService.getCaminoCorto(this.origenSeleccionado, this.destinoSeleccionado).subscribe({
      next: (data) => {
        this.resultado = data;
        this.buscando = false;
      },
      error: (err) => {
        this.buscando = false;
        Swal.fire('Sin Ruta', err.error?.mensaje || 'No se encontró una ruta entre las estaciones.', 'info');
      }
    });
  }

  limpiar(): void {
    this.origenSeleccionado = '';
    this.destinoSeleccionado = '';
    this.resultado = null;
  }
}
