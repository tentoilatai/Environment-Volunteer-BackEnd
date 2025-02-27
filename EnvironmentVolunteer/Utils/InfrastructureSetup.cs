using EnvironmentVolunteer.DataAccess.DbContexts;
using EnvironmentVolunteer.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EnvironmentVolunteer.Api.Utils
{
    public static class InfrastructureSetup
    {
        public static IHost MigrateDatabase(this IHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var webHostEnvironment = services.GetRequiredService<IWebHostEnvironment>();
                using (var context = services.GetRequiredService<EnvironmentVolunteerDbContext>())
                {
                    try
                    {
                        context.Database.Migrate();
                        context.SeedData(webHostEnvironment);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }

            return webHost;
        }

        public static void SeedData(this EnvironmentVolunteerDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            if (dbContext.Users.Any())
            {
                return; // DB has been seeded
            }

            var userManager = dbContext.GetService<UserManager<User>>();
            var roleManager = dbContext.GetService<RoleManager<Role>>();

            // Create role if it do not have 
            if (!dbContext.Roles.Any())
            {
                roleManager.CreateAsync(new Role { Name = "Admin" }).Wait();
                roleManager.CreateAsync(new Role { Name = "User" }).Wait();

            }

            var admin = new User
            {
                Id = Guid.NewGuid(),
                UserName = "admin",
                NameProfile = "Admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@envi.com",
                NormalizedEmail = "ADMIN@ENVI.COM",
                EmailConfirmed = true,
                IsDeleted = false,
                IsLogin = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = Guid.NewGuid(),
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            var result = userManager.CreateAsync(admin, "Envi2025@2025@2025").Result;

            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(admin, "Admin").Wait(); // assign Admin permission for this user
            }

            dbContext.SaveChanges();
        }

    }
}
