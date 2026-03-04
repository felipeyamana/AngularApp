namespace Application.Chats.Dtos
{
    public class ChatMessageDto
    {
        public Guid Id { get; set; }

        public Guid ChatId { get; set; }

        public string SenderId { get; set; } = default!;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
