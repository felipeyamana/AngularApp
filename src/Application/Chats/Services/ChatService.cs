using Application.Chats.Dtos;
using Application.Chats.Interfaces;
using Domain.Entities.Chat;
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
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = participantIds.First()
            };

            _context.Chats.Add(chat);

            foreach (var userId in participantIds)
            {
                _context.ChatParticipants.Add(new ChatParticipant
                {
                    ChatId = chat.Id,
                    UserId = userId
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
                    ParticipantIds = p.Chat.Participants.Select(x => x.UserId).ToList()
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
                    CreatedAt = m.CreatedAt
                })
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
