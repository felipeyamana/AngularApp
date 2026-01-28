using Application.Common.Models;

namespace Application.Common.Interfaces
{
    public interface INotificationPublisher
    {
        Task PublishAsync(NotificationEnvelope message);
    }
}
