namespace Application.Chats.Dtos
{
    public class ChatDto
    {
        public Guid Id { get; set; }
        public List<ChatParticipantDto> Participants { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
    }
}
