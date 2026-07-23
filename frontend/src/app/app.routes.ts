import { Routes } from '@angular/router';
import { EstacionesComponent } from './components/estaciones/estaciones.component';
import { RutasComponent } from './components/rutas/rutas.component';
import { CaminoCortoComponent } from './components/camino-corto/camino-corto.component';
import { PasajerosComponent } from './components/pasajeros/pasajeros.component';
import { ReportesComponent } from './components/reportes/reportes.component';
import { LoginComponent } from './components/login/login.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: '', redirectTo: 'estaciones', pathMatch: 'full' },
  { path: 'estaciones',  component: EstacionesComponent,  canActivate: [authGuard] },
  { path: 'rutas',       component: RutasComponent,       canActivate: [authGuard] },
  { path: 'camino-corto',component: CaminoCortoComponent, canActivate: [authGuard] },
  { path: 'pasajeros',   component: PasajerosComponent,   canActivate: [authGuard] },
  { path: 'reportes',    component: ReportesComponent,    canActivate: [authGuard] },
  { path: '**', redirectTo: 'estaciones' }
];
