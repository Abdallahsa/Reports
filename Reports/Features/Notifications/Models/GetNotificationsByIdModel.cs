namespace Reports.Features.Notifications.Models
{
    public class GetNotificationByIdModel : GetMyNotificationsModel
    {
        public string Content { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;

    }
}
