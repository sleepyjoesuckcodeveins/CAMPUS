using CAMPUS.DOMAIN.Entities;

namespace CAMPUS.PL.Auth;

public interface ICurrentUserService
{
    Task<User> GetUserAsync();
}
