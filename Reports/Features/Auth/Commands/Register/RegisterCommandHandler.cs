using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reports.Api.Auth.Models;
using Reports.Api.Auth.Services;
using Reports.Api.Data;
using Reports.Api.Services.CurrentUser;
using Reports.Common.Abstractions.Mediator;
using Serilog;

namespace Reports.Api.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler
        (IAuthService authService,
        ICurrentUserService currentUserService,

        AppDbContext appDbContext) : ICommandHandler<RegisterCommand>
    {
        public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Start a new transaction
            await using var transaction = await appDbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // register user
                var customerId = await authService.RegisterAdminAsync(new RegisterModel()
                {

                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,

                    Password = request.Password,
                    Level = request.Level,
                    Geha = request.Geha,
                    Signature = request.Signature,



                }, currentUserService.UserId);

                // log
                Log.Information("User registered successfully with id {UserId} by admin {AdminId}", customerId, currentUserService.UserId);




                // Commit the transaction if everything succeeds
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of an error
                Log.Error(ex, "Error occurred while registering user by admin {AdminId}", currentUserService.UserId);

                await transaction.RollbackAsync(cancellationToken);
                throw; // Rethrow the exception for further handling
            }

        }
    }

    // create fluent validation
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator(AppDbContext context)
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.")
                .MustAsync(async (email, cancellation) =>
                    !await context.Users.AnyAsync(u => u.Email == email, cancellation))
                .WithMessage("Email already exists.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

            RuleFor(x => x.Geha)
                .NotEmpty().WithMessage("Geha is required.");

            RuleFor(x => x.Level)
                .IsInEnum().WithMessage("Invalid level.");


        }
    }
}
