import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Navbar } from '../../components/navbar/navbar';
import { ChatService, ChatDto } from '../../services/chats.service';
import { ActivatedRoute } from '@angular/router';
import { UserService } from '../../services/user.service';
import { switchMap } from 'rxjs/operators';
import { ChangeDetectorRef } from '@angular/core';
import { map } from 'rxjs/operators';
import { ChatHubService } from '../../core/realtime/chat-hub.service';
import { ChatMessageDto } from '../../services/chats.service';

interface Chat {
  id: string;
  name: string;
  lastMessage: string;
  time: string;
  unreadCount: number;
}

type MessageStatus = 'sending' | 'sent' | 'read' | 'failed';

interface Message {
  id?: string;
  sender: string;
  content: string;
  time: string;
  isMine: boolean;
  status?: MessageStatus;
}

@Component({
  selector: 'app-chat-page',
  standalone: true,
  imports: [CommonModule, Navbar, FormsModule],
  templateUrl: './chat-page.component.html',
  styleUrls: ['./chat-page.component.scss']
})
export class ChatPageComponent {
  skeletons = Array(8);
  targetUserId: string | null = null;
  isLoadingChats = false;

  newMessage: string = '';

  selectedChat: Chat | null = null;

  chats: Chat[] = [];

  messages: Message[] = [];


  constructor(private route: ActivatedRoute,
    private chatService: ChatService,
    private userService: UserService,
    private cdr: ChangeDetectorRef,
    private chatHub: ChatHubService) { }

  ngOnInit() {

    this.route.queryParams.subscribe(params => {
      this.targetUserId = params['userId'] || null;

      if (this.targetUserId) {
        this.initializeChat();
      } else {
        this.loadChats();
      }
    });

    // subscrice to read events
    this.chatHub.messagesRead$.subscribe(({ chatId, userId }) => {

      if (this.selectedChat?.id !== chatId) return;

      const currentUser = this.userService.currentUser;

      // ignore own read event
      if (userId === currentUser?.id) return;

      this.messages.forEach(m => {
        if (m.isMine) {
          m.status = 'read';
        }
      });

      this.cdr.detectChanges();
    });

    // subscribe to message events
    this.chatHub.messages$.subscribe(message => {

      const currentUser = this.userService.currentUser;

      // always update sidebar
      this.updateChatPreview(message);

      // ignore own messages
      if (message.senderId === currentUser?.id) return;

      // if chat is open update messages
      if (this.selectedChat?.id === message.chatId) {

        this.messages.push({
          id: message.id,
          sender: message.senderId,
          content: message.content,
          time: new Date(message.createdAt).toLocaleTimeString(),
          isMine: false
        });

        this.scrollToBottom();

        this.chatService.markAsRead(message.chatId).subscribe();
      }

      this.cdr.detectChanges();
    });
  }

  private updateChatPreview(message?: ChatMessageDto) {

    if (!message) return;

    if (!message.senderId) return;

    const currentUser = this.userService.currentUser;

    const chatIndex = this.chats.findIndex(c => c.id === message.chatId);
    if (chatIndex === -1) return;

    const chat = this.chats[chatIndex];

    chat.lastMessage = message.content.length > 40
      ? message.content.substring(0, 40) + '...'
      : message.content;

    chat.time = new Date(message.createdAt).toLocaleTimeString();

    const isMine = message.senderId === currentUser?.id;
    const isChatOpen = this.selectedChat?.id === message.chatId;

    if (!isMine && !isChatOpen) {
      // increment unread
      chat.unreadCount = (chat.unreadCount || 0) + 1;
    }

    if (isChatOpen) {
      // reset unread if chat is open
      chat.unreadCount = 0;
    }

    // move chat to the top
    this.chats.splice(chatIndex, 1);
    this.chats.unshift(chat);
  }

  initializeChat() {
    this.userService.getCurrentUserCached().pipe(
      switchMap(user => {
        if (!user || !this.targetUserId) {
          // replace this with a proper error feedback to the user later
          throw new Error('Missing user');
        }

        return this.chatService.createChat([
          user.id,
          this.targetUserId
        ]);
      })
    ).subscribe({
      next: (chatId) => {
        console.log('Chat ready:', chatId);
        this.loadChats();
      },
      error: (err) => console.error(err)
    });
  }

