using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reports.Api.Domain.Constants;
using Reports.Api.Domain.Entities;

namespace Reports.Data.Seeders
{
    public class RoleSeeder
    {
        public static async Task SeedRoles(RoleManager<Role> roleManager)
        {
            if (await roleManager.Roles.AnyAsync())
                return;

            //manual save roles instead
            await roleManager.CreateAsync(new Role(RoleConstants.Admin));
            await roleManager.CreateAsync(new Role(RoleConstants.LevelZero));
            await roleManager.CreateAsync(new Role(RoleConstants.LevelOne));
            await roleManager.CreateAsync(new Role(RoleConstants.LevelTwo));
            await roleManager.CreateAsync(new Role(RoleConstants.LevelThree));
            await roleManager.CreateAsync(new Role(RoleConstants.LevelFour));
        }
    }
}
