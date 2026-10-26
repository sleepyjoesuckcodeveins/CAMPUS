using CAMPUS.DOMAIN;
using CAMPUS.DOMAIN.Entities;

namespace CAMPUS.BLL.Rules;

public static class AccessRules
{
    public static bool CanCreate(User actor)
    {
        return actor.Role == Roles.Employee
            || actor.Role == Roles.FacilitiesManager
            || actor.Role == Roles.SupportTeam
            || actor.Role == Roles.Administrator;
    }

    public static bool CanSeeAll(User actor)
    {
        return actor.Role == Roles.FacilitiesManager || actor.Role == Roles.Administrator;
    }

    public static bool CanAssign(User actor)
    {
        return actor.Role == Roles.FacilitiesManager || actor.Role == Roles.Administrator;
    }

    public static bool CanView(ServiceRequest request, User actor)
    {
        if (CanSeeAll(actor))
            return true;

        if (request.ReporterId == actor.Id)
            return true;

        return actor.Role == Roles.SupportTeam
            && actor.TeamId.HasValue
            && actor.TeamId == request.AssignedTeamId;
    }

    public static bool CanChangeStatus(ServiceRequest request, User actor)
    {
        if (CanSeeAll(actor))
            return true;

        return actor.Role == Roles.SupportTeam
            && actor.TeamId.HasValue
            && actor.TeamId == request.AssignedTeamId;
    }
}
