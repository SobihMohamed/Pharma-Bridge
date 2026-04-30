using Microsoft.Extensions.DependencyInjection;
using PharmaBridge.Services.AutoMapper.AuthMapping;
using PharmaBridge.Services.AutoMapper.ComplaintMapping;
using PharmaBridge.Services.AutoMapper.OrderMapping;
using PharmaBridge.Services.AutoMapper.PharmacyMapping;
using PharmaBridge.Services.AutoMapper.PrescriptionRequestMapping;
using System;
using System.Collections.Generic;
using System.Text;

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
                cfg.AddProfile(new PharmaBridge.Services.AutoMapper.PharmacyMapping.PharmacyProfileMapping());
                cfg.AddProfile(new ComplaintProfile());
                cfg.AddProfile(new AdminComplaintProfile());
                cfg.AddProfile(new PharmaBridge.Services.AutoMapper.BidMapping.BidProfile());
                cfg.AddProfile(new PharmacyProfileMapping());
                cfg.AddProfile(new OrderMappingProfile());
            });
            return services;
        }
    }
}
