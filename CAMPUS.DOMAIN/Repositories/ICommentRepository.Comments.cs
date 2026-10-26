using CAMPUS.DOMAIN.Entities;

namespace CAMPUS.DOMAIN.Repositories;

public partial interface ICommentRepository
{
    Task<List<Comment>> GetByRequestAsync(int requestId);

    Task AddAsync(Comment comment);
}
