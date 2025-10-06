import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { UserService, User } from '../../services/user.service';
import { AsyncPipe, NgIf } from '@angular/common';
import { NgbDropdownModule, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, AsyncPipe, NgIf, NgbTooltipModule, NgbDropdownModule],
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

  onLogout(event?: Event): void {
    this.userService.logout().subscribe({
      next: () => {
        console.log('[Navbar] Logged out');
        this.userService.clearCurrentUser();
        this.router.navigate(['/login']);
      },
      error: (err) => console.error('[Navbar] Logout failed', err)
    });
  }

  onProfileClick(event?: Event): void {
    this.router.navigate(['/user-profile']);
  }

  onSettingsClick(event?: Event): void {
    this.router.navigate(['/settings']);
  }

  onLogsClick(event?: Event): void {
    event?.preventDefault();
    this.router.navigate(['/logs']);
  }
}

