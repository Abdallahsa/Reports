using Microsoft.AspNetCore.Identity;
using Reports.Api.Data;
using Reports.Api.Domain.Entities;
using Reports.Data.Seeders;

namespace Manzoma.Api.Data.Seeders
{
    public static class MainSeeder
    {
        public static async Task SeedData(this IApplicationBuilder applicationBuilder)
        {
            // Create a new scope to retrieve scoped services
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
            var services = serviceScope.ServiceProvider;

            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<Role>>();
            var context = services.GetRequiredService<AppDbContext>();
            var colorSeed = services.GetRequiredService<AppDbContext>();
            // var logger = services.GetRequiredService<ILogger>();

            try
            {
                // seed Roles
                await RoleSeeder.SeedRoles(roleManager);
                // seed Admins
                await AdminSeeder.SeedAdmins(userManager);

                // seed users for testing approval
                await UserSeeder.SeedApprovalUsers(userManager);




            }
            catch
            {
                throw;
            }

        }
    }
}
