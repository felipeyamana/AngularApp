namespace Domain.Entities.Chat
{
    public class Chat
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string CreatedByUserId { get; set; } = default!;

        public ApplicationUser CreatedByUser { get; set; } = default!;

        public ICollection<ChatParticipant> Participants { get; set; }
            = new List<ChatParticipant>();

        public ICollection<ChatMessage> Messages { get; set; }
            = new List<ChatMessage>();
    }
}
