using CAMPUS.DOMAIN.Entities;

namespace CAMPUS.DOMAIN.Repositories;

public interface ILookupRepository
{
    /// <summary>Gets all campuses, ordered by name.</summary>
    Task<List<Campus>> GetCampusesAsync();

    /// <summary>Gets all categories, ordered by id, so Other is last.</summary>
    Task<List<Category>> GetCategoriesAsync();

    /// <summary>Gets all support teams, ordered by name.</summary>
    Task<List<Team>> GetTeamsAsync();

    /// <summary>Gets one campus by id, or null when it doesn't exist.</summary>
    Task<Campus?> GetCampusAsync(int id);

    /// <summary>Gets one category by id, or null when it doesn't exist.</summary>
    Task<Category?> GetCategoryAsync(int id);

    /// <summary>Gets one team by id, or null when it doesn't exist.</summary>
    Task<Team?> GetTeamAsync(int id);
}