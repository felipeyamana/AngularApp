import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class Navbar {
  constructor(private router: Router) {}

  // maybe do more on logout ?
  logout(): void {
    localStorage.removeItem('auth_token'); // clear JWT
    this.router.navigate(['/login']); // redirect to login
  }
}
