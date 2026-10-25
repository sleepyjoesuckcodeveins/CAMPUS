using CAMPUS.DOMAIN.Repositories;

namespace CAMPUS.DAL.Repositories;

// The request repository. This base file holds the constructor and _db for all partial files.
public partial class ServiceRequestRepository : IServiceRequestRepository
{
    private readonly CampusDbContext _db;

    public ServiceRequestRepository(CampusDbContext db)
    {
        _db = db;
    }
}