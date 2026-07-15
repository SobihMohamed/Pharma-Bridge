using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PharmaBridge.Domain.DbInitializer;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Persistence.Pharma_BridgeDbContext;
using PharmaBridge.Persistence.Seeds;

namespace PharmaBridge.Persistence.Implementations.InitializerImplement
{
    public class DbInitialized(
    PharmaDbContext projectDbContext,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration) : IDbInitializer
    {
        public async Task DataSeedAsync()
        {
            try
            {
                var pendingMigrations = await projectDbContext.Database.GetPendingMigrationsAsync();
                if (pendingMigrations != null && pendingMigrations.Any())
                    await projectDbContext.Database.MigrateAsync();
            }
            catch (Exception)
            {
                // Log the exception or handle it as needed
                throw;
            }
            // Seed Roles (Always)
            await SeederAsync.SeedRolesAsync(roleManager);

            // Seed Admin (Always)
            await SeederAsync.SeedAdminUserAsync(userManager);

            // Read configuration
            var enableDemoData = configuration.GetValue<bool>("Seeding:EnableDemoData");

            // Seed demo data only when enabled
            if (enableDemoData)
            {
                await SeederAsync.SeedDummyUsersAsync(userManager);
                await SeederAsync.SeedOrderTestDataAsync(projectDbContext);
            }
        }
    }
}   