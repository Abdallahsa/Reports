using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reports.Api.Data;
using Reports.Api.Domain.Entities;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;
using Reports.Service.LoggingService;


namespace Reports.Features.ForgotPasswordRequests.Commands.CreateForgotPasswordRequests
{
    public class CreateForgotPasswordRequestCommand : ICommand<string>
    {
        public required string Email { get; set; }
        public required string Phone { get; set; }

    }


    public class CreateForgotPasswordRequestCommandHandler(
        AppDbContext _context,
        ILoggingService _loggingService
        ) : ICommandHandler<CreateForgotPasswordRequestCommand, string>
    {
        public async Task<string> Handle(CreateForgotPasswordRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userExists = await _context.Users
                    .AnyAsync(u => u.Email == request.Email, cancellationToken);

                if (!userExists)
                    throw new NotFoundException(nameof(User), request.Email);

                var forgotRequest = new ForgotPasswordRequest
                {
                    Email = request.Email,
                    CreatedAt = DateTime.UtcNow,
                    IsUsed = false,
                    Phone = request.Phone
                };

                await _context.Set<ForgotPasswordRequest>().AddAsync(forgotRequest, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // Log the creation of the forgot password request
                await _loggingService.LogInformation("Forgot password request created for email: {Email} at {Time}", request.Email, DateTime.UtcNow);

                return "Forgot password request created successfully.";
            }
            catch (Exception ex)
            {

                await _loggingService.LogError("Error creating forgot password request for email {Email}: {Message}", ex, request.Email, ex.Message);

                throw new BadRequestException(ex.Message);
            }
        }
    }
    public class CreateForgotPasswordRequestCommandValidator : AbstractValidator<CreateForgotPasswordRequestCommand>
    {
        public CreateForgotPasswordRequestCommandValidator(AppDbContext context)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Email)
                .MustAsync(async (email, cancellationToken) =>
                {
                    return await context.Users.AnyAsync(u => u.Email == email, cancellationToken);
                })
                .WithMessage("Email does not exist in the system");
        }
    }
}





