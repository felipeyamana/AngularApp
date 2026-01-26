import { Injectable, NgZone } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { environment } from '../../../environments/environment'
import { AppNotification } from '../../models/app-notification.model';

const hubUrl = environment.apiUrl.replace(/\/api$/, '') + '/hubs/notifications';

@Injectable({
  providedIn: 'root'
})
export class NotificationHubService {

  private hubConnection!: signalR.HubConnection;

  private notificationsSubject = new Subject<AppNotification>();
  notifications$ = this.notificationsSubject.asObservable();

  constructor(private zone: NgZone) {}

  start(): void {
    if (this.hubConnection) return;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        withCredentials: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('NotificationReceived', (notification: any) => {
      const mapped: AppNotification = {
        type: notification.type,
        payload: notification.payload,
        receivedAt: new Date()
      };

      this.zone.run(() => {
        this.notificationsSubject.next(mapped);
      });
    });

    this.hubConnection
      .start()
      .catch(err => console.error('SignalR error:', err));
  }
}
