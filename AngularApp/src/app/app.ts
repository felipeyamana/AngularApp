import { Component, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NotificationHubService } from './core/realtime/notification-hub.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {

  protected readonly title = signal('AngularApp');

  constructor(
    private notificationHub: NotificationHubService
  ) {}

  ngOnInit(): void {
    this.notificationHub.start();
  }
}