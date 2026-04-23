
using PharmaBridge.Abstraction.IServices.Attachement;
using PharmaBridge.Abstraction.IServices.Pharmacy;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Persistence.ProgramService;
using PharmaBridge.Services.AutoMapper;
using PharmaBridge.Shared.DTOs.Pharmacy;
using PharmaBridge.Shared.EnumHelper.UserEnums;
using PharmaBridge.Web.Extensions;
using PharmaBridge.Web.Middleware;
using Scalar.AspNetCore;
using PharmaBridge.Services.ServicesImplementation.Attachement;
namespace PharmaBridge.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // get database config
            builder.Services.InjectDatabaseService(builder.Configuration);
            // get from identity Layer in web project (Identity Core)
            builder.Services.InjectIdentityCore();
            // get th application services 
            builder.Services.AddApplicationService();
            builder.Services.AddScoped<IAttachementService, AttachmentService>();
            builder.Services.InjectRateLimiting();
            // inject automapper
            builder.Services.InjectAutoMapperService();
            // Add services to the container.
            builder.Services.AddControllers()
             .AddApplicationPart(typeof(PharmaBridge.Presentation.Controllers.PharmacyController).Assembly);
            // Add Data Protection services
            builder.Services.AddDataProtection();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
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
