using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.ObjectPool;

namespace MSpace2.Data.Seeders
{
    public static class UserSeeder
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roleNames = { "Admin", "User" };
            foreach (var role in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminUser = new IdentityUser
            {
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
                EmailConfirmed = true
            };

            string password = "AdminLogin123$";
            var user = await userManager.FindByNameAsync(adminUser.Email);
            if (user == null)
            {
                var created = await userManager.CreateAsync(adminUser, password);
                if (created.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
