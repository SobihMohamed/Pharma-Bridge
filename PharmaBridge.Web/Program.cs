
using PharmaBridge.Persistence.ProgramService;
using PharmaBridge.Services.AutoMapper;
using PharmaBridge.Web.Extensions;
using PharmaBridge.Web.Middleware;

namespace PharmaBridge.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // get database config
            builder.Services.InjectDatabaseService(builder.Configuration);
            // get from identity Layer in web project (Identity Core)
            builder.Services.InjectIdentityCore();
            // get th application services 
            builder.Services.AddApplicationService();
            // inject the Rate Limiting Service
            builder.Services.InjectRateLimiting();
            // inject automapper
            builder.Services.InjectAutoMapperService();
            // Add services to the container.
            builder.Services.AddControllers();
            // Add Data Protection services
            builder.Services.AddDataProtection();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
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
