using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reports.Api.Domain.Constants;
using Reports.Api.Domain.Entities;

namespace Reports.Data.Seeders
{
    public class UserSeeder
    {
        public static async Task SeedApprovalUsers(UserManager<User> userManager)
        {
            var password = "user123";

            // users for each level with different Geha
            var users = new List<User>();

            // LevelZero users with different Geha
            var levelZeroGehas = new[]
            {
                Geha.Eshara, Geha.Operations, Geha.Tahrokat, Geha.Elc, Geha.Mar, Geha.Rader, Geha.Sar, Geha.Sat, Geha.Tash, Geha.AZ
            };

            foreach (var geha in levelZeroGehas)
            {
                users.Add(new User
                {
                    Email = $"{geha}@user.com",
                    UserName = $"{geha}@user.com",
                    EmailConfirmed = true,
                    Geha = geha.ToString(),
                    Level = Level.LevelZero
                });
            }

            // LevelOne user (NRA)
            users.Add(new User
            {
                Email = "NRA@user.com",
                UserName = "NRA@user.com",
                EmailConfirmed = true,
                Geha = Geha.NRA.ToString(),
                Level = Level.LevelOne
            });

            // LevelTwo user (LM)
            users.Add(new User
            {
                Email = "LM@user.com",
                UserName = "LM@user.com",
                EmailConfirmed = true,
                Geha = Geha.LM.ToString(),
                Level = Level.LevelTwo
            });

            // LevelThree user (RO)
            users.Add(new User
            {
                Email = "RO@user.com",
                UserName = "RO@user.com",
                EmailConfirmed = true,
                Geha = Geha.RO.ToString(),
                Level = Level.LevelThree
            });

            // LevelFour user (RA)
            users.Add(new User
            {
                Email = "RA@user.com",
                UserName = "RA@user.com",
                EmailConfirmed = true,
                Geha = Geha.RA.ToString(),
                Level = Level.LevelFour
            });

            // seed if users not exist
            foreach (var user in users)
            {
                if (!await userManager.Users.AnyAsync(u => u.Email == user.Email))
                {
                    await userManager.CreateAsync(user, password);

                    // add to role based on level
                    var role = user.Level switch
                    {
                        Level.LevelZero => RoleConstants.LevelZero,
                        Level.LevelOne => RoleConstants.LevelOne,
                        Level.LevelTwo => RoleConstants.LevelTwo,
                        Level.LevelThree => RoleConstants.LevelThree,
                        Level.LevelFour => RoleConstants.LevelFour,
                        _ => RoleConstants.Admin
                    };

                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}

