import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { UserService, User } from '../../services/user.service';
import { AsyncPipe, NgIf, NgFor } from '@angular/common';
import { NgbDropdownModule, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { NotificationHubService } from '../../core/realtime/notification-hub.service';
import { AppNotification } from '../../models/app-notification.model';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, AsyncPipe, NgIf, NgFor, NgbTooltipModule, NgbDropdownModule, DatePipe],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class Navbar implements OnInit {
  notifications: AppNotification[] = [];

  constructor(private router: Router, private userService: UserService, private notificationHub: NotificationHubService) {}

  get currentUser$() {
    return this.userService.currentUser$;
  }

  ngOnInit(): void {
    this.userService.loadCurrentUser().subscribe({
      next: (user) => console.log('[Navbar] Got user from API:', user),
      error: (err) => console.error('[Navbar] Error fetching user:', err)
    });

    this.notificationHub.notifications$
      .subscribe(notification => {
        // newest on top
        this.notifications.unshift(notification);

        if (this.notifications.length > 5) {
          this.notifications.pop();
        }
      });
  }

  get unreadCount(): number {
    return this.notifications.length;
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

