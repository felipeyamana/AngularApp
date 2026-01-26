import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { environment } from '../../../environments/environment'

const hubUrl = environment.apiUrl.replace(/\/api$/, '') + '/hubs/notifications';

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
      .withUrl(hubUrl, {withCredentials: true, transport: signalR.HttpTransportType.WebSockets})
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
