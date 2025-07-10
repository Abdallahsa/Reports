namespace Reports.Features.SystemLogs.Models
{
    public class LogEntryModel
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

}
