using Reports.Api.Services.CurrentUser;
using Reports.Domain.Entities;

namespace Reports.Service.ReportService
{
    public class UserReportService(ICurrentUserService _currentUserService) : IUserReportService
    {
        public List<ReportType> GetAllowedReportsForCurrentUser()
        {
            var allowedReports = new List<ReportType>();

            // لو المستخدم عنده أكتر من Role، نلف عليهم ونضيف حسب كل Role
            if (_currentUserService.Role != null)
            {
                foreach (var role in _currentUserService.Role)
                {
                    switch (role)
                    {
                        case "LevelZero":
                            allowedReports.Add(ReportType.DailyDeputyReport);
                            allowedReports.Add(ReportType.DailyOperationsReport);
                            break;

                        case "LevelOne":
                            allowedReports.Add(ReportType.BrigadeDeputyReport);
                            break;

                        case "LevelTwo":
                            allowedReports.Add(ReportType.ChiefOfStaffDeputyReport);
                            allowedReports.Add(ReportType.AirDefenseEmergencyDeputyReport);
                            break;

                        default:
                            break;
                    }
                }
            }

            return allowedReports.Distinct().ToList();
        }
    }
}
