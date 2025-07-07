using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reports.Api.Auth.Models;
using Reports.Api.Auth.Services;
using Reports.Api.Data;
using Reports.Api.Features.Auth.Models;
using Reports.Api.Services.CurrentUser;
using Reports.Common.Abstractions.Mediator;
using Serilog;

namespace Reports.Features.Admin.Commands.AddUser
{
    public class AddUserCommand : RegisterCustomerModel, ICommand<int>
    {
    }

    public class AddUserCommandHandler : ICommandHandler<AddUserCommand, int>
    {
        private readonly IAuthService authService;
        private readonly ICurrentUserService currentUserService;

        public AddUserCommandHandler(IAuthService authService, ICurrentUserService currentUserService)
        {
            this.authService = authService;
            this.currentUserService = currentUserService;
        }

        public async Task<int> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var createResult = await authService.RegisterCustomerAsync(new RegisterModel
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Password = request.Password,
                    Level = request.Level,
                    Geha = request.Geha,
                    Signature = request.Signature

                }, currentUserService.UserId);

                // log
                Log.Information("User registered successfully with id {UserId} by admin {AdminId}", createResult, currentUserService.UserId);


                return createResult;
            }


            catch (Exception ex)
            {
                Log.Error(ex, "Error while registering user with email {Email}", request.Email);

                throw new Exception($"Failed to register User: {ex.Message}", ex);

            }
        }
    }

    public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
    {
        public AddUserCommandValidator(AppDbContext context)
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
                .MustAsync(async (email, cancellation) => !await context.Users.AnyAsync(u => u.Email == email, cancellation))
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
