using Microsoft.AspNetCore.Identity;
using AuthenticationService.Models;

namespace AuthenticationService.Data.Seed
{
    public static class IdentitySeeder
    {
        public static async Task SeedIdentityAsync(RoleManager<IdentityRole<Guid>> roleManager, UserManager<ApplicationUser> userManager)
        {
            string[] roles = { "Admin", "User" };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                }
            }

            var adminEmail = "seifmoataz27249@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "SeifAdmin",
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    ActivtyLevel = "Medium" ,  // 🔥 حط أي قيمة Default
                    Goal ="Lose Wighet",
                    Age = 21 ,
                    Height = 180,
                    Weight = 120,
                    Gender="Male",
                    EmailConfirmed = true,
                    PhoneNumber = "01000000000"
                };

                var result = await userManager.CreateAsync(user, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                    Console.WriteLine("Default Admin user created successfully!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed to create default Admin user:");
                    foreach (var error in result.Errors)
                        Console.WriteLine($" - {error.Description}");
                    Console.ResetColor();
                }
            }
        }
    }
}
