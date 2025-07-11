using Microsoft.EntityFrameworkCore;
using Reports.Api.Data;
using Reports.Common.Abstractions.Mediator;
using Reports.Features.SystemLogs.Models;

namespace Reports.Features.SystemLogs.Queries.GetLogsStatistics
{

    public class GetLogsStatisticsQuery : ICommand<LogsStatisticsModel>
    {
        public DateTime? StartRange { get; set; }
        public DateTime? EndRange { get; set; }
    }

    public class GetLogsStatisticsQueryHandler(AppDbContext context)
        : ICommandHandler<GetLogsStatisticsQuery, LogsStatisticsModel>
    {
        public async Task<LogsStatisticsModel> Handle(GetLogsStatisticsQuery request, CancellationToken cancellationToken)
        {
            var logsQuery = context.SystemLog.AsQueryable();

            // make filter by date range if provided if have start and end range or start range only or end range only
            if (request.StartRange.HasValue && request.EndRange.HasValue)
            {
                logsQuery = logsQuery.Where(l => l.TimeStamp >= request.StartRange.Value && l.TimeStamp <= request.EndRange.Value);
            }
            else if (request.StartRange.HasValue)
            {
                logsQuery = logsQuery.Where(l => l.TimeStamp >= request.StartRange.Value);
            }
            else if (request.EndRange.HasValue)
            {
                logsQuery = logsQuery.Where(l => l.TimeStamp <= request.EndRange.Value);
            }

            var totalErrors = await logsQuery.CountAsync(l => l.Level == "Error", cancellationToken);
            var totalWarnings = await logsQuery.CountAsync(l => l.Level == "Warning", cancellationToken);
            var totalInformation = await logsQuery.CountAsync(l => l.Level == "Information", cancellationToken);
            var totalCosts = await logsQuery.CountAsync();

            return new LogsStatisticsModel
            {
                ErrorCount = totalErrors,
                WarningCount = totalWarnings,
                InformationCount = totalInformation,
                TotalCount = totalCosts,
            };
        }
    }
}


