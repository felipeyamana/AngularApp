import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotificationHubService {

  private hubConnection!: signalR.HubConnection;

  private notificationsSubject = new Subject<any>();
  notifications$ = this.notificationsSubject.asObservable();

  start(): void {
    if (this.hubConnection) return; // prevent double start

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:44307/hubs/notifications', {withCredentials: true})
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on(
      'NotificationReceived',
      (notification) => {
        console.log('SignalR notification:', notification);
        this.notificationsSubject.next(notification);
      }
    );

    this.hubConnection
      .start()
      .catch(err => console.error('SignalR error:', err));
  }
}
