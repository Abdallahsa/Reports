using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reports.Api.Data;
using Reports.Api.Services.CurrentUser;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Features.Notifications.Models;

namespace Reports.Features.Notifications.Queries.GetNotificationById
{
    public class GetNotificationByIdQuery : ICommand<GetNotificationByIdModel>
    {
        public required int Id { get; set; }
    }

    public class GetNotificationByIdQueryHandler(
        AppDbContext _context,
        ICurrentUserService _currentUserService
    ) : ICommandHandler<GetNotificationByIdQuery, GetNotificationByIdModel>
    {
        public async Task<GetNotificationByIdModel> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUserService.UserId;

                var notification = await _context.Notification
                    .Include(n => n.Sender)
                    .Where(n => n.Id == request.Id && n.ReceiverId == userId)
                    .Select(n => new GetNotificationByIdModel
                    {
                        Id = n.Id,
                        Title = n.Title,
                        Type = n.Type.ToString(),
                        CreatedAt = n.CreatedAt,
                        IsRead = n.IsRead,
                        Content = n.Content,
                        SenderName = n.Sender!.UserName != null ? n.Sender!.UserName : string.Empty,
                    })
                    .FirstOrDefaultAsync(cancellationToken)
                    ?? throw new NotFoundException("Notification not found");

                // Mark notification as read
                if (!notification.IsRead)
                {
                    var dbNotification = await _context.Notification.FindAsync(request.Id);
                    if (dbNotification != null)
                    {
                        dbNotification.IsRead = true;
                        _context.Notification.Update(dbNotification);
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                }

                return notification;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }
    }

    public class GetNotificationByIdQueryValidator : AbstractValidator<GetNotificationByIdQuery>
    {
        public GetNotificationByIdQueryValidator
            (
            AppDbContext _context,
            ICurrentUserService _currentUserService
            )
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Notification ID must be greater than 0.");

            RuleFor(x => x.Id)
                .MustAsync(async (id, cancellationToken) =>
                {
                    return await _context.Notification.AnyAsync(n => n.Id == id, cancellationToken);
                })
                .WithMessage("Notification with this ID does not exist.");

            // check if the notification belongs to the current user
            RuleFor(x => x.Id)
                .MustAsync(async (request, id, cancellationToken) =>
                {
                    return await _context.Notification.AnyAsync(n => n.Id == id && n.ReceiverId == _currentUserService.UserId, cancellationToken);
                })
                .WithMessage("You do not have permission to access this notification.");
        }
    }

}
