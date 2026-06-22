using Microsoft.Extensions.DependencyInjection;
using PharmaBridge.Services.AutoMapper.AuthMapping;
using PharmaBridge.Services.AutoMapper.ComplaintMapping;
using PharmaBridge.Services.AutoMapper.OrderMapping;
using PharmaBridge.Services.AutoMapper.PatientAddressMapping;
using PharmaBridge.Services.AutoMapper.PharmacyMapping;
using PharmaBridge.Services.AutoMapper.PatientMapping;
using PharmaBridge.Services.AutoMapper.PrescriptionRequestMapping;
using PharmaBridge.Services.AutoMapper.NotificationMapping;

namespace PharmaBridge.Services.AutoMapper
{
    public static class AutoMapperService
    {
        public static IServiceCollection InjectAutoMapperService(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(new AuthProfile());
                cfg.AddProfile(new PrescriptionRequestProfile());
                cfg.AddProfile(new BidMapping.BidProfile());
                cfg.AddProfile(new ComplaintProfile());
                cfg.AddProfile(new AdminComplaintProfile());
                cfg.AddProfile(new PharmacyProfileMapping());
                cfg.AddProfile(new OrderMappingProfile());
                cfg.AddProfile(new PatientProfileMapping());
                cfg.AddProfile(new NotificationProfile());



                cfg.AddProfile(new PatientAddressProfile());
            });
            return services;
        }
    }
}
