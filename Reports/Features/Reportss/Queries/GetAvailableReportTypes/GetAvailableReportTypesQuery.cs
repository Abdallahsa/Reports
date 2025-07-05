using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Service.ReportService;

namespace Reports.Features.Reportss.Queries.GetAvailableReportTypes
{
    public class GetAvailableReportTypesQuery : ICommand<List<string>>
    {
    }

    public class GetAvailableReportTypesQueryHandler(
        IUserReportService _userReportService
    ) : ICommandHandler<GetAvailableReportTypesQuery, List<string>>
    {
        public Task<List<string>> Handle(GetAvailableReportTypesQuery request, CancellationToken cancellationToken)
        {

            var availableReports = _userReportService.GetAllowedReportsForCurrentUser()
                .Select(report => report.ToArabic());

            return Task.FromResult(availableReports.ToList());

        }
    }

}
