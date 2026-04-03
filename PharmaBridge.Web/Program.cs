
using PharmaBridge.Persistence.ProgramService;
using PharmaBridge.Web.Extensions;

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

            app.UseHttpsRedirection();

            app.UseAuthorization();
            //Test2

            app.MapControllers();

            app.Run();
        }
    }
}
