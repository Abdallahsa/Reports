using Microsoft.AspNetCore.Identity;
using Reports.Api.Auth.Models;
using Reports.Api.Domain.Entities;
using Reports.Application.Auth.Models;
using Reports.Features.Admin.Models;

namespace Reports.Api.Auth.Services
{
    public interface IAuthService
    {
        Task<int> RegisterAdminAsync(RegisterModel model, int? createdBy);
        Task<int> RegisterCustomerAsync(RegisterModel model, int? createdBy);

        // update user profile 
        Task<int> UpdateProfileAsync(UpdateProfileModel model);
        Task<LoginResponseModel> LoginAsync(LoginModel model);

        Task SaveRefreshTokenAsync(User user, string refreshToken);

        Task<IdentityResult> ForceChangePasswordAsync(string userId, string newPassword);

        public Task ConfirmByEmail(string email);

        Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordModel model);
        Task<ICollection<string>> GetRolesAsync(User? user = null);
    }
}
