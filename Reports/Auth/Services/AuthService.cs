﻿using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reports.Api.Auth.Models;
using Reports.Api.Data;
using Reports.Api.Domain.Constants;
using Reports.Api.Domain.Entities;
using Reports.Api.Services;
using Reports.Application.Auth.Models;
using Reports.Common.Exceptions;
using Reports.Features.Admin.Models;

namespace Reports.Api.Auth.Services
{
    public class AuthService
        (AppDbContext context,
        RoleManager<Role> roleManager,
        UserManager<User> userManager,
        ITokenGenerator tokenGenerator,
        IStorageService _storageService
        ) : IAuthService
    {


        private async Task<int> RegisterUserAsync<T>(RegisterModel model, string roleName, Func<T, Task<IdentityResult>> createFunc) where T : User, new()
        {
            var user = new T
            {
                Email = model.Email,
                EmailConfirmed = true,
                Level = model.Level,
                Geha = model.Geha.ToString(),
                SignaturePath = model.Signature != null ? await _storageService.SaveFileAsync(model.Signature) : string.Empty,
            };

            var createResult = await createFunc(user);
            if (!createResult.Succeeded)
            {
                List<ValidationFailure> validationFailures = createResult.Errors
                    .Select(error => new ValidationFailure(error.Code, error.Description))
                    .ToList();
                throw new BadRequestException(validationFailures);
            }


            var roleResult = await userManager.AddToRoleAsync(user, roleName);
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException($"Failed to assign role {roleName} to user.");
            }

            return user.Id;
        }
        public async Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordModel model)
        {
            var user = await userManager.FindByIdAsync(userId)
               ?? throw new NotFoundException(nameof(User), userId);
            var result = await userManager.ChangePasswordAsync(user!, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(x => new ValidationFailure(x.Code, x.Description)));


            return result;
        }

        public async Task ConfirmByEmail(string email)
        {
            var user = await userManager.FindByEmailAsync(email)
                ?? throw new NotFoundException();

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var result = await userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                throw new Exception("Failed to confirm email.");
        }


        public async Task<ICollection<string>> GetRolesAsync(User? user = null)
        {

            if (user == null)
            {
                var res = await roleManager.Roles.Select(x => x.Name).ToListAsync();
                return res!;
            }

            return await userManager.GetRolesAsync(user);
        }

        public async Task<LoginResponseModel> LoginAsync(LoginModel model)
        {
            // Manually authenticate the user
            var user = await userManager.FindByEmailAsync(model.Email) ??
                throw new UnauthorizedAccessException("Invalid login attempt.");

            if (!await userManager.CheckPasswordAsync(user, model.Password))
            {
                //await _userManager.AccessFailedAsync(user);
                throw new UnauthorizedAccessException("Invalid login attempt.");
            }

            if (!user.EmailConfirmed)
            {
                throw new UnauthorizedAccessException("this Account is not Confirmed by admin.");
            }
            ICollection<string> roles = await userManager.GetRolesAsync(user);
            var token = await tokenGenerator.GenerateJwtToken(user);
            var refreshToken = await tokenGenerator.GenerateRefreshToken();
            await SaveRefreshTokenAsync(user, refreshToken);

            return new LoginResponseModel
            {
                Token = token,
                TokenExpiryInMinutes = tokenGenerator.TokenExpiryInMinutes,
                RefreshToken = refreshToken,
                IsConfirmed = user.EmailConfirmed,
                Geha = user.Geha.ToString(),
                UserId = user.Id,
                Roles = roles,
            };
        }

        public Task<int> RegisterAdminAsync(RegisterModel model, int? createdBy)
        {
            return RegisterUserAsync<User>(model, RoleConstants.Admin, async user =>
            {
                user.Level = Level.Admin;
                user.UserName = model.Email;
                user.EmailConfirmed = true; // Admins are confirmed by default 
                return await userManager.CreateAsync(user, model.Password);
            });

        }

        public Task<int> RegisterCustomerAsync(RegisterModel model, int? createdBy)
        {



            return RegisterUserAsync<User>(model, model.Level.ToString(), async user =>
            {
                user.UserName = model.Email;
                user.Level = model.Level;
                user.EmailConfirmed = true;
                user.Geha = model.Geha.ToString();
                //user.SignaturePath = model.Signature != null ? await _storageService.SaveFileAsync(model.Signature) : string.Empty;
                return await userManager.CreateAsync(user, model.Password);
            });

        }


        public async Task<IdentityResult> ForceChangePasswordAsync(string userId, string newPassword)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found.");

            // Remove the old password if it exists
            var hasPassword = await userManager.HasPasswordAsync(user);
            if (hasPassword)
            {
                var removeResult = await userManager.RemovePasswordAsync(user);
                if (!removeResult.Succeeded)
                {
                    return removeResult;
                }
            }

            // Add the new password
            var addResult = await userManager.AddPasswordAsync(user, newPassword);
            return addResult;
        }



        public async Task SaveRefreshTokenAsync(User user, string refreshToken)
        {
            var token = new RefreshToken
            {
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(tokenGenerator.RefreshTokenExpiryInDays),
                UserId = user.Id
            };

            await context.RefreshTokens.AddAsync(token);
            await context.SaveChangesAsync();
        }

        public async Task<int> UpdateProfileAsync(UpdateProfileModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId.ToString())
              ?? throw new NotFoundException(nameof(User), model.UserId);

            user.SignaturePath = model.Signature != null
                ? await _storageService.SaveFileAsync(model.Signature, false)
                : user.SignaturePath;
            user.PasswordHash = model.Password != null
                ? userManager.PasswordHasher.HashPassword(user, model.Password)
                : user.PasswordHash;

            userManager.UpdateAsync(user).GetAwaiter().GetResult();

            return user.Id;
        }
    }

}
