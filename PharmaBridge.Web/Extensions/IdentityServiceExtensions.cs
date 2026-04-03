using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Persistence.Pharma_BridgeDbContext;

namespace PharmaBridge.Web.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection InjectIdentityCore(this IServiceCollection services) 
        {
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                // var otp = await _usermanager.GenratePasswordRestTokenAsync(user);
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<PharmaDbContext>()
                .AddDefaultTokenProviders();
            return services;
        }

        public static IServiceCollection InjectRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("OtpPolicy", opt =>
                {
                    opt.Window = TimeSpan.FromMinutes(2); // time to expire is 2 min
                    opt.PermitLimit = 3; // only 3 request is allowed
                    opt.QueueLimit = 0; // no queuing, reject immediately when limit is reached
                });
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });
            return services;
        }
    }
}
