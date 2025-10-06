namespace Application.Logs.Dtos
{
    public class LogResponseDto
    {
        public long Id { get; set; }
        public string LogTypeName { get; set; } = string.Empty;
        public string? UserFullName { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IpAddress { get; set; }
        public DateTime EventDate { get; set; }
        public bool? ActionSuccess { get; set; }
    }
}
