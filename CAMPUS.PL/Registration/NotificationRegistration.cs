using CAMPUS.BLL.Notifications;

namespace CAMPUS.PL.Registration;

public static class NotificationRegistration
{
    /// <summary>Registers the notification service. For now the NullNotificationService, which sends nothing.</summary>
    public static IServiceCollection AddCampusNotifications(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<INotificationService, NullNotificationService>();
        return services;
    }
}
