using Reports.Api.Domain.Entities;
using Reports.Domain.Primitives;

namespace Reports.Domain.Entities
{
    public class ReportApproval : BaseEntity
    {
        public int ReportId { get; set; }
        public int UserId { get; set; }
        public required string Geha { get; set; }  // "NRA", "LM", "RO", "RA"

        // public bool IsApproved { get; set; } = false;
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
        public DateTime? ApprovalDate { get; set; }

        public Report? Report { get; set; }
        public User? User { get; set; }
    }

    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled
    }
}
