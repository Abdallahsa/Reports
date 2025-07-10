using FluentValidation;
using Reports.Api.Common.Abstractions.Collections;
using Reports.Api.Data;
using Reports.Common.Abstractions.Mediator;
using Reports.Domain.Entities;
using Reports.Features.ForgotPasswordRequests.Models;
using TwoHO.Api.Extensions;

namespace Reports.Features.ForgotPasswordRequests.Queries.GetAllForgotPasswordRequest
{
    public class GetAllForgotPasswordRequestQuery : HasTableViewWithDate, ICommand<PagedList<ForgotPasswordRequestModel>>
    {
        public string? Search { get; set; }
    }

    public class GetAllForgotPasswordRequestQueryHandler(
               AppDbContext _context
           ) : ICommandHandler<GetAllForgotPasswordRequestQuery, PagedList<ForgotPasswordRequestModel>>
    {
        public async Task<PagedList<ForgotPasswordRequestModel>> Handle(GetAllForgotPasswordRequestQuery request, CancellationToken cancellationToken)
        {
            var allowedFields = new List<string> { "Id", "Email", "Phone", "CreatedAt", "IsUsed" };
            var allowedSorting = new List<string> { "CreatedAt" };

            request.ValidateFiltersAndSorting(allowedFields, allowedSorting);


            var queryAll = _context.Set<ForgotPasswordRequest>().AsQueryable();

            if (!string.IsNullOrEmpty(request.Search))
            {
                queryAll = queryAll.Where(x => x.Email.Contains(request.Search) || x.Phone.Contains(request.Search));
            }

            var query = queryAll
                .Select(x => new ForgotPasswordRequestModel
                {
                    Id = x.Id,
                    Email = x.Email,
                    CreatedAt = x.CreatedAt,
                    IsUsed = x.IsUsed,
                    Phone = x.Phone
                })
                .ApplyFilters(request.Filters)
                .ApplyDateRangeFilter(request.StartRange, request.EndRange)
                .ApplySorting(request.SortBy, request.SortDirection)
                .AsQueryable();


            var result = await PagedList<ForgotPasswordRequestModel>.CreateAsync(query, request.PageNumber, request.PageSize, cancellationToken);

            return result;

        }
    }



}
