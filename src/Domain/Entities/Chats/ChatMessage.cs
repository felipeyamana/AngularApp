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
    }
}
