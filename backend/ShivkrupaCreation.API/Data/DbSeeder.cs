using Microsoft.AspNetCore.Identity;
using ShivkrupaCreation.API.Models;

namespace ShivkrupaCreation.API.Data
{
    /// <summary>
    /// Seeds Identity roles (Admin, Customer) and a default admin user
    /// so the Shivkrupa Creation admin panel is usable immediately after
    /// the first migration + run.
    /// </summary>
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            foreach (var role in new[] { Roles.Admin, Roles.Customer })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            const string adminEmail = "admin@shivkrupacreation.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser is null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Shivkrupa Creation Admin",
                    EmailConfirmed = true,
                    IsActive = true
                };

                // NOTE: change this password immediately after first login in production
                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, Roles.Admin);
                }
            }
        }
    }
}
