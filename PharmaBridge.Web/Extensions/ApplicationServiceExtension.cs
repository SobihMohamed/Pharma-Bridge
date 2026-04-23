using PharmaBridge.Abstraction.IServices.Auth;
using PharmaBridge.Abstraction.IServices.Pharmacy;
using PharmaBridge.Abstraction.IServices.Token;
using PharmaBridge.Domain.Contracts.GenericReposPattern;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.DbInitializer;
using PharmaBridge.Persistence.Implementations.InitializerImplement;
using PharmaBridge.Persistence.Implementations.ReposPattern;
using PharmaBridge.Persistence.Implementations.UoWPattern;
using PharmaBridge.Services.ServicesImplementation.Auth;
using PharmaBridge.Services.ServicesImplementation.Pharmacy;
using SoftBridge.Services.Services.Token;

namespace PharmaBridge.Web.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IDbInitializer, DbInitialized>();
            services.AddScoped<IPharmacyProfileService, PharmacyProfileService>();
            
            
            return services;
        }
    }
}
