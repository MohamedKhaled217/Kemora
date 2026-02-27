using Kemora.Application.Interfaces;
using Kemora.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Kemora.Api.Services
{
    public class SignalRNotificationPusher : INotificationPusher
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotificationPusher(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PushToUserAsync(string userId, string title, string message)
        {
            await _hubContext.Clients.Group($"user_{userId}")
                .SendAsync("ReceiveNotification", new { title, message, timestamp = System.DateTime.UtcNow });
        }
    }
}
