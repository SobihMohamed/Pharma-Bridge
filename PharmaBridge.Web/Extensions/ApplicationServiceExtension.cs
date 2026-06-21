using Microsoft.Extensions.DependencyInjection;
using PharmaBridge.Abstraction.IServices.Attachement;
using PharmaBridge.Abstraction.IServices.Auth;
using PharmaBridge.Abstraction.IServices.Complaint;
using PharmaBridge.Abstraction.IServices.CurrentUser;
using PharmaBridge.Abstraction.IServices.Notification;
using PharmaBridge.Abstraction.IServices.Order;
using PharmaBridge.Abstraction.IServices.Pharmacy;
using PharmaBridge.Abstraction.IServices.PrescriptionRequest;
using PharmaBridge.Abstraction.IServices.Token;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.DbInitializer;
using PharmaBridge.Persistence.Implementations.InitializerImplement;
using PharmaBridge.Persistence.Implementations.ReposPattern;
using PharmaBridge.Persistence.Implementations.SpecificReposPattern;
using PharmaBridge.Domain.Contracts.SpecificReposPattern;
using PharmaBridge.Persistence.Implementations.UoWPattern;
using PharmaBridge.Services.Resolver;
using PharmaBridge.Services.ServicesImplementation.Attachement;
using PharmaBridge.Services.ServicesImplementation.Auth;
using PharmaBridge.Services.ServicesImplementation.Complaint;
using PharmaBridge.Services.ServicesImplementation.CurrentUser;
using PharmaBridge.Services.ServicesImplementation.Notification;
using PharmaBridge.Services.ServicesImplementation.Notification.StrategyPattern;
using PharmaBridge.Services.ServicesImplementation.OrderService;
using PharmaBridge.Services.ServicesImplementation.Pharmacy;
using PharmaBridge.Services.ServicesImplementation.PrescriptionRequest;
using PharmaBridge.Web.Hubs;
using SoftBridge.Services.Services.Token;

namespace PharmaBridge.Web.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            // 1. Core & Infrastructure
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDbInitializer, DbInitialized>();
            // 2. (Strategy Pattern)
            services.AddScoped<INotificationStrategy, PushedNotificationStrategy>();
            //services.AddScoped<INotificationStrategy, EmailNotificationStrategy>();

            // Notification Hubs
            services.AddScoped<IWebNotificationPusher, WebNotificationPusher>();
            services.AddSignalR();

            // 3. Application Services
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAttachementService, AttachmentService>();
            services.AddScoped<IPrescriptionRequestService, PrescriptionRequestService>();
            services.AddScoped<IPharmacyProfileService, PharmacyProfileService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPharmacyRatingService, PharmacyRatingService>();
            services.AddScoped<IPharmacyDashboardService, PharmacyDashboardService>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IComplaintService, ComplaintService>();

            // 4. Helpers & Resolvers
            services.AddHttpContextAccessor();
            services.AddScoped(typeof(PictureResolver<,>));

            // 5. MediatR Registration 
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(PrescriptionRequestService).Assembly);
            });

            return services;
        }
    }
}