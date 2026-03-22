using Domain.Entities.Chat;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ChatParticipant> ChatParticipants { get; set; }
        = new List<ChatParticipant>();

        public ICollection<ChatMessage> SentMessages { get; set; }
            = new List<ChatMessage>();
    }
}
