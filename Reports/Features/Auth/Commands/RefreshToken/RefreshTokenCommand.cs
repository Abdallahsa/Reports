using Reports.Api.Auth.Models;
using Reports.Api.Auth.Services;
using Reports.Api.Data;
using Reports.Common.Abstractions.Mediator;
using Microsoft.EntityFrameworkCore;

namespace Reports.Api.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommand : ICommand<LoginResponseModel>
    {
        public string? RefreshToken { get; set; }
    }

    public class RefreshTokenCommandHandler(
       AppDbContext context,
       IAuthService authService,
       ITokenGenerator tokenGenerator) : ICommandHandler<RefreshTokenCommand, LoginResponseModel>
    {
        public async Task<LoginResponseModel> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // begin db transaction
            using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // Find the refresh token in the database
                var refreshToken = await context
                    .RefreshTokens
                    .FirstOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken)
                    ?? throw new UnauthorizedAccessException("Invalid token.");

                if (!refreshToken.IsActive)
                {
                    throw new UnauthorizedAccessException("Refresh token is expired or not active.");
                }

                // Generate a new access token and a new refresh token
                var user = await context.Users
                    .FindAsync([refreshToken.UserId, cancellationToken], cancellationToken: cancellationToken)
                    ?? throw new UnauthorizedAccessException("User not found.");

                var newTokens = await tokenGenerator.GenerateJwtToken(user);

                // commit db transaction
                await transaction.CommitAsync(cancellationToken);

                return new LoginResponseModel
                {
                    IsConfirmed = user.EmailConfirmed,
                    Token = newTokens,
                    TokenExpiryInMinutes = tokenGenerator.TokenExpiryInMinutes,
                    RefreshToken = refreshToken.Token, // return the same refresh token
                    UserId = user.Id,
                    level = user.Level.ToString(),
                    Roles = await authService.GetRolesAsync(user)
                };

            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }



        }
    }
}
