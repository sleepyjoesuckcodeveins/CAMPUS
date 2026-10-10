using CAMPUS.DOMAIN.Entities;

namespace CAMPUS.BLL.Notifications;

public class NullNotificationService : INotificationService
{
    public Task RequestSubmittedAsync(ServiceRequest request) => Task.CompletedTask;

    public Task RequestAssignedAsync(ServiceRequest request) => Task.CompletedTask;
}
