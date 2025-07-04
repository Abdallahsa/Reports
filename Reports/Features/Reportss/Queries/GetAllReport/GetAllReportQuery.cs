using Reports.Api.Common.Abstractions.Collections;
using Reports.Api.Data;
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
        public bool Archive { get; set; } = true;

    }

    //Create handler for GetAllReportQuery

    public class GetAllReportQueryHandler(
        AppDbContext context,
        IUserReportService _userReportService

        ) : ICommandHandler<GetAllReportQuery, PagedList<GetAllReportModel>>
    {
        public async Task<PagedList<GetAllReportModel>> Handle(GetAllReportQuery request, CancellationToken cancellationToken)
        {
            try
            {

                var allowedFields = new List<string> { "Id", "reportType", "GehaCode", "ShoabaName", "Description" };
                var allowedSorting = new List<string> { "Id", "reportType", "CreatedAt" };

                request.ValidateFiltersAndSorting(allowedFields, allowedSorting);

                // useing service IUserReportService to get allowed reports for current user and using where 
                var allowedReports = _userReportService.GetAllowedReportsForCurrentUser();

                var queryAll = context
                    .Reports
                    .Where(r => allowedReports.Contains(r.ReportType) && r.IsApprovedByRA == request.Archive)
                    .AsQueryable();

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(request.Search))
                {
                    queryAll = queryAll.Where(r => r.GehaCode.Contains(request.Search) || r.ShoabaName.Contains(request.Search) || r.Description.Contains(request.Search));
                }



                if (request.Filters != null && request.Filters.TryGetValue("reportType", out string? reportType))
                {
                    if (Enum.TryParse<ReportType>(reportType, true, out var type))
                    {
                        queryAll = queryAll.Where(o => o.ReportType == type);

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
                .AsQueryable()
                ?? throw new NotFoundException("Cart item not found");


                // Paginate the result
                var result = await PagedList<GetAllReportModel>.CreateAsync(query, request.PageNumber, request.PageSize, cancellationToken);

                return result;


            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);


            }
        }





    }
}
