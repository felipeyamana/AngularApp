using Application.Chats.Dtos;

namespace Application.Chats.Interfaces
{
    public interface IChatRealTimePublisher
    {
        Task PublishMessageAsync(Guid chatId, ChatMessageDto message);
    }
}
