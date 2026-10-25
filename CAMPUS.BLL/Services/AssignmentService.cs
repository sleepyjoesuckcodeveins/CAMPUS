using CAMPUS.BLL.Common;
using CAMPUS.BLL.Interfaces;
using CAMPUS.BLL.Notifications;
using CAMPUS.BLL.Rules;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;
using CAMPUS.DOMAIN.Repositories;
using System.Security.AccessControl;

namespace CAMPUS.BLL.Services;

// Assigning a request to a support team, then e-mailing the reporter and the team (FR-06).
public class AssignmentService : IAssignmentService
{
    private readonly IServiceRequestRepository _requests;
    private readonly ILookupRepository _lookups;
    private readonly INotificationService _notifications;
    private readonly TimeProvider _time;

    public AssignmentService(
        IServiceRequestRepository requests,
        ILookupRepository lookups,
        INotificationService notifications,
        TimeProvider time)
    {
        _requests = requests;
        _lookups = lookups;
        _notifications = notifications;
        _time = time;
    }

    // Assigns or reassigns the request to a team; only for managers and administrators.
    public async Task<ServiceResult> AssignAsync(int requestId, int teamId, User actor)
    {
        if (!AccessRules.CanAssign(actor))
            return ServiceResult.Forbidden();

        var request = await _requests.GetByIdAsync(requestId);
        if (request is null)
            return ServiceResult.NotFound(Messages.RequestNotFound);

        if (request.Status == RequestStatus.Resolved)
            return ServiceResult.Invalid(Messages.RequestAlreadyResolved);

        var team = await _lookups.GetTeamAsync(teamId);
        if (team is null)
            return ServiceResult.Invalid(Messages.TeamNotFound, "TeamId");

        if (request.AssignedTeamId == teamId)
            return ServiceResult.Invalid(Messages.AlreadyAssigned, "TeamId");

        var now = Now();
        request.AssignedTeamId = team.Id;
        request.AssignedTeam = team;
        request.AssignedAt = now;
        request.UpdatedAt = now;

        await _requests.UpdateAsync(request);

        // Save first, then e-mail, so nobody gets an e-mail about a change that wasn't saved.
        await _notifications.RequestAssignedAsync(request);

        return ServiceResult.Success();
    }

    // The current time in UTC, from the injected TimeProvider, so tests can fix it.
    private DateTime Now()
    {
        return _time.GetUtcNow().UtcDateTime;
    }
}