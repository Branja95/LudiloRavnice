using AccountManaging.Models.IdentityUsers;
using AccountManaging.Persistance;
using AccountManaging.User;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace AccountManaging.DB
{
    public class EntryData
    {
        public static async Task Initialize(ApplicationDbContext context,
                             UserManager<ApplicationUser> userManager,
                             RoleManager<ApplicationRole> roleManager)
        {
            context.Database.EnsureCreated();

            string adminRole = "Administrator";
            string managerRole = "Manager";
            string clientRole = "Client";

            if (await roleManager.FindByNameAsync(adminRole) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(adminRole));
            }
            if (await roleManager.FindByNameAsync(managerRole) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(managerRole));
            }
            if (await roleManager.FindByNameAsync(clientRole) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(clientRole));
            }

            if (await userManager.FindByEmailAsync("david.djekic@gmail.com") == null)
            {
                ApplicationUser adminOne = new ApplicationUser
                {
                    UserName = "thelood",
                    Email = "david.djekic@gmail.com",
                    FirstName = "David",
                    LastName = "Djekic",
                    DateOfBirth = DateTime.Parse("02.06.1995."),
                    EmailConfirmed = true,
                    IsApproved = true
                };
                adminOne.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(adminOne, "David123");

                IdentityResult result = await userManager.CreateAsync(adminOne);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(adminOne, adminOne.PasswordHash);
                    await userManager.AddToRoleAsync(adminOne, adminRole);
                }
            }

            if (await userManager.FindByEmailAsync("branici@gmail.com") == null)
            {
                ApplicationUser adminTwo = new ApplicationUser
                {
                    UserName = "branja",
                    Email = "branici@gmail.com",
                    FirstName = "Branko",
                    LastName = "Jelic",
                    DateOfBirth = DateTime.Parse("07.09.1995."),
                    EmailConfirmed = true,
                    IsApproved = true
                };
                adminTwo.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(adminTwo, "Branko123");

                IdentityResult result = await userManager.CreateAsync(adminTwo);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(adminTwo, adminTwo.PasswordHash);
                    await userManager.AddToRoleAsync(adminTwo, adminRole);
                }
            }
        }
    }
}
