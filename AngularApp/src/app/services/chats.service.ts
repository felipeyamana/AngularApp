import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface ChatMessageDto {
  id: string;
  chatId: string;
  senderId: string;
  content: string;
  createdAt: string;
  isRead: boolean;
}

export interface ChatParticipantDto {
  userId: string;
  name: string;
}

export interface ChatDto {
  id: string;
  createdAt: string;
  participants: ChatParticipantDto[];
  lastMessage?: string;
  lastMessageAt?: string;
  unreadCount: number;
}

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  private apiUrl = `${environment.apiUrl}/chat`;

  constructor(private http: HttpClient) { }

  createChat(participants: string[]): Observable<string> {
    return this.http.post<string>(this.apiUrl, participants);
  }

  getMyChats(): Observable<ChatDto[]> {
    return this.http.get<ChatDto[]>(`${this.apiUrl}/my`);
  }

  getMessages(chatId: string, page: number = 1) {
    return this.http.get<ChatMessageDto[]>(
      `${this.apiUrl}/${chatId}/messages?page=${page}`,
      { withCredentials: true }
    );
  }

  sendMessage(chatId: string, content: string) {
    return this.http.post<ChatMessageDto>(
      `${this.apiUrl}/${chatId}/messages`,
      { content },
      { withCredentials: true }
    );
  }

  markAsRead(chatId: string) {
    return this.http.post(
      `${this.apiUrl}/${chatId}/read`,
      {},
      { withCredentials: true }
    );
  }
}