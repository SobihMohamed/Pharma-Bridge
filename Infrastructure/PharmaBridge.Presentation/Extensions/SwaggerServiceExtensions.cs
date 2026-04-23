using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace PharmaBridge.Presentation.Extensions
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "PharmaBridge API",
                    Version = "v1",
                    Description = "API documentation for PharmaBridge application.",
                    Contact = new OpenApiContact
                    {
                        Name = "PharmaBridge Support",
                        Email = "sobihmohamedsobih@gmail.com"
                    }
                });

                // 2.(JWT Setup)
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter your JWT Token only (without 'Bearer' prefix).",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                };

                options.AddSecurityDefinition("Bearer", securityScheme);

                // 3. Add Security Requirement to include JWT in Swagger UI
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        securityScheme,
                        new List<string>()
                    }
                });
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            // تم التعديل لـ PharmaBridge
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PharmaBridge API v1"));
            return app;
        }
    }
}