  sendMessage() {
    if (!this.newMessage.trim() || !this.selectedChat) return;

    const content = this.newMessage;
    this.newMessage = ''; // clear immediately

    console.log('clearing current message');
    console.log(this.newMessage);

    const tempId = crypto.randomUUID(); // temporary ID

    const currentUser = this.userService.currentUser;

    // immediate frontend feedback (no need to wait for backend to actually receive and process the message)
    const tempMessage = {
      id: tempId,
      sender: currentUser?.id!,
      content,
      time: new Date().toLocaleTimeString(),
      isMine: true,
      status: 'sending' as MessageStatus
    };

    this.messages.push(tempMessage);
    this.cdr.detectChanges();
    this.scrollToBottom();

    this.chatService.sendMessage(this.selectedChat.id, content).subscribe({
      next: (message) => {
        // replace temp message
        const index = this.messages.findIndex(m => m.id === tempId);

        if (index !== -1) {
          this.messages[index] = {
            id: message.id,
            sender: message.senderId,
            content: message.content,
            time: new Date(message.createdAt).toLocaleTimeString(),
            isMine: true,
            status: 'sent'
          };
        }

        this.updateChatPreview({
          chatId: this.selectedChat?.id,
          content,
          createdAt: new Date().toISOString(),
          senderId: currentUser?.id
        } as ChatMessageDto);

        this.cdr.detectChanges();
      },
      error: () => {
        const msg = this.messages.find(m => m.id === tempId);
        if (msg) msg.status = 'failed';

        this.cdr.detectChanges();
      }
    });
  }

  loadChats() {
    this.isLoadingChats = true;

    this.userService.getCurrentUserCached().pipe(
      switchMap(currentUser => {
        if (!currentUser) throw new Error('No user');

        return this.chatService.getMyChats().pipe(
          map(chats => ({ chats, currentUser }))
        );
      })
    ).subscribe({
      next: ({ chats, currentUser }) => {

        this.chats = chats
          .sort((a, b) => {
            const dateA = a.lastMessageAt || a.createdAt;
            const dateB = b.lastMessageAt || b.createdAt;
            return new Date(dateB).getTime() - new Date(dateA).getTime();
          })
          .map(chat => {
            const otherUser = chat.participants.find(p => p.userId !== currentUser.id);

            return {
              id: chat.id,
              name: otherUser?.name || 'Unknown',
              lastMessage: chat.lastMessage || 'No messages yet',
              time: chat.lastMessageAt
                ? new Date(chat.lastMessageAt).toLocaleTimeString()
                : '',
              unreadCount: chat.unreadCount
            };
          });

        this.isLoadingChats = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err);
        this.isLoadingChats = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadMessages(chatId: string) {
    this.chatService.getMessages(chatId).subscribe({
      next: (messages) => {

        const currentUser = this.userService.currentUser;

        this.messages = messages.reverse().map(m => ({
          id: m.id,
          sender: m.senderId,
          content: m.content,
          time: new Date(m.createdAt).toLocaleTimeString(),
          isMine: m.senderId === currentUser?.id,
          status: m.senderId === currentUser?.id
            ? (m.isRead ? 'read' : 'sent')
            : undefined
        }));

        this.cdr.detectChanges();
        this.scrollToBottom();
      },
      error: (err) => console.error(err)
    });
  }

  createChatWith(userId: string) {
    this.userService.getCurrentUserCached().pipe(
      switchMap(user => {
        if (!user) throw new Error('No current user');

        return this.chatService.createChat([
          user.id,
          userId
        ]);
      })
    ).subscribe({
      next: () => this.loadChats(),
      error: (err) => console.error(err)
    });
  }

  selectChat(chat: Chat) {
    this.selectedChat = chat;

    chat.unreadCount = 0;

    this.chatHub.joinChat(chat.id);

    this.loadMessages(chat.id);

    this.chatService.markAsRead(chat.id).subscribe();
  }

  scrollToBottom() {
    setTimeout(() => {
      const container = document.querySelector('.chat-messages');
      if (container) {
        container.scrollTop = container.scrollHeight;
      }
    }, 0);
  }
}