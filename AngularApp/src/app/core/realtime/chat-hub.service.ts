import { Injectable, NgZone } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ChatMessageDto } from '../../services/chats.service';

const hubUrl = environment.apiUrl.replace(/\/api$/, '') + '/hubs/chat';

@Injectable({
  providedIn: 'root'
})
export class ChatHubService {

  private hubConnection!: signalR.HubConnection;

  private messageSubject = new Subject<ChatMessageDto>();
  messages$ = this.messageSubject.asObservable();

  private readSubject = new Subject<{ chatId: string; userId: string }>();
  messagesRead$ = this.readSubject.asObservable();

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

    this.hubConnection.on('ReceiveMessage', (message: ChatMessageDto) => {
      this.zone.run(() => {
        this.messageSubject.next(message);
      });
    });
    
    this.hubConnection.on('MessagesRead', (payload) => {
      this.zone.run(() => {
        this.readSubject.next(payload);
      });
    });

    this.hubConnection.start()
      .catch(err => console.error('Chat SignalR error:', err));
  }

  joinChat(chatId: string) {
    this.hubConnection.invoke('JoinChat', chatId);
  }
}