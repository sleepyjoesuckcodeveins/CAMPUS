using CAMPUS.DOMAIN.Entities;

namespace CAMPUS.DOMAIN.Repositories;

public interface ILookupRepository
{
    Task<Category?> GetCategoryAsync(int id);

    Task<Campus?> GetCampusAsync(int id);

    Task<Team?> GetTeamAsync(int id);
}
