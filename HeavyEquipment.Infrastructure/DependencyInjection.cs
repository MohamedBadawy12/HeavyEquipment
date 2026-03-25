using HeavyEquipment.Domain.Interfaces;
using HeavyEquipment.Infrastructure.Persistence.Data;
using HeavyEquipment.Infrastructure.Persistence.Repositories;
using HeavyEquipment.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HeavyEquipment.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
                ));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEmailService, SendGridEmailService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IImageService, LocalImageService>();
            services.AddTransient<ISmsService, TwilioSmsService>();

            return services;
        }
    }
}
