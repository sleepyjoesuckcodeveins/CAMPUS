using CAMPUS.BLL.Common;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;

namespace CAMPUS.BLL.Interfaces;

public interface IStatusService
{
    // The statuses a request can move to from its current status.
    IReadOnlyList<RequestStatus> GetNextStatuses(RequestStatus current);

    // Changes the status; sets ResolvedAt when it becomes Resolved.
    Task<ServiceResult> ChangeStatusAsync(int requestId, RequestStatus newStatus, User actor);
}