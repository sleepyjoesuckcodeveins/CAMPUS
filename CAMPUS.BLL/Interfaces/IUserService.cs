using CAMPUS.BLL.Common;
using CAMPUS.DOMAIN.Entities;

namespace CAMPUS.BLL.Interfaces;

public interface IUserService
{
    // Gets the user for this login, creating or updating them when needed.
    Task<User> GetOrCreateAsync(string email, string name, string role);

    // All users, for admins only.
    Task<ServiceResult<List<User>>> GetAllAsync(User actor);

    // Links a support member to a team, or removes them when teamId is null.
    Task<ServiceResult> SetTeamAsync(int userId, int? teamId, User actor);
}