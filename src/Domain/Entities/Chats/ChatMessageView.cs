using Domain.Entities.Chat;

namespace Domain.Entities.Chats
{
    public class ChatMessageView
    {
        public Guid Id { get; set; }

        public Guid ChatMessageId { get; set; }
        public ChatMessage ChatMessage { get; set; } = default!;

        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;

        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    }
}
