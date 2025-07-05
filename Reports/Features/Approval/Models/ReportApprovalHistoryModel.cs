using Reports.Features.Reportss.Model;

namespace Reports.Features.Approval.Models
{
    public class ReportApprovalHistoryModel : GetAllReportModel
    {

        public ICollection<GetReportApprovalModel> Approvals { get; set; } = new List<GetReportApprovalModel>();

    }
}
