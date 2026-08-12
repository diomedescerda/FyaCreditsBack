using FyaCredits.Application;
using FyaCredits.Infrastructure.Notifications;
using FyaCredits.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FyaCredits.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<ICreditRepository, CreditRepository>();
        services.AddSingleton<NotificationQueue>();
        services.AddSingleton<INotificationQueue>(provider => provider.GetRequiredService<NotificationQueue>());
        services.AddHttpClient("EmailRenderer");
        services.AddSingleton<IEmailTemplateRenderer, EmailHtmlRenderer>();
        services.AddSingleton<IEmailSender, SmtpEmailSender>();
        services.AddHostedService<NotificationWorker>();

        return services;
    }
}
