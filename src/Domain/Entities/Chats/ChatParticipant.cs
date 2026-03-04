namespace Domain.Entities.Chat
{
    public class ChatParticipant
    {
        public int Id { get; set; }

        public Guid ChatId { get; set; }

        public Chat Chat { get; set; } = default!;

        public string UserId { get; set; } = default!;

        public ApplicationUser User { get; set; } = default!;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
