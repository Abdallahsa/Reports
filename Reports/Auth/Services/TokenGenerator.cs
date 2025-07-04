using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Reports.Api.Data;
using Reports.Api.Domain.Entities;
using Reports.Api.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Reports.Api.Auth.Services
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtSettings _settings;
        private readonly RoleManager<Role> _roleManager;
        private readonly AppDbContext _context;

        public TokenGenerator(UserManager<User> userManager, RoleManager<Role> roleManager, IOptions<JwtSettings> settings, AppDbContext context)
        {
            _userManager = userManager;
            _settings = settings.Value;
            _roleManager = roleManager;
            _context = context;
        }







        public int TokenExpiryInMinutes
        {
            get
            {
                // Try to parse the string to an integer, if parsing fails, fallback to 60
                return _settings.ExpiryInMinutes;
            }
        }

        public int RefreshTokenExpiryInDays
        {
            get
            {
                // Try to parse the string to an integer, if parsing fails, fallback to 30
                return _settings.RefreshTokenExpiryInDays;
            }
        }

        public async Task<string> GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return await Task.Run(() => Convert.ToBase64String(randomNumber));
        }

        public async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            };

            // add user geha
            var geha = GetUserLevel(user);
            if (geha != null)
            {
                claims.Add(new Claim("Geha", geha));
            }

            // Fetch roles and add them as claims
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret ?? string.Empty));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.ValidIssuer,
                audience: _settings.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_settings.ExpiryInMinutes),
                signingCredentials: creds);

            return await Task.Run(() => new JwtSecurityTokenHandler().WriteToken(token));
        }


        private static string? GetUserLevel(User user)
        {
            return user.Level.ToString();

        }




    }
}
