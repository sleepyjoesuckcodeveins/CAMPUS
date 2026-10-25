using CAMPUS.BLL.Common;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;

namespace CAMPUS.BLL.Interfaces;

public interface IOverviewService
{
    // All requests with filters, for managers and admins.
    Task<ServiceResult<List<ServiceRequest>>> GetAllAsync(RequestStatus? status, int? campusId, User actor);

    // The requests of the actor's team, for support members.
    Task<ServiceResult<List<ServiceRequest>>> GetAssignedAsync(RequestStatus? status, User actor);
}