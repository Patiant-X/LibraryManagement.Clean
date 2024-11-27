using LibraryManagement.Application.Contracts.Email;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Models.Email;
using LibraryManagement.Infrastructure.EmailService;
using LibraryManagement.Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace LibraryManagement.Infrastrure
{
    public static class InfrastructureServicesRegistrations
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
            IConfiguration configuration
            )
        {
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddTransient<IEmailSender, EmailNotificationSender>();
            services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdopter<>));
            services.AddHostedService<BookReservationExpiryService>();
            return services;
        }
    }
}
