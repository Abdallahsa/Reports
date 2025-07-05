using Microsoft.EntityFrameworkCore;
using Reports.Api.Common.Abstractions.Collections;
using Reports.Api.Data;
using Reports.Api.Services.CurrentUser;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;
using Reports.Features.Reportss.Model;
using TwoHO.Api.Extensions;

namespace Reports.Features.Reportss.Queries.GetMyApprovedReports
{
    public class GetMyApprovedReportsQuery : HasTableViewWithDate, ICommand<PagedList<GetAllReportModel>>
    {
        public string? Search { get; set; }
    }

    public class GetMyApprovedReportsQueryHandler(
         AppDbContext _context,
         ICurrentUserService _currentUserService
       ) : ICommandHandler<GetMyApprovedReportsQuery, PagedList<GetAllReportModel>>
    {
        public async Task<PagedList<GetAllReportModel>> Handle(GetMyApprovedReportsQuery request, CancellationToken cancellationToken)
        {
            var allowedFields = new List<string> { "Id", "reportType", "GehaCode", "ShoabaName", "Description", "CreatedAt" };
            var allowedSorting = new List<string> { "Id", "CreatedAt" };

            request.ValidateFiltersAndSorting(allowedFields, allowedSorting);

            // Step 1: Get approvals for current user where status is Approved
            var approvalsQuery = _context.Approval
                .Include(a => a.Report)
                .Where(a => a.UserId == _currentUserService.UserId && a.ApprovalStatus == ApprovalStatus.Approved)
                .AsNoTracking()
                .AsQueryable();

            // Step 2: Apply search
            if (!string.IsNullOrEmpty(request.Search))
            {
                approvalsQuery = approvalsQuery.Where(a =>
                    a.Report!.GehaCode.Contains(request.Search) ||
                    a.Report.ShoabaName.Contains(request.Search) ||
                    a.Report.Description.Contains(request.Search));
            }

            // Step 3: Apply filters
            if (request.Filters != null && request.Filters.TryGetValue("reportType", out string? reportType))
            {
                if (Enum.TryParse<ReportType>(reportType, true, out var type))
                {
                    approvalsQuery = approvalsQuery.Where(a => a.Report!.ReportType == type);
                    request.Filters.Remove("reportType");
                }
                else
                {
                    throw new BadRequestException("Invalid reportType value.");
                }
            }

            // Step 4: Project to model
            var reportModelsQuery = approvalsQuery
                .Select(a => new GetAllReportModel
                {
                    Id = a.Report!.Id,
                    ReportType = a.Report.ReportType.ToArabic(),
                    GehaCode = a.Report.GehaCode,
                    ShoabaName = a.Report.ShoabaName,
                    Description = a.Report.Description,
                    CreatedAt = a.Report.CreatedAt,
                    CurrentApprovalLevel = a.Report.CurrentApprovalLevel.ToString(),
                    IsApprovedByRA = a.Report.IsApprovedByRA,
                    IsRejected = a.Report.IsRejected,
                    FilePath = a.Report.FilePath
                })
                .ApplyFilters(request.Filters)
                .ApplyDateRangeFilter(request.StartRange, request.EndRange)
                .ApplySorting(request.SortBy, request.SortDirection);

            // Step 5: Paginate & return
            var result = await PagedList<GetAllReportModel>.CreateAsync(
                reportModelsQuery,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            return result;
        }
    }
}
