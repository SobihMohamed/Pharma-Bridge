using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Persistence.Pharma_BridgeDbContext;

namespace PharmaBridge.Persistence.ProgramService
{
    public static class AddDbService
    {
        public static IServiceCollection InjectDatabaseService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<PharmaDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<PharmaDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}