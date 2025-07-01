namespace Reports.Features.Reportss.Model
{
    public class GetReportApprovalModel
    {
        public string Role { get; set; } = string.Empty; // "NRA", "LM", "RO", "RA"
        public bool IsApproved { get; set; } = false;
        public DateTime? ApprovalDate { get; set; }
    }

}
