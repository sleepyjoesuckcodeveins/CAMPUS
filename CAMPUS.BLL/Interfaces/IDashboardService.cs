using CAMPUS.BLL.Models;
using CAMPUS.DOMAIN.Entities;

namespace CAMPUS.BLL.Interfaces;

public interface IDashboardService
{
    // The four counts and the five latest requests for the actor's role.
    Task<DashboardStats> GetAsync(User actor);
}