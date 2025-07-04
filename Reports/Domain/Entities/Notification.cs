using Reports.Api.Domain.Entities;
using Reports.Domain.Primitives;

namespace Reports.Domain.Entities
{
    public class Notification : BaseAuditableEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public NotificationType Type { get; set; } = NotificationType.Info;

        public int? SenderId { get; set; }
        public int ReceiverId { get; set; }
        public User? Sender { get; set; }
        public User? Receiver { get; set; }
    }

    public enum NotificationType
    {
        Info,
        Warning,
        Error,
        Success
    }

}
