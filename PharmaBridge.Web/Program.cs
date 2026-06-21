
using Microsoft.OpenApi.Models;
using PharmaBridge.Abstraction.IServices.Attachement;
using PharmaBridge.Abstraction.IServices.PatientAddresses;
using PharmaBridge.Abstraction.IServices.Pharmacy;
using PharmaBridge.Abstraction.IServices.Pharmacy;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Persistence.Extensions;
using PharmaBridge.Persistence.Extensions;
using PharmaBridge.Persistence.ProgramService;
using PharmaBridge.Presentation.Extensions;
using PharmaBridge.Services.AutoMapper;
using PharmaBridge.Services.Resolver;
using PharmaBridge.Services.ServicesImplementation.Attachement;
using PharmaBridge.Services.ServicesImplementation.PatientAddress;
using PharmaBridge.Shared.DTOs.Pharmacy;
using PharmaBridge.Shared.EnumHelper.UserEnums;
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


            builder.Services.AddScoped<IPatientAddressService, PatientAddressService>();
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

                //builder.Services.AddCors(options =>
                //{
                //    options.AddPolicy("DevPolicy", policy =>
                //    {
                //        policy.AllowAnyOrigin()
                //              .AllowAnyMethod()
                //              .AllowAnyHeader();
                //    });


                });

            var app = builder.Build();
                await app.SeedDatabaseAsync();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwaggerDocumentation();
                }
            
                //app.UseCors("DevPolicy");

                // add middleware for global exception handling
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