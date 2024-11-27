using LibraryManagement.Application.Contracts.Identity;
using LibraryManagement.Application.Models.Identity;
using LibraryManagement.Identity.DbContext;
using LibraryManagement.Identity.Models;
using LibraryManagement.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LibraryManagement.Identity
{
    public static class IdentityServicesRegistration
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            services.AddDbContext<LmIdentityDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("LmDatabaseConnectionString"))
                    .ConfigureWarnings(warnings =>
                           warnings.Ignore(RelationalEventId.PendingModelChangesWarning))
            );


            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<LmIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IAuthServices, AuthService>();
            services.AddTransient<IUserServices, UserService>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JwtSettings: Issuer"],
                    ValidAudience = configuration["JwtSettings: Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes
                    (configuration["JwtSettings:Key"]))
                };
            });

            return services;

        }

    }
}
