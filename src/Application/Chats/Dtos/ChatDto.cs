namespace Application.Chats.Dtos
{
    public class ChatDto
    {
        public Guid Id { get; set; }

        public List<string> ParticipantIds { get; set; } = new();

        public DateTime CreatedAt { get; set; }
    }
}
