using Reports.Api.Domain.Entities;
using Reports.Domain.Primitives;

namespace Reports.Domain.Entities
{
    public class Report : BaseEntity
    {
        public string GehaCode { get; set; } = string.Empty; // مثل "NRA", "LM", ...
        public string ShoabaName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ReportType ReportType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsApprovedByRA { get; set; }
        public bool IsRejected { get; set; } = false; // نعرف هل التقرير اترفض في أي مرحلة
        public Level CurrentApprovalLevel { get; set; } // المستوى الحالي اللي بيوافق

        public string FilePath { get; set; } = string.Empty;
        public FileStatus Status { get; set; } = FileStatus.Locked;

        public ICollection<ReportApproval> Approvals { get; set; } = new List<ReportApproval>();
    }
    public enum FileStatus
    {
        Locked = 0,
        Unlocked = 1
    }

    public enum ReportType
    {
        // تقرير المنوبين اليومى
        DailyDeputyReport = 0,

        // تقرير العمليات اليومى
        DailyOperationsReport = 1,

        // تقرير لواء منوب
        BrigadeDeputyReport = 2,

        // تقرير نائب رئيس الأركان المنوب
        ChiefOfStaffDeputyReport = 3,

        // تقرير مساعد لواء منوب الدفاع الجوي (طوارئ)
        AirDefenseEmergencyDeputyReport = 4
    }


}
