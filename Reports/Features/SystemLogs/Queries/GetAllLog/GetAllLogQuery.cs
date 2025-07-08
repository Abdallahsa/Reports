using Reports.Api.Common.Abstractions.Collections;
using Reports.Api.Data;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Features.SystemLogs.Models;
using Serilog;
using TwoHO.Api.Extensions;

namespace Reports.Features.SystemLogs.Queries.GetAllLog
{
    public class GetAllLogQuery : HasTableViewWithDate, ICommand<PagedList<GetAllLogModel>>
    {
        public string? Search { get; set; }
    }

    // handler of GetAllLogQuery
    public class GetAllLogQueryHandler(AppDbContext _context)
         : ICommandHandler<GetAllLogQuery, PagedList<GetAllLogModel>>
    {
        public async Task<PagedList<GetAllLogModel>> Handle(GetAllLogQuery request, CancellationToken cancellationToken)
        {
            try
            {

                var allowedFields = new List<string> { "Id", "Message", "Level", "Exception" };
                var allowedSorting = new List<string> { "Id", "TimeStamp" };

                request.ValidateFiltersAndSorting(allowedFields, allowedSorting);


                var queryAll = _context.SystemLog.AsQueryable();


                // Apply search filter if provided
                if (!string.IsNullOrEmpty(request.Search))
                {
                    queryAll = queryAll.Where(r =>
                        r.Message.Contains(request.Search));
                }



                var logs = queryAll
                    .Select(n => new GetAllLogModel
                    {
                        Id = n.Id,
                        Message = n.Message,
                        MessageTemplate = n.MessageTemplate,
                        Level = n.Level,
                        TimeStamp = n.TimeStamp,
                        Exception = n.Exception,
                        Properties = n.Properties
                    })
                    .ApplyFilters(request.Filters)
                .ApplySorting(request.SortBy, request.SortDirection)
                .AsQueryable();

                return await PagedList<GetAllLogModel>.CreateAsync(logs, request.PageNumber, request.PageSize, cancellationToken);

            }
            catch (Exception ex)
            {

                Log.Error(ex, "An error occurred while handling GetAllLogQuery");
                throw new BadRequestException(ex.Message);
            }

        }
    }

}
