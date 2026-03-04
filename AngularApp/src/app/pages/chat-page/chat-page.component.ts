import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Navbar } from '../../components/navbar/navbar';

interface Chat {
  id: string;
  name: string;
  lastMessage: string;
  time: string;
}

interface Message {
  sender: string;
  content: string;
  time: string;
  isMine: boolean;
}

@Component({
  selector: 'app-chat-page',
  standalone: true,
  imports: [CommonModule, Navbar],
  templateUrl: './chat-page.component.html',
  styleUrls: ['./chat-page.component.scss']
})
export class ChatPageComponent {

  selectedChat: Chat | null = null;

  chats: Chat[] = [
    { id: '1', name: 'Victor Yoga', lastMessage: 'You can check it...', time: 'now' },
    { id: '2', name: 'Devon Lane', lastMessage: 'I will try my best...', time: '4m' },
    { id: '3', name: 'Guy Hawkins', lastMessage: 'Okay noted bro!', time: '7m' },
    { id: '4', name: 'Kristin Watson', lastMessage: 'Nice.', time: '23m' }
  ];

  messages: Message[] = [
    {
      sender: 'Victor',
      content: 'Hey everyone! Ready for today?',
      time: '01:20 AM',
      isMine: false
    },
    {
      sender: 'You',
      content: 'Yes! Let’s start working on the design.',
      time: '01:21 AM',
      isMine: true
    }
  ];

  selectChat(chat: Chat) {
    this.selectedChat = chat;
  }
}