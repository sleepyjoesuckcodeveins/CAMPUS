using CAMPUS.DOMAIN.Entities;
using Microsoft.EntityFrameworkCore;

namespace CAMPUS.DAL.Repositories;

// Person 3's part of the request repository. The constructor and _db are in Person 1's base file.
public partial class ServiceRequestRepository
{
    public async Task<ServiceRequest?> GetByIdAsync(int id)
    {
        return await _db.ServiceRequests
            .Include(r => r.Category)
            .Include(r => r.Campus)
            .Include(r => r.Reporter)
            .Include(r => r.AssignedTeam)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<ServiceRequest>> GetByReporterAsync(int reporterId)
    {
        return await _db.ServiceRequests
            .AsNoTracking()
            .Where(r => r.ReporterId == reporterId)
            .Include(r => r.Category)
            .Include(r => r.Campus)
            .Include(r => r.AssignedTeam)
            .OrderByDescending(r => r.CreatedAt)
            .ThenByDescending(r => r.Id)
            .ToListAsync();
    }

    public async Task AddAsync(ServiceRequest request)
    {
        _db.ServiceRequests.Add(request);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(ServiceRequest request)
    {
        // A request from GetByIdAsync is already tracked; only attach it when it isn't.
        if (_db.Entry(request).State == EntityState.Detached)
            _db.ServiceRequests.Update(request);

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(ServiceRequest request)
    {
        _db.ServiceRequests.Remove(request);
        await _db.SaveChangesAsync();
    }
}