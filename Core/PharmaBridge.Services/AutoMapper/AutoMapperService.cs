using Microsoft.Extensions.DependencyInjection;
using PharmaBridge.Services.AutoMapper.AuthMapping;
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
            });
            return services;
        }
    }
}
