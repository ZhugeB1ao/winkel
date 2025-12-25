using Microsoft.AspNetCore.Identity;

namespace WebApplication1.AppData;

public static class DataSeeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        // Seed Roles
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }
    }

    public static async Task SeedAdminUserAsync(UserManager<AppUser> userManager)
    {
        string adminEmail = "Admin@admin.com";
        string adminPassword = "Admin@123";
        
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var newAdminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "Administrator"
            };
            var result = await userManager.CreateAsync(newAdminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdminUser, "Admin");    
            }
        }
        else
        {
            var roles = await userManager.GetRolesAsync(adminUser);
            if (!roles.Contains("Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}