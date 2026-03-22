import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { UserService, User } from '../../services/user.service';
import { AsyncPipe, NgIf, NgFor } from '@angular/common';
import { NgbDropdownModule, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { NotificationHubService } from '../../core/realtime/notification-hub.service';
import { AppNotification } from '../../models/app-notification.model';
import { DatePipe } from '@angular/common';
import { ChatHubService } from '../../core/realtime/chat-hub.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, AsyncPipe, NgIf, NgFor, NgbTooltipModule, NgbDropdownModule, DatePipe],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class Navbar implements OnInit {
  notifications: AppNotification[] = [];
  unreadMessages = 0;

  constructor(private router: Router, private userService: UserService, private notificationHub: NotificationHubService, private chatHub: ChatHubService) { }

  get currentUser$() {
    return this.userService.currentUser$;
  }

  ngOnInit(): void {
    this.userService.loadCurrentUser().subscribe({
      next: (user) => console.log('[Navbar] Got user from API:', user),
      error: (err) => console.error('[Navbar] Error fetching user:', err)
    });

    // general notifications hub (navbar icon only here)
    this.notificationHub.notifications$
      .subscribe(notification => {
        // newest on top
        this.notifications.unshift(notification);

        if (this.notifications.length > 5) {
          this.notifications.pop();
        }
      });


    // chat notifications hub (navbar icon only here)
    this.chatHub.messages$.subscribe(message => {
      // if user is NOT on chat page count as unread
      if (!this.router.url.includes('/chat-page')) {
        this.unreadMessages++;
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

  onTeamListClick(event?: Event): void {
    event?.preventDefault();
    this.router.navigate(['/team-list']);
  }

  onMessagesClick(event?: Event): void {
    event?.preventDefault();
    this.unreadMessages = 0;
    this.router.navigate(['/chat-page']);
  }
}

