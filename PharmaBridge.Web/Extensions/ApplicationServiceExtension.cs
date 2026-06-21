using Microsoft.Extensions.DependencyInjection;
using PharmaBridge.Abstraction.IServices.Attachement;
using PharmaBridge.Abstraction.IServices.Auth;
using PharmaBridge.Abstraction.IServices.Bidding;
using PharmaBridge.Abstraction.IServices.Complaint;
using PharmaBridge.Abstraction.IServices.Notification;
using PharmaBridge.Abstraction.IServices.Order;
using PharmaBridge.Abstraction.IServices.PatientProfiles;
using PharmaBridge.Abstraction.IServices.Pharmacy;
using PharmaBridge.Abstraction.IServices.PrescriptionRequest;
using PharmaBridge.Abstraction.IServices.Token;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.DbInitializer;
using PharmaBridge.Persistence.Implementations.InitializerImplement;
using PharmaBridge.Persistence.Implementations.SpecificReposPattern;
using PharmaBridge.Domain.Contracts.SpecificReposPattern;
using PharmaBridge.Persistence.Implementations.UoWPattern;
using PharmaBridge.Services.Bidding;
using PharmaBridge.Services.Resolver;
using PharmaBridge.Services.ServicesImplementation.Attachement;
using PharmaBridge.Services.ServicesImplementation.Auth;
using PharmaBridge.Services.ServicesImplementation.Complaint;
using PharmaBridge.Services.ServicesImplementation.Notification;
using PharmaBridge.Services.ServicesImplementation.Notification.StrategyPattern;
using PharmaBridge.Services.ServicesImplementation.OrderService;
using PharmaBridge.Services.ServicesImplementation.Patient;
using PharmaBridge.Services.ServicesImplementation.Pharmacy;
using PharmaBridge.Services.ServicesImplementation.PrescriptionRequest;
using PharmaBridge.Web.Hubs;
using SoftBridge.Services.Services.Token;
using System.Text.Json.Serialization;

namespace PharmaBridge.Web.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            // 1. Core & Infrastructure
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDbInitializer, DbInitialized>();

            // 2. (Strategy Pattern)
            services.AddScoped<INotificationStrategy, PushedNotificationStrategy>();

            // Notification Hubs
            services.AddScoped<IWebNotificationPusher, WebNotificationPusher>();
            services.AddSignalR();

            // 3. Application Services
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAttachementService, AttachmentService>();
            services.AddScoped<IPrescriptionRequestService, PrescriptionRequestService>();
            services.AddScoped<IPharmacyProfileService, PharmacyProfileService>();
            services.AddScoped<IBidService, BidService>();
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



            services.AddScoped<IPatientProfileService, PatientProfileService>();

            services.AddScoped<IComplaintService, ComplaintService>();
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