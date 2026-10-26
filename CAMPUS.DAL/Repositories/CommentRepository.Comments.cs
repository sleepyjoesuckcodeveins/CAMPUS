using CAMPUS.DOMAIN.Entities;
using Microsoft.EntityFrameworkCore;

namespace CAMPUS.DAL.Repositories;

public partial class CommentRepository
{
    public async Task<List<Comment>> GetByRequestAsync(int requestId)
    {
        return await _db.Comments
            .AsNoTracking()
            .Where(c => c.ServiceRequestId == requestId)
            .Include(c => c.Author)
            .OrderBy(c => c.CreatedAt)
            .ThenBy(c => c.Id)
            .ToListAsync();
    }

    public async Task AddAsync(Comment comment)
    {
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();
    }
}
