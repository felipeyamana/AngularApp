using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Logs
{
    public class Log
    {
        public long Id { get; set; }

        public int LogTypeId { get; set; }
        public LogType LogType { get; set; } = null!;
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        [MaxLength(150)]
        public string Action { get; set; } = string.Empty;
        public string? Description { get; set; }
        [MaxLength(45)]
        public string? IpAddress { get; set; }
        public bool? ActionSuccess { get; set; }
        public DateTime EventDate { get; set; } = DateTime.UtcNow;
    }
}
