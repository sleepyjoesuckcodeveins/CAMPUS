using CAMPUS.DOMAIN.Entities;

namespace CAMPUS.BLL.Notifications;

public interface INotificationService
{
    Task RequestSubmittedAsync(ServiceRequest request);

    Task RequestAssignedAsync(ServiceRequest request);
}
