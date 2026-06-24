using Microsoft.OpenApi.Models;
using PharmaBridge.Domain.DbInitializer;
using PharmaBridge.Persistence.Extensions;
using PharmaBridge.Persistence.ProgramService;
using PharmaBridge.Presentation.Extensions;
using PharmaBridge.Services.AutoMapper;
using PharmaBridge.Web.Extensions;
using PharmaBridge.Web.Hubs;
using PharmaBridge.Web.Middleware;
using System.Text.Json.Serialization;

namespace PharmaBridge.Web
{
    /// <summary>
    /// ???? ????? ??????
    /// ////////////////////////////LAST Version////////////////////////////////////////////////
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ==========================================
            // 1. Database & Identity
            // ==========================================
            builder.Services.InjectDatabaseService(builder.Configuration);
            builder.Services.InjectIdentityCore();

            // ==========================================
            // 2. Application Services & Third-Party
            // ==========================================
            builder.Services.AddApplicationService();
            builder.Services.InjectAutoMapperService();
            builder.Services.InjectRateLimiting();

            // ==========================================
            // 3. Security, CORS, & Protection
            // ==========================================
            builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment);
            builder.Services.AddCustomCors(builder.Configuration); 
            builder.Services.AddDataProtection();

            // ==========================================
            // 4. Controllers & JSON Options
            // ==========================================
            builder.Services.AddControllers()
                .AddApplicationPart(typeof(PharmaBridge.Presentation.Controllers.PharmacyController).Assembly)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            // ==========================================
            // 5. Swagger Setup
            // ==========================================
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PharmaBridge API", Version = "v1" });

                // Add JWT Authentication (Authorize button)
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your valid JWT token below.\nExample: 'eyJhbGciOiJIUzI1NiIsInR...'"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            var app = builder.Build();

            // ==========================================
            // 6. Database Initialization (Seeding)
            // ==========================================
            await app.SeedDatabaseAsync();

            // ==========================================
            // 7. HTTP Request Pipeline (Middleware)
            // ==========================================

            app.UseMiddleware<GlobalErrorHandlerMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerDocumentation();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<NotificationHub>("/notify");
            app.Run();
        }
    }
}