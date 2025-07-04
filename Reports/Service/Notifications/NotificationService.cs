using Microsoft.EntityFrameworkCore;
using Reports.Api.Data;
using Reports.Api.Domain.Entities;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;


namespace Reports.Api.Services.Notifications
{
    public class NotificationService
    (
        AppDbContext _context
    ) : INotificationService
    {
        public async Task SendNotificationAsync(string title, string content, int receiverId, NotificationType type, int? senderId = null, CancellationToken cancellationToken = default)
        {
            // Check if user is approved to receive notifications
            var customer = await _context.Set<User>()
                .FirstOrDefaultAsync(x => x.Id == receiverId)
                ?? throw new NotFoundException("Customer not found");




            // Save notification to the database
            var notification = new Notification
            {
                Title = title,
                Content = content,
                ReceiverId = receiverId,
                SenderId = senderId,
                Type = type
            };

            await _context.Set<Notification>().AddAsync(notification, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);


        }

    }
}
