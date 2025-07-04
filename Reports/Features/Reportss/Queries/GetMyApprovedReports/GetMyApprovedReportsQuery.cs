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

            try
            {
                var allowedFields = new List<string> { "Id", "reportType", "GehaCode", "ShoabaName", "Description", "CreatedAt" };
                var allowedSorting = new List<string> { "Id", "CreatedAt" };

                request.ValidateFiltersAndSorting(allowedFields, allowedSorting);

                // Fetch the reports from the database
                var queryAll = _context.Approval
                    .Include(r => r.Report)
                    .Where(r => r.UserId == _currentUserService.UserId)
                    .AsQueryable();

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(request.Search))
                {
                    queryAll = queryAll.Where(r => r.Report!.GehaCode.Contains(request.Search) || r.Report.ShoabaName.Contains(request.Search) || r.Report.Description.Contains(request.Search));
                }

                if (request.Filters != null && request.Filters.TryGetValue("reportType", out string? reportType))
                {
                    if (Enum.TryParse<ReportType>(reportType, true, out var type))
                    {
                        queryAll = queryAll.Where(o => o.Report!.ReportType == type);

                        // remove from filters
                        request.Filters.Remove("productType");
                    }
                    else
                    {
                        throw new BadRequestException("Invalid productType value.");
                    }
                }

                var query = queryAll
                    .Select(r => new GetAllReportModel
                    {
                        Id = r.Id,
                        ReportType = r.Report!.ReportType.ToArabic(),
                        GehaCode = r.Report.GehaCode,
                        ShoabaName = r.Report.ShoabaName,
                        Description = r.Report.Description,
                        CreatedAt = r.Report.CreatedAt,
                        CurrentApprovalLevel = r.Report.CurrentApprovalLevel.ToString(),
                        IsApprovedByRA = r.Report.IsApprovedByRA,
                        IsRejected = r.Report.IsRejected,
                        FilePath = r.Report.FilePath,
                    })
                    .ApplyFilters(request.Filters)
                    .ApplyDateRangeFilter(request.StartRange, request.EndRange)
                    .ApplySorting(request.SortBy, request.SortDirection)
                    .AsQueryable()
                    ?? throw new NotFoundException("Cart item not found");


                // Paginate the result
                var result = await PagedList<GetAllReportModel>.CreateAsync(query, request.PageNumber, request.PageSize, cancellationToken);

                return result;


            }
            catch (Exception ex)
            {
                // Log the exception
                throw new BadRequestException(ex.Message);
            }
        }
    }

}
