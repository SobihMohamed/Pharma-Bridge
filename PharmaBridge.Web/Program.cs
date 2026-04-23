
using PharmaBridge.Abstraction.IServices.Attachement;

using PharmaBridge.Abstraction.IServices.Pharmacy;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Persistence.Extensions;
using PharmaBridge.Persistence.ProgramService;
using PharmaBridge.Presentation.Extensions; 
using PharmaBridge.Services.AutoMapper;
using PharmaBridge.Web.Extensions;
using PharmaBridge.Web.Middleware;
using Scalar.AspNetCore;
using PharmaBridge.Services.ServicesImplementation.Attachement;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace PharmaBridge.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // get database config
            builder.Services.InjectDatabaseService(builder.Configuration);
            builder.Services.InjectIdentityCore();
            builder.Services.AddApplicationService();
            builder.Services.AddScoped<IAttachementService, AttachmentService>();
            builder.Services.InjectRateLimiting();
            builder.Services.InjectAutoMapperService();

            // Add controllers, application parts (Scalar fix), and JSON options (Enum fix) all together
            builder.Services.AddControllers()
                .AddApplicationPart(typeof(PharmaBridge.Presentation.Controllers.PharmacyController).Assembly)
                .AddJsonOptions(options =>
                {
                    // convert the enum from num to string
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            // Add Data Protection services (Only once)
            builder.Services.AddDataProtection();

           
            // 💡 swagger configuration (Clean & Simple)
            builder.Services.AddSwaggerDocumentation();

            var app = builder.Build();
            await app.SeedDatabaseAsync();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerDocumentation();
            }

            // add middleware for global exception handling
            app.UseMiddleware<GlobalErrorHandlerMiddleware>();
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();
            app.MapControllers();
            app.Run();
        }
    }
}