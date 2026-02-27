using System.Threading.Tasks;

namespace Kemora.Application.Interfaces
{
    public interface INotificationPusher
    {
        Task PushToUserAsync(string userId, string title, string message);
    }
}
