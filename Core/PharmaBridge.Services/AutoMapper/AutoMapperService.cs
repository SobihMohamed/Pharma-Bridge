using Microsoft.Extensions.DependencyInjection;
using PharmaBridge.Services.AutoMapper.AuthMapping;
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
            });
            return services;
        }
    }
}
