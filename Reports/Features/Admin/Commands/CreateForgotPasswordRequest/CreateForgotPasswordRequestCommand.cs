using Microsoft.EntityFrameworkCore;
using Reports.Api.Data;
using Reports.Api.Domain.Entities;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;

namespace Reports.Features.Admin.Commands.CreateForgotPasswordRequest
{
    public class CreateForgotPasswordRequestCommand : ICommand<bool>
    {
        public string Email { get; set; } = string.Empty;
    }

    public class CreateForgotPasswordRequestCommandHandler(
        AppDbContext _context)
        : ICommandHandler<CreateForgotPasswordRequestCommand, bool>
    {
        public async Task<bool> Handle(CreateForgotPasswordRequestCommand request, CancellationToken cancellationToken)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.Email == request.Email, cancellationToken)
                ?? new BadRequestException("this Email is not found");

            if (!userExists)
                throw new NotFoundException(nameof(User), request.Email);

            var forgotRequest = new ForgotPasswordRequest
            {
                Email = request.Email,
                CreatedAt = DateTime.UtcNow,
                IsUsed = false
            };

            await _context.Set<ForgotPasswordRequest>().AddAsync(forgotRequest, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

}
