using Domain.Entities.Chats;

namespace Domain.Entities.Chat
{
    public class ChatMessage
    {
        public Guid Id { get; set; }

        public Guid ChatId { get; set; }

        public Chat Chat { get; set; } = default!;

        public string SenderId { get; set; } = default!;

        public ApplicationUser Sender { get; set; } = default!;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Refers to what users viewed a specific message in a chat (currently the system only supports 1:1 chats, but it will be expanded to support group chats)
        public ICollection<ChatMessageView> Views { get; set; } = new List<ChatMessageView>();
    }
}
