using Reports.Api.Common.Abstractions.Collections;
using Reports.Api.Data;
using Reports.Api.Domain.Entities;
using Reports.Api.Features.Common.Models;
using Reports.Common.Abstractions.Mediator;
using Serilog;
using TwoHO.Api.Extensions;

namespace Reports.Features.Auth.Queries.GetAllUsers
{
    public class GetAllUsersQuery : HasTableViewWithDate, ICommand<PagedList<UserDto>>
    {
        public string? Search { get; set; }
    }

    // Handler of GetAllUsersQuery
    public class GetAllUsersQueryHandler(
               AppDbContext _context
           ) : ICommandHandler<GetAllUsersQuery, PagedList<UserDto>>
    {
        public async Task<PagedList<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var allowedFields = new List<string> { "Id", "UserName", "Email", "CreatedAt", "IsConfirmed" };
                var allowedSorting = new List<string> { "CreatedAt", "Id" };

                request.ValidateFiltersAndSorting(allowedFields, allowedSorting);

                var queryAll = _context.Set<User>().AsQueryable();

                if (!string.IsNullOrEmpty(request.Search))
                {
                    queryAll = queryAll.Where(x => x.UserName!.Contains(request.Search) || x.Email!.Contains(request.Search));
                }

                var query = queryAll
                    .Select(x => new UserDto
                    {
                        Id = x.Id,
                        UserName = x.UserName != null ? x.UserName : string.Empty,
                        Email = x.Email != null ? x.Email : string.Empty,
                        IsActive = x.EmailConfirmed,
                        SignaturePath = x.SignaturePath,
                        Geha = x.Geha,
                        Level = x.Level.ToString()

                    })
                    .ApplyFilters(request.Filters)
                    .ApplyDateRangeFilter(request.StartRange, request.EndRange)
                    .ApplySorting(request.SortBy, request.SortDirection)
                    .AsQueryable();

                var result = await PagedList<UserDto>.CreateAsync(query, request.PageNumber, request.PageSize, cancellationToken);

                // Log the successful retrieval of users
                Log.Information("Successfully retrieved users. Count: {Count}", result.TotalCount);

                return result;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Log.Error(ex, "An error occurred while retrieving users.");
                throw new Exception("An error occurred while retrieving users.", ex);
            }
        }
    }

}
