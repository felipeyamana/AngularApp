import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {
  email: string = '';
  password: string = '';
  errorMessage: string = '';
  successMessage: string = '';

  // Inject HttpClient directly
  constructor(private http: HttpClient, private router: Router) {}

  onSubmit(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.email || !this.password) {
      this.errorMessage = 'Both fields are required!';
      return;
    }

    this.http.post<{ token: string }>('https://localhost:44307/api/auth/login', {
      email: this.email,
      password: this.password
    }).subscribe({
      next: (response) => {
        localStorage.setItem('auth_token', response.token); // cache jwt for now
        this.successMessage = 'Login successful!';
        console.log('JWT token:', response.token);

        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.errorMessage = err.error?.errors?.[0] || 'Login failed.';
      }
    });
  }
}
