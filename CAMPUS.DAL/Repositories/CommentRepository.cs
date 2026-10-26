using CAMPUS.DOMAIN.Repositories;

namespace CAMPUS.DAL.Repositories;

public partial class CommentRepository : ICommentRepository
{
    private readonly CampusDbContext _db;

    public CommentRepository(CampusDbContext db)
    {
        _db = db;
    }
}
