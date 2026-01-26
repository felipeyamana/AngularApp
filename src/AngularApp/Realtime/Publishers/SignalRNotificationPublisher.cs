using AngularApp.Realtime.Hubs;
using Application.Common.Models;
using Microsoft.AspNetCore.SignalR;

namespace AngularApp.Realtime.Publishers
{
    public class SignalRNotificationPublisher
    {
        private readonly IHubContext<NotificationHub> _hub;

        public SignalRNotificationPublisher(IHubContext<NotificationHub> hub)
        {
            _hub = hub;
        }

        public async Task PublishToUserAsync(NotificationEnvelope notification)
        {
            await _hub.Clients
                .User(notification.UserId!)
                .SendAsync("NotificationReceived", notification);
        }
        public async Task NotifyAllUsersAsync(string type, object payload, string? userId = null)
        {
            await _hub.Clients.All.SendAsync("NotificationReceived", new NotificationEnvelope(type, payload));
        }
    }
}
