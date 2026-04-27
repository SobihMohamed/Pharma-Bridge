using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PharmaBridge.Domain.DbInitializer;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Persistence.Pharma_BridgeDbContext;
using PharmaBridge.Persistence.Seeds;

namespace PharmaBridge.Persistence.Implementations.InitializerImplement
{
    public class DbInitialized(PharmaDbContext projectDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) : IDbInitializer
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
            // seed roles
            await SeederAsync.SeedRolesAsync(roleManager);
            // seed admin
            await SeederAsync.SeedAdminUserAsync(userManager);
            // seed dummy users
            await SeederAsync.SeedDummyUsersAsync(userManager);
            
            await SeederAsync.SeedOrderTestDataAsync(projectDbContext);
        }
    }
}