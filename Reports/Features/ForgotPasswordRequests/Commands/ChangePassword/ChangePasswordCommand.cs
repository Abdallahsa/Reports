﻿using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reports.Api.Data;
using Reports.Api.Domain.Entities;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;
using Reports.Service.LoggingService;


namespace Reports.Features.ForgotPasswordRequests.Commands.ChangePassword
{
    public class ChangePasswordCommand : ICommand<string>
    {
        public int RequestId { get; set; }
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ChangePasswordCommandHandler(
        AppDbContext _context,
        ILoggingService _loggingService,
        UserManager<User> _userManager)
        : ICommandHandler<ChangePasswordCommand, string>
    {
        public async Task<string> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var forgotRequest = await _context.ForgotPasswordRequests
                    .FirstOrDefaultAsync(f => f.Id == request.RequestId, cancellationToken)
                    ?? throw new NotFoundException("ForgotPasswordRequest", request.RequestId);

                if (forgotRequest.IsUsed)
                    throw new BadRequestException("This reset password request has already been used.");

                var user = await _userManager.FindByEmailAsync(forgotRequest.Email)
                    ?? throw new NotFoundException("User with email", forgotRequest.Email);

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

                if (!result.Succeeded)
                {
                    await _loggingService.LogError(
                        "Failed to reset password for user {UserId}: {Errors}",
                        null,
                        user.Id,
                        string.Join(",", result.Errors.Select(e => e.Description))
                    );
                    throw new BadRequestException(result.Errors.First().Description);
                }

                forgotRequest.IsUsed = true;
                await _context.SaveChangesAsync(cancellationToken);

                await _loggingService.LogInformation("Password changed successfully for user {UserId}", user.Id);

                return "Password changed successfully.";
            }
            catch (Exception ex)
            {
                await _loggingService.LogError("Error changing password for request {RequestId}: {Message}", ex, request.RequestId, ex.Message);

                throw new BadRequestException(ex.Message);
            }
        }
    }

    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator(AppDbContext _context)
        {
            RuleFor(x => x.RequestId)
                .NotEmpty().WithMessage("RequestId is required")
                .GreaterThan(0).WithMessage("RequestId must be greater than 0");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required")
                .MinimumLength(6).WithMessage("New password must be at least 6 characters long");

            // check if the RequestId exists in the database
            RuleFor(x => x.RequestId)
                .MustAsync(async (id, cancellation) =>
                {
                    return await _context.Set<ForgotPasswordRequest>().AnyAsync(x => x.Id == id, cancellation);
                }).WithMessage("Forgot password request with the specified ID does not exist.");

        }
    }

}
