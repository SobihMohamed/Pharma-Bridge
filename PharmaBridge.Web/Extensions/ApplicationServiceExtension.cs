using PharmaBridge.Abstraction.IServices.Attachement;
using PharmaBridge.Abstraction.IServices.Auth;
using PharmaBridge.Abstraction.IServices.CurrentUser;
using PharmaBridge.Abstraction.IServices.Order;
using PharmaBridge.Abstraction.IServices.Pharmacy;
using PharmaBridge.Abstraction.IServices.PrescriptionRequest;
using PharmaBridge.Abstraction.IServices.Token;
using PharmaBridge.Domain.Contracts.GenericReposPattern;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.DbInitializer;
using PharmaBridge.Persistence.Implementations.InitializerImplement;
using PharmaBridge.Persistence.Implementations.ReposPattern;
using PharmaBridge.Persistence.Implementations.UoWPattern;
using PharmaBridge.Services.Resolver;
using PharmaBridge.Services.ServicesImplementation.Attachement;
using PharmaBridge.Services.ServicesImplementation.Auth;
using PharmaBridge.Services.ServicesImplementation.CurrentUser;
using PharmaBridge.Services.ServicesImplementation.OrderService;
using PharmaBridge.Services.ServicesImplementation.Pharmacy;
using PharmaBridge.Services.ServicesImplementation.PrescriptionRequest;
using SoftBridge.Services.Services.Token;
using System.Text.Json.Serialization;

namespace PharmaBridge.Web.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAttachementService, AttachmentService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IDbInitializer, DbInitialized>();
            services.AddScoped<IPrescriptionRequestService, PrescriptionRequestService>();
            services.AddScoped<IPharmacyProfileService, PharmacyProfileService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPharmacyRatingService, PharmacyRatingService>();
            services.AddHttpContextAccessor();


            services.AddScoped<ICurrentUserService, CurrentUserService>();
            

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // convert the enum from num to string
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddTransient(typeof(PictureResolver<,>));
            return services;
        }
    }
}