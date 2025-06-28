using Reports.Api.Domain.Entities;

namespace Reports.Api.Auth.Services
{
    public interface ITokenGenerator
    {
        public Task<string> GenerateJwtToken(User user);
        public Task<string> GenerateRefreshToken();
        int RefreshTokenExpiryInDays { get; }
        int TokenExpiryInMinutes { get; }
    }
}
