using Microsoft.EntityFrameworkCore;
using Reports.Api.Common.Abstractions.Collections;
using Reports.Api.Data;
using Reports.Api.Domain.Entities;
using Reports.Api.Services.CurrentUser;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;
using Reports.Features.Reportss.Model;
using Reports.Features.Reportss.Queries.GetMyApprovedReports;
using Reports.Service.ReportService;
using TwoHO.Api.Extensions;

namespace Reports.Features.Reportss.Queries.GetAllReport
{
    public class GetAllReportQuery : GetMyApprovedReportsQuery
    {
        public bool Archive { get; set; } = false;

    }

    //Create handler for GetAllReportQuery

    public class GetAllReportQueryHandler(
     AppDbContext context,
     IUserReportService _userReportService,
     ICurrentUserService _currentUserService
 ) : ICommandHandler<GetAllReportQuery, PagedList<GetAllReportModel>>
    {
        public async Task<PagedList<GetAllReportModel>> Handle(GetAllReportQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var allowedFields = new List<string> { "Id", "reportType", "GehaCode", "ShoabaName", "Description" };
                var allowedSorting = new List<string> { "Id", "CreatedAt" };

                request.ValidateFiltersAndSorting(allowedFields, allowedSorting);

                // 🧑‍💻 Get current user
                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.Id == _currentUserService.UserId, cancellationToken)
                    ?? throw new NotFoundException(nameof(User), _currentUserService.UserId);

                // Get allowed reports for current user
                var allowedReports = _userReportService.GetAllowedReportsForCurrentUser();

                //allowedReports.Contains(r.ReportType) &&

                var queryAll = context.Reports
                    .Where(r => r.IsApprovedByRA == request.Archive);

                // لو LevelZero → نرجع فقط التقارير من نوعين معينين
                if (user.Level == Level.LevelZero)
                {
                    queryAll = queryAll.Where(r => r.ReportType == ReportType.DailyDeputyReport || r.ReportType == ReportType.DailyOperationsReport);
                }


                // Apply search filter if provided
                if (!string.IsNullOrEmpty(request.Search))
                {
                    queryAll = queryAll.Where(r =>
                        r.GehaCode.Contains(request.Search) ||
                        r.ShoabaName.Contains(request.Search) ||
                        r.Description.Contains(request.Search));
                }

                // Filter by reportType
                if (request.Filters != null && request.Filters.TryGetValue("reportType", out string? reportType))
                {
                    if (Enum.TryParse<ReportType>(reportType, true, out var type))
                    {
                        queryAll = queryAll.Where(r => r.ReportType == type);
                        request.Filters.Remove("reportType"); // remove it after use
                    }
                    else
                    {
                        throw new BadRequestException("Invalid reportType value.");
                    }
                }

                var query = queryAll
                    .Select(r => new GetAllReportModel
                    {
                        Id = r.Id,
                        ReportType = r.ReportType.ToArabic(),
                        GehaCode = r.GehaCode,
                        ShoabaName = r.ShoabaName,
                        Description = r.Description,
                        CreatedAt = r.CreatedAt,
                        CurrentApprovalLevel = r.CurrentApprovalLevel.ToString(),
                        IsApprovedByRA = r.IsApprovedByRA,
                        IsRejected = r.IsRejected,
                        FilePath = r.FilePath,
                    })
                    .ApplyFilters(request.Filters)
                    .ApplyDateRangeFilter(request.StartRange, request.EndRange)
                    .ApplySorting(request.SortBy, request.SortDirection)
                    .AsQueryable();

                // Paginate
                var result = await PagedList<GetAllReportModel>.CreateAsync(query, request.PageNumber, request.PageSize, cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }
    }

}
