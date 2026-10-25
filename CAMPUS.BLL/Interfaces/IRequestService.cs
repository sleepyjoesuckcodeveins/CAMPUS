using CAMPUS.BLL.Common;
using CAMPUS.BLL.Models;
using CAMPUS.DOMAIN.Entities;

namespace CAMPUS.BLL.Interfaces;

public interface IRequestService
{
    Task<ServiceResult<int>> CreateAsync(RequestInput input, User actor);

    Task<List<ServiceRequest>> GetMineAsync(User actor);

    Task<ServiceResult<ServiceRequest>> GetForViewAsync(int id, User actor);

    RequestPermissions GetPermissions(ServiceRequest request, User actor);

    Task<ServiceResult> UpdateAsync(int id, RequestInput input, User actor);

    Task<ServiceResult> DeleteAsync(int id, User actor);
}