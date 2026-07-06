import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TransporteService } from '../../services/transporte.service';
import { ReporteTrafico } from '../../models/reporte-trafico.model';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-reportes',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reportes.component.html',
  styleUrl: './reportes.component.scss'
})
export class ReportesComponent implements OnInit {
  reporte: ReporteTrafico | null = null;
  cargando = true;

  constructor(private transporteService: TransporteService) {}

  ngOnInit(): void {
    this.cargarReporte();
  }

  cargarReporte(): void {
    this.cargando = true;
    this.transporteService.getReporteTrafico().subscribe({
      next: (data) => {
        this.reporte = data;
        this.cargando = false;
      },
      error: () => {
        this.cargando = false;
        Swal.fire('Error', 'No se pudo generar el reporte.', 'error');
      }
    });
  }

  getBarWidth(conexiones: number): number {
    if (!this.reporte || this.reporte.estacionesMasConectadas.length === 0) return 0;
    const max = this.reporte.estacionesMasConectadas[0].numeroConexiones;
    return max > 0 ? (conexiones / max) * 100 : 0;
  }
}
