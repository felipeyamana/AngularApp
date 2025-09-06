import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { environment } from '../../../environments/environment';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: 'login.html',
  styleUrl: 'login.scss'
})
export class Login {
  email: string = '';
  password: string = '';
  successMessage: string = '';
  errorMessage: string = '';

  private apiUrl = `${environment.apiUrl}/auth/login`;

  constructor(
    private http: HttpClient,
    private router: Router,
    private userService: UserService
  ) {}

  onSubmit(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.email || !this.password) {
      this.errorMessage = 'Both fields are required!';
      return;
    }

    this.http.post<{ token: string }>(this.apiUrl, {
      email: this.email,
      password: this.password
    }).subscribe({
      next: (response) => {
        this.successMessage = 'Login successful';
        localStorage.setItem('auth_token', response.token);
        console.log('[Login] Got token:', response.token);

        this.userService.loadCurrentUser().subscribe({
          next: () => {
            this.router.navigate(['/dashboard']); // static redirect to dashboard after storing user
          }
        });
      },
      error: (err) => {
        this.errorMessage = err.error?.errors?.[0] || 'Login failed.';
      }
    });
  }
}
