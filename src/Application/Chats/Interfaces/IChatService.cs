using Application.Chats.Dtos;

namespace Application.Chats.Interfaces
{
    public interface IChatService
    {
        Task<Guid> CreateChatAsync(List<string> participantIds);

        Task<List<ChatDto>> GetUserChatsAsync(string userId);

        Task<List<ChatMessageDto>> GetChatMessagesAsync(Guid chatId, int page);

        Task<ChatMessageDto> SendMessageAsync(Guid chatId, string senderId, string content);

        Task MarkMessagesAsReadAsync(Guid chatId, string userId);
    }
}
