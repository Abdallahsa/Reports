using Reports.Domain.Entities;

namespace Reports.Features.Reportss.Model
{
    public class GetAllReportModel
    {
        public int Id { get; set; }
        public string GehaCode { get; set; } = string.Empty; // مثل "NRA", "LM", ...
        public string ShoabaName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ReportType ReportType { get; set; }
        public DateTime CreatedAt { get; set; } 
        public bool IsApprovedByRA { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public ICollection<GetReportApprovalModel>? Approvals { get; set; }
    }

}
