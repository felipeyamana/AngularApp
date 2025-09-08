import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
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
export class Login implements OnInit {
  email: string = '';
  password: string = '';
  successMessage: string = '';
  errorMessage: string = '';
  private returnUrl: string = '/dashboard'; // default fallback

  private apiUrl = `${environment.apiUrl}/auth/login`;

  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private userService: UserService
  ) {}

  ngOnInit(): void {
    console.log('something on init');
  }

  isLoading: boolean = false;

  onSubmit(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.isLoading = true;

    if (!this.email || !this.password) {
      this.errorMessage = 'Both fields are required!';
      this.isLoading = false;
      return;
    }

    this.http.post<{ token: string }>(this.apiUrl, {
      email: this.email,
      password: this.password
    }).subscribe({
      next: (response) => {
        this.successMessage = 'Login successful';
        console.log('[Login] Got token:', response.token);

        this.userService.loadCurrentUser().subscribe({
          next: () => {
            this.isLoading = false;
            this.router.navigate([this.returnUrl]);
          }
        });
      },
      error: (err) => {
        this.errorMessage = err.error?.errors?.[0] || 'Login failed.';
        this.isLoading = false;
      }
    });
  }

}
