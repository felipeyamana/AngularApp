namespace Application.Common.Models
{
    public record NotificationEnvelope(string Type, object Payload, string? UserId = null);
}
