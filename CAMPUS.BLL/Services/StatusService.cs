using CAMPUS.BLL.Common;
using CAMPUS.BLL.Interfaces;
using CAMPUS.DOMAIN;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;
using CAMPUS.DOMAIN.Repositories;

namespace CAMPUS.BLL.Services;

public class StatusService : IStatusService
{
    private readonly IServiceRequestRepository _requests;
    private readonly TimeProvider _time;

    public StatusService(IServiceRequestRepository requests, TimeProvider time)
    {
        _requests = requests;
        _time = time;
    }

    public IReadOnlyList<RequestStatus> GetNextStatuses(RequestStatus current)
    {
        return current switch
        {
            RequestStatus.Open => new[] { RequestStatus.InProgress, RequestStatus.Resolved },
            RequestStatus.InProgress => new[] { RequestStatus.Resolved },
            _ => Array.Empty<RequestStatus>()
        };
    }

    public async Task<ServiceResult> ChangeStatusAsync(int requestId, RequestStatus newStatus, User actor)
    {
        var request = await _requests.GetByIdAsync(requestId);
        if (request is null)
            return ServiceResult.NotFound(Messages.RequestNotFound);

        if (!CanChangeStatus(request, actor))
            return ServiceResult.Forbidden();

        if (!Enum.IsDefined(newStatus))
            return ServiceResult.Invalid(Messages.StatusRequired, nameof(newStatus));

        if (request.Status == RequestStatus.Resolved)
            return ServiceResult.Invalid(Messages.RequestAlreadyResolved);

        if (request.Status == newStatus)
            return ServiceResult.Invalid(Messages.SameStatus, nameof(newStatus));

        if (!GetNextStatuses(request.Status).Contains(newStatus))
            return ServiceResult.Invalid(Messages.StatusChangeNotAllowed, nameof(newStatus));

        var now = Now();
        request.Status = newStatus;
        request.UpdatedAt = now;
        if (newStatus == RequestStatus.Resolved)
            request.ResolvedAt = now;

        await _requests.UpdateAsync(request);
        return ServiceResult.Success();
    }

    private static bool CanChangeStatus(ServiceRequest request, User actor)
    {
        return actor.Role == Roles.Administrator
            || actor.Role == Roles.FacilitiesManager
            || (actor.Role == Roles.SupportTeam
                && actor.TeamId.HasValue
                && actor.TeamId == request.AssignedTeamId);
    }

    private DateTime Now()
    {
        return _time.GetUtcNow().UtcDateTime;
    }
}
