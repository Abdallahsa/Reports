namespace Reports.Features.Notifications.Models
{
    public class GetMyNotificationsModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; } = false;
    }
}
