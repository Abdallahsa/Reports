namespace Reports.Features.Reportss.Model
{
    public class GetReportByIdModel : GetAllReportModel
    {
        public ICollection<GetReportApprovalModel> Approvals { get; set; } = new List<GetReportApprovalModel>();

    }
}
