using Reports.Domain.Primitives;

namespace Reports.Domain.Entities
{
    public class ReportPath : BaseEntity
    {
        public int ReportId { get; set; }
        public string Role { get; set; } = string.Empty; // "NRA", "LM", "RO", "RA"
        public string FilePath { get; set; } = string.Empty;

        public Report? Report { get; set; }
    }
}
