namespace Reports.Features.SystemLogs.Models
{
    public class LogsStatisticsModel
    {
        public int TotalCount { get; set; }
        public int ErrorCount { get; set; }
        public int WarningCount { get; set; }
        public int InformationCount { get; set; }
    }

}
