using AngularApp.Realtime.Hubs;
using Application.Chats.Dtos;
using Application.Chats.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AngularApp.Realtime.Publishers
{
    public class SignalRChatPublisher : IChatRealTimePublisher
    {
        private readonly IHubContext<ChatHub> _hub;

        public SignalRChatPublisher(IHubContext<ChatHub> hub)
        {
            _hub = hub;
        }

        public async Task PublishMessageAsync(Guid chatId, ChatMessageDto message)
        {
            await _hub.Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", message);
        }
    }
}
