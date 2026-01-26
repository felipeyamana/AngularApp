import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { environment } from '../../../environments/environment';
import { UserService } from '../../services/user.service';

declare var google: any;

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: 'login.html',
  styleUrl: 'login.scss'
})

export class Login implements OnInit {
  // shared fields
  email: string = '';
  password: string = '';
  successMessage: string = '';
  errorMessage: string = '';
  isLoading: boolean = false;
  isRegisterMode: boolean = false;

  // registration form fields
  firstName: string = '';
  lastName: string = '';

  private returnUrl: string = '/dashboard'; // default fallback
  private apiUrl = `${environment.apiUrl}/auth`;

  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private userService: UserService
  ) { }

  ngOnInit(): void {
    console.log('Login component initialized');

    this.waitForGoogle()
      .then((google) => {
        console.log('Google SDK ready');

        google.accounts.id.initialize({
          client_id: environment.googleClientId,
          callback: (response: any) => this.handleCredentialResponse(response)
        });

        google.accounts.id.renderButton(
          document.getElementById('googleBtn'),
          {
            theme: 'outline',
            size: 'large',
            width: '500'
          }
        );
      })
      .catch(err => {
        console.error(err);
      });
  }


  onSubmitLogin(): void {
    this.resetMessages();
    this.isLoading = true;

    if (!this.email || !this.password) {
      this.errorMessage = 'Both fields are required!';
      this.isLoading = false;
      return;
    }

    this.http.post<{ token: string }>(`${this.apiUrl}/login`, {
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

  onSubmitRegister(): void {
    this.resetMessages();
    this.isLoading = true;

    if (!this.firstName || !this.lastName || !this.email || !this.password) {
      this.errorMessage = 'All fields are required!';
      this.isLoading = false;
      return;
    }

    const payload = {
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email,
      password: this.password
    };

    this.http.post(`${this.apiUrl}/register`, payload).subscribe({
      next: () => {
        this.successMessage = 'Account created successfully! You can now log in.';
        this.isLoading = false;
        this.isRegisterMode = false;
        this.clearForm();
      },
      error: (err) => {
        console.log(err);
        this.errorMessage = err.error?.errors?.[0] || 'Registration failed.';
        this.isLoading = false;
      }
    });
  }

  onGoogleLogin(): void {
    console.log('starting google login');
    google.accounts.id.prompt();
  }

  private handleCredentialResponse(response: any): void {
    if (!response?.credential) {
      console.error('No Google credential returned');
      return;
    }

    this.resetMessages();
    this.isLoading = true;

    this.loginWithGoogle(response.credential);
  }

  private loginWithGoogle(idToken: string): void {
    this.http.post<{ token: string }>(
      `${this.apiUrl}/google`,
      { token: idToken }
    ).subscribe({
      next: (response) => {
        this.successMessage = 'Login successful';

        console.log('[GoogleLogin] Got token:', response.token);

        this.userService.loadCurrentUser().subscribe({
          next: () => {
            this.isLoading = false;
            this.router.navigate([this.returnUrl]);
          },
          error: () => {
            this.errorMessage = 'Failed to load user profile';
            this.isLoading = false;
          }
        });
      },
      error: (err) => {
        this.errorMessage =
          err.error?.errors?.[0] || 'Google login failed.';
        this.isLoading = false;
      }
    });
  }


  private waitForGoogle(): Promise<any> {
    return new Promise((resolve, reject) => {
      const interval = setInterval(() => {
        if ((window as any).google?.accounts?.id) {
          clearInterval(interval);
          resolve((window as any).google);
        }
      }, 50);

      setTimeout(() => {
        clearInterval(interval);
        reject('Google Identity Services failed to load');
      }, 5000);
    });
  }

  // --- Helpers ---
  private resetMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  private clearForm(): void {
    this.firstName = '';
    this.lastName = '';
    this.email = '';
    this.password = '';
  }
}
