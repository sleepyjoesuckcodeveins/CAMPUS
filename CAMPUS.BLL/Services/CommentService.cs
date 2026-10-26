using CAMPUS.BLL.Common;
using CAMPUS.BLL.Interfaces;
using CAMPUS.BLL.Rules;
using CAMPUS.DOMAIN;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Repositories;

namespace CAMPUS.BLL.Services;

public class CommentService : ICommentService
{
    private readonly IServiceRequestRepository _requests;
    private readonly ICommentRepository _comments;
    private readonly TimeProvider _time;

    public CommentService(
        IServiceRequestRepository requests,
        ICommentRepository comments,
        TimeProvider time)
    {
        _requests = requests;
        _comments = comments;
        _time = time;
    }

    public async Task<ServiceResult<List<Comment>>> GetForRequestAsync(int requestId, User actor)
    {
        var request = await _requests.GetByIdAsync(requestId);
        if (request is null)
            return ServiceResult<List<Comment>>.NotFound(Messages.RequestNotFound);

        if (!CanView(request, actor))
            return ServiceResult<List<Comment>>.Forbidden();

        var comments = await _comments.GetByRequestAsync(requestId);
        return ServiceResult<List<Comment>>.Success(comments);
    }

    public async Task<ServiceResult> AddAsync(int requestId, string? text, User actor)
    {
        var request = await _requests.GetByIdAsync(requestId);
        if (request is null)
            return ServiceResult.NotFound(Messages.RequestNotFound);

        if (!CanView(request, actor))
            return ServiceResult.Forbidden();

        if (request.Status == DOMAIN.Enums.RequestStatus.Resolved)
            return ServiceResult.Invalid(Messages.CommentsClosed);

        var check = CommentRules.Validate(text);
        if (!check.Succeeded)
            return check;

        await _comments.AddAsync(new Comment
        {
            ServiceRequestId = request.Id,
            AuthorId = actor.Id,
            Text = text!.Trim(),
            CreatedAt = Now()
        });

        return ServiceResult.Success();
    }

    private static bool CanView(ServiceRequest request, User actor)
    {
        if (actor.Role == Roles.Administrator || actor.Role == Roles.FacilitiesManager)
            return true;

        if (request.ReporterId == actor.Id)
            return true;

        return actor.Role == Roles.SupportTeam
            && actor.TeamId.HasValue
            && actor.TeamId == request.AssignedTeamId;
    }

    private DateTime Now()
    {
        return _time.GetUtcNow().UtcDateTime;
    }
}
