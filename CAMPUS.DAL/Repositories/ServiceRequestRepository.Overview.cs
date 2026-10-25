using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;
using Microsoft.EntityFrameworkCore;

namespace CAMPUS.DAL.Repositories;

// Person 4's part of the request repository. The constructor and _db are in the base file.
public partial class ServiceRequestRepository
{
    public async Task<List<ServiceRequest>> GetAllAsync(RequestStatus? status, int? campusId)
    {
        var query = _db.ServiceRequests.AsNoTracking();

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        if (campusId.HasValue)
            query = query.Where(r => r.CampusId == campusId.Value);

        return await query
            .Include(r => r.Category)
            .Include(r => r.Campus)
            .Include(r => r.Reporter)
            .Include(r => r.AssignedTeam)
            .OrderByDescending(r => r.CreatedAt)
            .ThenByDescending(r => r.Id)
            .ToListAsync();
    }

    public async Task<List<ServiceRequest>> GetByTeamAsync(int teamId, RequestStatus? status)
    {
        var query = _db.ServiceRequests
            .AsNoTracking()
            .Where(r => r.AssignedTeamId == teamId);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        return await query
            .Include(r => r.Category)
            .Include(r => r.Campus)
            .Include(r => r.Reporter)
            .OrderBy(r => r.Status == RequestStatus.Resolved)
            .ThenByDescending(r => r.Priority)
            .ThenBy(r => r.CreatedAt)
            .ToListAsync();
    }
}