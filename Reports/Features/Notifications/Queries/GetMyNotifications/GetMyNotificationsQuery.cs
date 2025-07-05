using Reports.Api.Common.Abstractions.Collections;
using Reports.Api.Data;
using Reports.Api.Services.CurrentUser;
using Reports.Common.Abstractions.Collections;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;
using Reports.Features.Notifications.Models;
using TwoHO.Api.Extensions;

namespace Reports.Features.Notifications.Queries.GetMyNotifications
{
    public class GetMyNotificationsQuery : HasTableView, ICommand<PagedList<GetMyNotificationsModel>>
    {
        public string Search { get; set; } = string.Empty;
    }

    public class GetMyNotificationsQueryHandler(
        AppDbContext _context,
        ICurrentUserService _currentUserService
    ) : ICommandHandler<GetMyNotificationsQuery, PagedList<GetMyNotificationsModel>>
    {
        public async Task<PagedList<GetMyNotificationsModel>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var userId = _currentUserService.UserId;


                var allowedFields = new List<string> { "Id", "Title", "type", "CreatedAt", "IsRead" };
                var allowedSorting = new List<string> { "Id", "CreatedAt" };

                request.ValidateFiltersAndSorting(allowedFields, allowedSorting);


                var queryAll = _context.Notifications
                    .Where(n => n.ReceiverId == userId)
                    .AsQueryable();


                // Apply search filter if provided
                if (!string.IsNullOrEmpty(request.Search))
                {
                    queryAll = queryAll.Where(r =>
                        r.Title.Contains(request.Search));
                }

                // Filter by Notification type
                if (request.Filters != null && request.Filters.TryGetValue("type", out string? type))
                {
                    if (Enum.TryParse<NotificationType>(type, true, out var notificationType))
                    {
                        queryAll = queryAll.Where(r => r.Type == notificationType);
                        request.Filters.Remove("type"); // remove it after use
                    }
                    else
                    {
                        throw new BadRequestException("Invalid type value.");
                    }
                }

                var notifications = queryAll
                    .Select(n => new GetMyNotificationsModel
                    {
                        Id = n.Id,
                        Title = n.Title,
                        Type = n.Type.ToString(),
                        CreatedAt = n.CreatedAt,
                        IsRead = n.IsRead
                    })
                    .ApplyFilters(request.Filters)
                .ApplySorting(request.SortBy, request.SortDirection)
                .AsQueryable();

                return await PagedList<GetMyNotificationsModel>.CreateAsync(notifications, request.PageNumber, request.PageSize, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }

        }
    }


}
