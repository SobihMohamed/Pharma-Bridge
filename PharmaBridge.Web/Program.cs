
//using PharmaBridge.Abstraction.IServices.Attachement;

//using PharmaBridge.Abstraction.IServices.Pharmacy;
//using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
//using PharmaBridge.Persistence.Extensions;
//using PharmaBridge.Persistence.ProgramService;
//using PharmaBridge.Presentation.Extensions;
//using PharmaBridge.Services.AutoMapper;
//using PharmaBridge.Web.Extensions;
//using PharmaBridge.Web.Middleware;
//using Scalar.AspNetCore;
//using PharmaBridge.Services.ServicesImplementation.Attachement;
//using System.Text.Json.Serialization;
//using Microsoft.Extensions.DependencyInjection;

//namespace PharmaBridge.Web
//{
//    public class Program
//    {
//        public static async Task Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // get database config
//            builder.Services.InjectDatabaseService(builder.Configuration);

//            builder.Services.InjectIdentityCore();
//            builder.Services.AddApplicationService();
//            builder.Services.AddScoped<IAttachementService, AttachmentService>();
//            builder.Services.InjectRateLimiting();
//            builder.Services.InjectAutoMapperService();

//            // Add controllers, application parts (Scalar fix), and JSON options (Enum fix) all together
//            builder.Services.AddControllers()
//                .AddApplicationPart(typeof(PharmaBridge.Presentation.Controllers.PharmacyController).Assembly)
//                .AddJsonOptions(options =>
//                {
//                    // convert the enum from num to string
//                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
//                });

//            // Add Data Protection services (Only once)
//            builder.Services.AddDataProtection();


//            // 💡 swagger configuration (Clean & Simple)
//            builder.Services.AddSwaggerDocumentation();

//            var app = builder.Build();
//            await app.SeedDatabaseAsync();

//            // Configure the HTTP request pipeline.
//            if (app.Environment.IsDevelopment())
//            {
//                app.UseSwaggerDocumentation();
//            }

//            // add middleware for global exception handling
//            app.UseMiddleware<GlobalErrorHandlerMiddleware>();
//            app.UseHttpsRedirection();

//            app.UseAuthentication();
//            app.UseAuthorization();

//            app.UseStaticFiles();
//            app.MapControllers();
//            app.Run();
//        }
//    }
//}
//using PharmaBridge.Abstraction.IServices.Pharmacy;
//using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
//using PharmaBridge.Persistence.Extensions;
//using PharmaBridge.Persistence.ProgramService;
//using PharmaBridge.Presentation.Extensions;
//using PharmaBridge.Services.AutoMapper;
//using PharmaBridge.Web.Extensions;
//using PharmaBridge.Web.Middleware;
//using System.Text.Json.Serialization;

//namespace PharmaBridge.Web
//{
//    public class Program
//    {
//        public static async Task Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // get database config
//            builder.Services.InjectDatabaseService(builder.Configuration);

//            builder.Services.InjectIdentityCore();
//            builder.Services.AddApplicationService();

//            builder.Services.InjectRateLimiting();
//            builder.Services.InjectAutoMapperService();

//            // Custom Extensions (Security & CORS)
//            builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment);
//            builder.Services.AddCustomCors(builder.Configuration);


//            builder.Services.AddDataProtection();

//            // 💡 swagger configuration (Clean & Simple)
//            builder.Services.AddSwaggerDocumentation();

//            var app = builder.Build();
//            await app.SeedDatabaseAsync();

//            // Configure the HTTP request pipeline.
//            if (app.Environment.IsDevelopment())
//            {
//                app.UseSwaggerDocumentation();
//            }

//            // add middleware for global exception handling
//            app.UseMiddleware<GlobalErrorHandlerMiddleware>();
//            app.UseHttpsRedirection();

//            app.UseAuthentication();
//            app.UseAuthorization();

//            app.UseStaticFiles();
//            app.MapControllers();
//            app.Run();
//        }
//    }
//}
using Microsoft.OpenApi.Models;
using PharmaBridge.Abstraction.IServices.Attachement;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Persistence.Extensions;
using PharmaBridge.Persistence.ProgramService;
using PharmaBridge.Presentation.Extensions;
using PharmaBridge.Services.AutoMapper;
using PharmaBridge.Services.Resolver;
using PharmaBridge.Services.ServicesImplementation.Attachement;
using PharmaBridge.Web.Extensions;
using PharmaBridge.Web.Middleware;
using System.Text.Json.Serialization;

namespace PharmaBridge.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.InjectDatabaseService(builder.Configuration);
            builder.Services.InjectIdentityCore();

            // 2. الخدمات الأساسية
            builder.Services.AddApplicationService();
            builder.Services.AddScoped<IAttachementService, AttachmentService>();

            builder.Services.AddHttpContextAccessor();                      
            builder.Services.AddScoped(typeof(PictureResolver<,>));         

            builder.Services.InjectRateLimiting();
            builder.Services.InjectAutoMapperService();

            // 💡 السطر ده هو اللي هيحل الإيرور بتاعك (تسجيل خدمة رفع الصور)
            builder.Services.AddScoped<IAttachementService, AttachmentService>();

            builder.Services.InjectRateLimiting();
            builder.Services.InjectAutoMapperService();

            // 3. الحماية والـ CORS
            builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment);
            builder.Services.AddCustomCors(builder.Configuration);

            // 💡 السطر ده عشان السيرفر يقرا الـ Controllers وميضربش 404
            builder.Services.AddControllers()
                .AddApplicationPart(typeof(PharmaBridge.Presentation.Controllers.PharmacyController).Assembly)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            builder.Services.AddDataProtection();

            // 💡 4. إعدادات Swagger عشان "القفل" يظهر وتقدر تحط التوكن
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PharmaBridge API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "دخل التوكن هنا (مفيش داعي تكتب كلمة Bearer)"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] {}
                    }
                });
            });

            var app = builder.Build();
            await app.SeedDatabaseAsync();

            // 5. الـ Middleware Pipeline (الترتيب هنا مهم جداً)
            app.UseMiddleware<GlobalErrorHandlerMiddleware>();
            app.UseHttpsRedirection();

            // تشغيل واجهة Swagger
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseStaticFiles();
            app.UseRouting();

            // تأكد إن اسم الـ Policy هنا مطابق للي جوه AddCustomCors
            app.UseCors("CorsPolicy");

            // 💡 التأكد من الهوية والصلاحيات
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}