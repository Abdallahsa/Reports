namespace Reports.Features.Reportss.Model
{
    public class GetAllReportModel
    {
        public int Id { get; set; }
        public string GehaCode { get; set; } = string.Empty; // مثل "NRA", "LM", ...
        public string ShoabaName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ReportType { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRejected { get; set; } = false;
        public string? CurrentApprovalLevel { get; set; }
        public bool IsApprovedByRA { get; set; }
        public string FilePath { get; set; } = string.Empty;
    }

}
