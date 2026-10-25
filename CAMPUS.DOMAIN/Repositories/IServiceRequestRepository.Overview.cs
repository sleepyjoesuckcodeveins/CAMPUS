using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;

namespace CAMPUS.DOMAIN.Repositories;

// Person 4's part of the request repository: the lists for managers and support teams.
public partial interface IServiceRequestRepository
{
    // All requests, filtered only on the values that are given, newest first.
    Task<List<ServiceRequest>> GetAllAsync(RequestStatus? status, int? campusId);

    // One team's requests: open ones first, then Urgent to Low, then oldest first.
    Task<List<ServiceRequest>> GetByTeamAsync(int teamId, RequestStatus? status);
}