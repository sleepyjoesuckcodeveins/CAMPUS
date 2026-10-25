using CAMPUS.DOMAIN.Entities;

namespace CAMPUS.BLL.Interfaces;

public interface ILookupService
{
    // Campuses, ordered by name.
    Task<List<Campus>> GetCampusesAsync();

    // Categories, ordered by id (Other last).
    Task<List<Category>> GetCategoriesAsync();

    // Teams, ordered by name.
    Task<List<Team>> GetTeamsAsync();
}