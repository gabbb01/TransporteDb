import { Routes } from '@angular/router';
import { EstacionesComponent } from './components/estaciones/estaciones.component';
import { RutasComponent } from './components/rutas/rutas.component';
import { CaminoCortoComponent } from './components/camino-corto/camino-corto.component';
import { PasajerosComponent } from './components/pasajeros/pasajeros.component';
import { ReportesComponent } from './components/reportes/reportes.component';

export const routes: Routes = [
  { path: '', redirectTo: 'estaciones', pathMatch: 'full' },
  { path: 'estaciones', component: EstacionesComponent },
  { path: 'rutas', component: RutasComponent },
  { path: 'camino-corto', component: CaminoCortoComponent },
  { path: 'pasajeros', component: PasajerosComponent },
  { path: 'reportes', component: ReportesComponent },
  { path: '**', redirectTo: 'estaciones' }
];
