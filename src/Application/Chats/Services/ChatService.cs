using Application.Chats.Dtos;
using Application.Chats.Interfaces;
using Domain.Entities.Chat;
using Domain.Entities.Chats;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Chats.Services
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;
        private readonly IChatRealTimePublisher _publisher;
        public ChatService(ApplicationDbContext context, IChatRealTimePublisher publisher)
        {
            _context = context;
            _publisher = publisher;
        }

        public async Task<Guid> CreateChatAsync(List<string> participantIds)
        {
            participantIds = participantIds.Distinct().OrderBy(x => x).ToList();

            var existingChat = await _context.Chats
                .Where(c => c.Participants.Count == participantIds.Count)
                .Where(c => c.Participants.All(p => participantIds.Contains(p.UserId)))
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            if (existingChat != Guid.Empty)
                return existingChat;

            var chatDate = DateTime.UtcNow;

            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                CreatedAt = chatDate,
                CreatedByUserId = participantIds.First()
            };

            _context.Chats.Add(chat);

            foreach (var userId in participantIds)
            {
                _context.ChatParticipants.Add(new ChatParticipant
                {
                    ChatId = chat.Id,
                    UserId = userId,
                    JoinedAt = chatDate
                });
            }

            await _context.SaveChangesAsync();

            return chat.Id;
        }

        public async Task<ChatMessageDto> SendMessageAsync(Guid chatId, string senderId, string content)
        {
            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                ChatId = chatId,
                SenderId = senderId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(message);

            await _context.SaveChangesAsync();

            var dto = new ChatMessageDto
            {
                Id = message.Id,
                ChatId = chatId,
                SenderId = senderId,
                Content = content,
                CreatedAt = message.CreatedAt
            };

            await _publisher.PublishMessageAsync(chatId, dto);

            return dto;
        }

        public async Task<List<ChatDto>> GetUserChatsAsync(string userId)
        {
            return await _context.ChatParticipants
                .Where(p => p.UserId == userId)
                .Select(p => new ChatDto
                {
                    Id = p.ChatId,
                    CreatedAt = p.Chat.CreatedAt,
                    Participants = p.Chat.Participants
                        .Select(x => new ChatParticipantDto
                        {
                            UserId = x.UserId,
                            Name = $"{x.User.FirstName} {x.User.LastName}".Trim()
                        }).ToList(),
                    LastMessage = p.Chat.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => m.Content)
                    .FirstOrDefault(),

                    LastMessageAt = p.Chat.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => (DateTime?)m.CreatedAt)
                    .FirstOrDefault(),

                    UnreadCount = p.Chat.Messages
                    .Count(m =>
                        m.SenderId != userId &&
                        !m.Views.Any(v => v.UserId == userId)
                    )
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ChatMessageDto>> GetChatMessagesAsync(Guid chatId, int page)
        {
            const int pageSize = 50;

            return await _context.ChatMessages
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new ChatMessageDto
                {
                    Id = m.Id,
                    ChatId = m.ChatId,
                    SenderId = m.SenderId,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt,
                    IsRead = m.Views.Any()
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task MarkMessagesAsReadAsync(Guid chatId, string userId)
        {
            // get messages NOT sent by this user AND not yet viewed
            var messagesToMark = await _context.ChatMessages
                .Where(m => m.ChatId == chatId && m.SenderId != userId)
                .Where(m => !m.Views.Any(v => v.UserId == userId))
                .Select(m => m.Id)
                .ToListAsync();

            if (!messagesToMark.Any())
                return;

            var views = messagesToMark.Select(messageId => new ChatMessageView
            {
                Id = Guid.NewGuid(),
                ChatMessageId = messageId,
                UserId = userId,
                ViewedAt = DateTime.UtcNow
            });

            _context.ChatMessageViews.AddRange(views);

            await _context.SaveChangesAsync();

            await _publisher.PublishMessagesReadAsync(chatId, userId);
        }
    }
}
