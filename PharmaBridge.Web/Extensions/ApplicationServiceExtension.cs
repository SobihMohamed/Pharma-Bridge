using PharmaBridge.Domain.Contracts.GenericReposPattern;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Persistence.Implementations.ReposPattern;
using PharmaBridge.Persistence.Implementations.UoWPattern;

using PharmaBridge.Abstraction.IServices.Pharmacy;
using PharmaBridge.Services.ServicesImplementation.Pharmacy;
using PharmaBridge.Abstraction.IServices.Auth;
using PharmaBridge.Services.ServicesImplementation.Auth;

namespace PharmaBridge.Web.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPharmacyProfileService, PharmacyProfileService>();
            
            
            return services;
        }
    }
}
