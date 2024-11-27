using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Persistence.DatabaseContext;
using LibraryManagement.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagement.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<LmDatabaseContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("LmDatabaseConnectionString"));
        });

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        return services;
    }
}
