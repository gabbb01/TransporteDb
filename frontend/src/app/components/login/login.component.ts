import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  username  = '';
  password  = '';
  error     = '';
  cargando  = false;
  mostrarPassword = false;

  constructor(private authService: AuthService, private router: Router) {
    // Si ya está logueado, redirigir al inicio
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/estaciones']);
    }
  }

  login(): void {
    if (!this.username.trim() || !this.password.trim()) {
      this.error = 'Por favor ingresa tu usuario y contraseña.';
      return;
    }

    this.cargando = true;
    this.error    = '';

    this.authService.login({ username: this.username, password: this.password }).subscribe({
      next: () => {
        this.cargando = false;
        this.router.navigate(['/estaciones']);
      },
      error: (err) => {
        this.cargando = false;
        
        if (err.status === 401) {
          this.error = 'Usuario o contraseña incorrectos.';
        } else if (err.status === 0) {
          this.error = 'Error de conexión. Verifica que el servidor esté activo.';
        } else {
          this.error = err.error?.mensaje || 'Ha ocurrido un error al intentar iniciar sesión.';
        }
      }
    });
  }

  togglePassword(): void {
    this.mostrarPassword = !this.mostrarPassword;
  }
}
