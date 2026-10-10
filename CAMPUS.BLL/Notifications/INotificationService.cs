using CAMPUS.DOMAIN.Entities;

namespace CAMPUS.BLL.Notifications;

public interface INotificationService
{
    /// <summary>E-mails every facilities manager that a new request was submitted.</summary>
    Task RequestSubmittedAsync(ServiceRequest request);

    /// <summary>E-mails the reporter and every member of the assigned team that the request was assigned.</summary>
    Task RequestAssignedAsync(ServiceRequest request);
}
