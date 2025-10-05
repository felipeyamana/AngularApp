using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Logs
{
    public class LogType
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<Log> Logs { get; set; } = new List<Log>();
    }
}
