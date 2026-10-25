using CAMPUS.BLL.Common;
using CAMPUS.DOMAIN.Entities;

namespace CAMPUS.BLL.Interfaces;

public interface IAssignmentService
{
    // Assigns a request to a team, saves it and sends the e-mails.
    Task<ServiceResult> AssignAsync(int requestId, int teamId, User actor);
}