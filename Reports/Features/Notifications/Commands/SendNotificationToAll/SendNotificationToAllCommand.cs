using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reports.Api.Data;
using Reports.Api.Services.Notifications;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;

namespace Reports.Features.Notifications.Commands.SendNotificationToAll
{
    public class SendNotificationToAllCommand : ICommand
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required NotificationType Type { get; set; }
    }

    public class SendNotificationToAllCommandHandler(
        AppDbContext _context,
        INotificationService _notificationService
    ) : ICommandHandler<SendNotificationToAllCommand>
    {
        public async Task Handle(SendNotificationToAllCommand request, CancellationToken cancellationToken)
        {


            var users = await _context.Users.ToListAsync(cancellationToken);

            if (!users.Any())
                throw new NotFoundException("No users found to send notifications.");

            foreach (var user in users)
            {
                await _notificationService.SendNotificationAsync(
                    title: request.Title,
                    content: request.Content,
                    receiverId: user.Id,
                    type: request.Type,
                    cancellationToken: cancellationToken
                );
            }
        }
    }

    public class SendNotificationToAllCommandValidator : AbstractValidator<SendNotificationToAllCommand>
    {
        public SendNotificationToAllCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.")
                .MaximumLength(500).WithMessage("Content cannot exceed 500 characters.");
        }
    }
}
