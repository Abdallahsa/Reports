namespace Reports.Service.ApprovalService
{
    public interface IReportApprovalService
    {
        Task ApproveReportAsync(int reportId, int userId);
        Task RejectReportAsync(int reportId, int userId);
    }
}
