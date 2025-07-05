namespace Reports.Features.Reportss.Model
{
    public class GetReportApprovalModel
    {
        public int Id { get; set; }
        public string Geha { get; set; } = string.Empty; // "NRA", "LM", "RO", "RA"
        public string ApprovalStatus { get; set; } = string.Empty; // "Pending", "Approved", "Rejected", "Cancelled"
        public DateTime? ApprovalDate { get; set; }
    }

}
