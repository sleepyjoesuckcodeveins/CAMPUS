using CAMPUS.BLL.Common;
using CAMPUS.BLL.Interfaces;
using CAMPUS.BLL.Rules;
using CAMPUS.DOMAIN;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;
using CAMPUS.DOMAIN.Repositories;
using System.Security.AccessControl;

namespace CAMPUS.BLL.Services;

// The request lists: all requests for managers, and the team's requests for support (FR-05).
public class OverviewService : IOverviewService
{
    private readonly IServiceRequestRepository _requests;

    public OverviewService(IServiceRequestRepository requests)
    {
        _requests = requests;
    }

    // All requests with the chosen filters; only for facilities managers and administrators.
    public async Task<ServiceResult<List<ServiceRequest>>> GetAllAsync(RequestStatus? status, int? campusId, User actor)
    {
        if (!AccessRules.CanSeeAll(actor))
            return ServiceResult<List<ServiceRequest>>.Forbidden();

        var requests = await _requests.GetAllAsync(status, campusId);
        return ServiceResult<List<ServiceRequest>>.Success(requests);
    }

    // The requests assigned to the actor's team; only for support team members.
    public async Task<ServiceResult<List<ServiceRequest>>> GetAssignedAsync(RequestStatus? status, User actor)
    {
        if (actor.Role != Roles.SupportTeam)
            return ServiceResult<List<ServiceRequest>>.Forbidden();

        // A support member without a team has nothing assigned; no need to ask the database.
        if (actor.TeamId is null)
            return ServiceResult<List<ServiceRequest>>.Success(new List<ServiceRequest>());

        var requests = await _requests.GetByTeamAsync(actor.TeamId.Value, status);
        return ServiceResult<List<ServiceRequest>>.Success(requests);
    }
}