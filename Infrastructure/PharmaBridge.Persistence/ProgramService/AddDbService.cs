using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PharmaBridge.Persistence.Pharma_BridgeDbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Persistence.ProgramService
{
    public static class AddDbService
    {
        public static IServiceCollection InjectDatabaseService(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddDbContext<PharmaDbContext>(options => 
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            return services;
        }
    }
}
