using Reports.Domain.Entities;

namespace Reports.Api.Services.Notifications
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string title, string content, int receiverId, NotificationType type, int? senderId = null, CancellationToken cancellationToken = default);
    }
}
