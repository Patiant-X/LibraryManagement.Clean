using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LibraryManagement.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton(new MapperConfiguration(mc =>
            mc.AddMaps(Assembly.GetExecutingAssembly()))
                .CreateMapper());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}
