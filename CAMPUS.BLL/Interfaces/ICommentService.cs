using CAMPUS.BLL.Common;
using CAMPUS.DOMAIN.Entities;
using System.Xml.Linq;

namespace CAMPUS.BLL.Interfaces;

public interface ICommentService
{
    // The comments of a request, oldest first.
    Task<ServiceResult<List<Comment>>> GetForRequestAsync(int requestId, User actor);

    // Adds a comment to a request that isn't resolved.
    Task<ServiceResult> AddAsync(int requestId, string? text, User actor);
}