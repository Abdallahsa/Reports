using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reports.Api.Domain.Constants;
using Reports.Api.Domain.Entities;

namespace Reports.Data.Seeders
{
    public static class AdminSeeder
    {
        public static async Task SeedAdmins(UserManager<User> userManager)
        {
            var pass = "test123";
            List<User> admins = [
                new()
                {
                    Email = "test@test.com",
                    UserName = "test",
                    EmailConfirmed = true,
                    Level = Level.Admin,
},
            ];

            if (await userManager.Users.AnyAsync())
                return;

            foreach (var admin in admins)
            {
                await userManager.CreateAsync(admin, pass);
                await userManager.AddToRoleAsync(admin, RoleConstants.Admin);
            }
        }
    }
}
