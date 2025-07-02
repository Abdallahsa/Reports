using Reports.Api.Services.CurrentUser;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;

namespace Reports.Features.Reportss.Queries.GetAvailableReportTypes
{
    public class GetAvailableReportTypesQuery : ICommand<List<string>>
    {
    }

    public class GetAvailableReportTypesQueryHandler(
        ICurrentUserService _currentUserService
    ) : ICommandHandler<GetAvailableReportTypesQuery, List<string>>
    {
        public Task<List<string>> Handle(GetAvailableReportTypesQuery request, CancellationToken cancellationToken)
        {
            var userLevel = _currentUserService.Level;

            var availableReports = new List<string>();

            if (userLevel == "LevelZero")
            {
                availableReports.Add(ReportType.DailyDeputyReport.ToArabic());
                availableReports.Add(ReportType.DailyOperationsReport.ToArabic());
            }
            else if (userLevel == "LevelOne")
            {
                availableReports.Add(ReportType.BrigadeDeputyReport.ToArabic());
            }
            else if (userLevel == "LevelTwo")
            {
                availableReports.Add(ReportType.ChiefOfStaffDeputyReport.ToArabic());
                availableReports.Add(ReportType.AirDefenseEmergencyDeputyReport.ToArabic());
          
            }
            else
            {
            }

            return Task.FromResult(availableReports);
        }
    }

}
