import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { UserService, User } from '../../services/user.service';
import { AsyncPipe, NgIf } from '@angular/common';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, AsyncPipe, NgIf],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class Navbar implements OnInit {
  constructor(private router: Router, private userService: UserService) {}

  get currentUser$() {
    return this.userService.currentUser$;
  }

  ngOnInit(): void {
    this.userService.loadCurrentUser().subscribe({
      next: (user) => console.log('[Navbar] Got user from API:', user),
      error: (err) => console.error('[Navbar] Error fetching user:', err)
    });
  }

  logout(): void {
    localStorage.removeItem('auth_token');
    this.userService.clearCurrentUser();
    this.router.navigate(['/login']);
  }
}

