using PharmaBridge.Domain.Contracts.GenericReposPattern;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Persistence.Implementations.ReposPattern;
using PharmaBridge.Persistence.Implementations.UoWPattern;

namespace PharmaBridge.Web.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddScoped(typeof(IGenericRepo<,>), typeof(GenericRepo<,>)); // not needed because we have a generic method in unit of work that return the generic repo 

            // specification 
            return services;
        }
    }
}
