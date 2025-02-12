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

            var user = new User
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

            userManager.CreateAsync(user, "Envi2025@2025@2025").Wait();

            dbContext.SaveChanges();
        }

    }
}
