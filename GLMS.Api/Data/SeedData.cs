using Microsoft.AspNetCore.Identity;

namespace GLMS.Api.Data
{
    public static class SeedData
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            await SeedRolesAsync(roleManager);
            await SeedUsersAsync(userManager);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private static async Task SeedUsersAsync(UserManager<IdentityUser> userManager)
        {
            await CreateUserAsync(
                userManager,
                email: "admin@techmore.co.za",
                password: "Admin@123",
                role: "Admin"
            );

            await CreateUserAsync(
                userManager,
                email: "user@techmore.co.za",
                password: "User@123",
                role: "User"
            );
        }

        private static async Task CreateUserAsync(
            UserManager<IdentityUser> userManager,
            string email,
            string password,
            string role)
        {
            // Only create if user doesn't already exist
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}