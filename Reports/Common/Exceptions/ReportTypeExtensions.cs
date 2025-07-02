using Reports.Domain.Entities;

namespace Reports.Common.Exceptions
{
    public static class ReportTypeExtensions
    {
        private static readonly Dictionary<ReportType, string> ArabicNames = new()
        {
            { ReportType.DailyDeputyReport, "تقرير المنوبين اليومى" },
            { ReportType.DailyOperationsReport, "تقرير العمليات اليومى" },
            { ReportType.BrigadeDeputyReport, "تقرير لواء منوب" },
            { ReportType.ChiefOfStaffDeputyReport, "تقرير نائب رئيس الأركان المنوب" },
            { ReportType.AirDefenseEmergencyDeputyReport, "تقرير مساعد لواء منوب الدفاع الجوي (طوارئ)" }
        };

        public static string ToArabic(this ReportType reportType)
        {
            return ArabicNames.TryGetValue(reportType, out var arabic)
                ? arabic
                : "تقرير غير معرف";
        }
    }
}
